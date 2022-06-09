// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CMS.Web.ViewModels;
using AutoMapper;
using DAL.Models;
using DAL.Core.Interfaces;
using CMS.Web.Authorization;
using CMS.Web.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using DAL.Core;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Authorize(Policies.ManageUsersPolicy)]
    [Area("cpanel")]
    public class AccountController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<AccountController> _logger;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";

        public AccountController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<AccountController> logger)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (GetCurrentUserId() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCode = StatusCodes.Status401Unauthorized });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (GetCurrentUserId() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var user = await _accountManager.GetUserByUserNameAsync(model.Username);
                if (user != null)
                {
                    if (!user.IsEnabled)
                    {
                        ModelState.AddModelError("", "تم تعطيل هذا الحساب، لا يمكنك تسجيل الدخول!");
                    }
                }

                if (ModelState.IsValid)
                {
                    var result = await _accountManager.PasswordSignInAsync(user?.UserName ?? "", model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "فشل تسجيل الدخول.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            await _accountManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [Authorize(Policies.ManageWebsitePolicy)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteRoleAsync(id))
                return BadRequest("لا يمكن حذف هذا الدور، لوجود مستخدمين مضافين إليه");


            RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);

            var (Succeeded, Errors) = await _accountManager.DeleteRoleAsync(appRole);
            if (!Succeeded)
                throw new Exception("حدث خطأ عند حذف الدور: " + string.Join(", ", Errors));
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Users(int? pageSize, int? page = 1)
        {
            var pageNumber = page ?? 1;
            var pageIndex = (page ?? 1) - 1;
            var usersAndRoles = await _accountManager.GetUsersAndRolesAsync(pageIndex, pageSize ?? 50);
            var usersAsIPagedList = new List<(UserViewModel, string[])>(usersAndRoles.Select(a => (_mapper.Map<UserViewModel>(a.User), a.Roles))).ToPagedList(pageNumber, pageSize ?? 50);
            return View(nameof(Users), usersAsIPagedList);
        }

        [HttpPost]
        [Authorize(Policies.ManageWebsitePolicy)]
        public async Task<IActionResult> Delete(IFormCollection formCollection)
        {
            var id = formCollection["Id"];

            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteUserAsync(id))
                throw new Exception("لا يمكن حذف المستخدم، قم بحذف العناصر التي قام بإضافتها ثم قم بحذفه.");


            UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);

            var (Succeeded, Errors) = await _accountManager.DeleteUserAsync(appUser);
            if (!Succeeded)
                throw new Exception("The following errors occurred whilst deleting user: " + string.Join(", ", Errors));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> ViewUsers(int? pageSize, int? page = 1)
        {
            return await Users(pageSize, page);
        }



        [HttpGet]
        [Authorize(Policies.ManageWebsitePolicy)]
        public async Task<IActionResult> Register(string Id)
        {
            UserEditViewModel userEditViewModel = null;
            if (!string.IsNullOrWhiteSpace(Id))
            {
                var user = await _accountManager.GetUserAndRolesAsync(Id);
                userEditViewModel = new UserEditViewModel
                {
                    Configuration = user.Value.User.Configuration,
                    FullName = user.Value.User.FullName,
                    UserName = user.Value.User.UserName,
                    JobTitle = user.Value.User.JobTitle,
                    Email = user.Value.User.Email,
                    RoleId = user.Value.Roles.FirstOrDefault(),
                    Id = user.Value.User.Id,
                    IsEnabled = user.Value.User.IsEnabled,
                };
            }

            var roles = await _accountManager.GetRolesLoadRelatedAsync(-1, -1);
            ViewBag.Roles = new SelectList(roles.Select(a => new { Value = a.Name, Text = a.Name }), "Value", "Text", userEditViewModel?.RoleId);
            return View(userEditViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policies.ManageWebsitePolicy)]
        public async Task<IActionResult> Register(UserEditViewModel user)
        {
            var IsEditMode = !string.IsNullOrWhiteSpace(user.Id);
            ModelState.Remove(nameof(UserEditViewModel.NewPassword));

            if (IsEditMode)
            {
                if (user.EditPassword == false)
                {
                    ModelState.Remove(nameof(UserEditViewModel.CurrentPassword));
                    ModelState.Remove(nameof(UserEditViewModel.ConfirmPassword));
                }
            }

            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");
                ApplicationUser appUser = null;
                if (IsEditMode)
                {
                    var appUserWithRoles = await _accountManager.GetUserAndRolesAsync(user.Id);
                    appUser = appUserWithRoles.Value.User;
                    appUser.Configuration = user.Configuration;
                    appUser.FullName = user.FullName;
                    appUser.UserName = user.UserName;
                    appUser.JobTitle = user.JobTitle;
                    appUser.Email = user.Email;
                    appUser.EmailConfirmed = true;
                    appUser.IsEnabled = user.IsEnabled;
                    var result = new List<(bool, string[])>();

                    var result1 = await _accountManager.DeleteUserRolesAsync(appUser, appUserWithRoles.Value.Roles); result.Add(result1);

                    var result2 = await _accountManager.AddUserToRolesAsync(appUser, new[] { user.RoleId }); result.Add(result2);

                    if (user.EditPassword)
                    {
                        var result3 = await _accountManager.ResetPasswordAsync(appUser, user.ConfirmPassword); result.Add(result3);
                    }
                    var result4 = await _accountManager.UpdateUserAsync(appUser);
                    result.Add(result4);

                    if (result.All(a => a.Item1 == true))
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        AddError(result.SelectMany(a => a.Item2));
                    }
                }
                else
                {
                    appUser = _mapper.Map<ApplicationUser>(user);
                    appUser.EmailConfirmed = true;
                    var (Succeeded, Errors) = await _accountManager.CreateUserAsync(appUser, new[] { user.RoleId }, user.CurrentPassword);

                    if (Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        AddError(Errors);
                    }
                }
            }
            var roles = await _accountManager.GetRolesLoadRelatedAsync(-1, -1);
            ViewBag.Roles = new SelectList(roles.Select(a => new { Value = a.Name, Text = a.Name }), "Value", "Text");
            return View(user);
        }



        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var userid = Utilities.GetUserId(this.User);
            var user = await _accountManager.GetUserAndRolesAsync(userid);
            var userViewModel = new UserViewModel
            {
                Configuration = user.Value.User.Configuration,
                FullName = user.Value.User.FullName,
                UserName = user.Value.User.UserName,
                JobTitle = user.Value.User.JobTitle,
                Email = user.Value.User.Email,
                Id = user.Value.User.Id,
            };
            return View(userViewModel);
        }
        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel userViewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var appUser = await _accountManager.GetUserByIdAsync(Utilities.GetUserId(this.User));
            if (ModelState.IsValid)
            {

                var (Succeeded, Errors) = await _accountManager.UpdatePasswordAsync(appUser, userViewModel.CurrentPassword, userViewModel.NewPassword);

                if (Succeeded)
                    return RedirectToAction(nameof(Manage));

                AddError(new string[] { "خطأ في كلمة السر" });
            }

            return View(userViewModel);
        }


        [HttpPut("users/unblock/{id}")]

        public async Task<IActionResult> UnblockUser(string id)
        {
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var (Succeeded, Errors) = await _accountManager.UpdateUserAsync(appUser);
            if (!Succeeded)
                throw new Exception("The following errors occurred whilst unblocking user: " + string.Join(", ", Errors));


            return NoContent();
        }
        [Authorize(Policies.ManageWebsitePolicy)]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string userId)
        {
            var appUser = await _accountManager.GetUserByIdAsync(userId);
            appUser.IsEnabled = !appUser.IsEnabled;
            var result = await _accountManager.UpdateUserAsync(appUser);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                throw new Exception("حدثت مشكلة ما!");
            }

        }
        [HttpGet("users/me/preferences")]
        public async Task<IActionResult> UserPreferences()
        {
            var userId = Utilities.GetUserId(this.User);
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(userId);
            return Ok(appUser.Configuration);
        }


        [HttpPut("users/me/preferences")]
        public async Task<IActionResult> UserPreferences(string data)
        {
            var userId = Utilities.GetUserId(this.User);
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(userId);

            appUser.Configuration = data;

            var (Succeeded, Errors) = await _accountManager.UpdateUserAsync(appUser);
            if (!Succeeded)
                throw new Exception("The following errors occurred whilst updating User Configurations: " + string.Join(", ", Errors));

            return NoContent();
        }
        [HttpGet("roles/{id}", Name = GetRoleByIdActionName)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);

            return await GetRoleByName(appRole.Name);
        }


        [HttpGet("roles/name/{name}")]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            RoleViewModel roleVM = await GetRoleViewModelHelper(name);

            if (roleVM == null)
                return NotFound(name);

            return Ok(roleVM);
        }



        public async Task<IActionResult> Roles()
        {
            return await Roles(-1, -1);
        }


        [HttpGet("roles/{pageNumber:int}/{pageSize:int}")]

        public async Task<IActionResult> Roles(int pageNumber, int pageSize)
        {
            var roles = await _accountManager.GetRolesLoadRelatedAsync(pageNumber, pageSize);
            return View(_mapper.Map<List<RoleViewModel>>(roles));
        }

        [HttpGet]

        public async Task<IActionResult> EditRolePermissions(string id)
        {
            ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

            var appRolePermissions = await _accountManager.GetRolePermissionsAsync(appRole);
            RoleViewModel roleViewModel = _mapper.Map<RoleViewModel>(appRole);

            var rolePermissions = await _accountManager.GetRolePermissionsAsync(appRole);
            roleViewModel.Permissions = rolePermissions.Select(a => new PermissionViewModel { Description = a.Description, GroupName = a.GroupName, Name = a.Name, Value = a.Value }).ToList();

            roleViewModel.Permissions = ApplicationPermissions.GetAllPermissionValues()?
                            .Select(a => new PermissionViewModel
                            {
                                Name = ApplicationPermissions.GetPermissionByValue(a)?.Name,
                                Value = a,
                                Selected = rolePermissions.Any(_ => _.Value == a),
                                Description = ApplicationPermissions.GetPermissionByValue(a)?.Description,
                                GroupName =
                                ApplicationPermissions.GetPermissionByValue(a)?.GroupName
                            }).ToList();
            return View(roleViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> EditRolePermissions(string id, RoleViewModel role)
        {
            ModelState.Remove(nameof(role.Name));
            ModelState.Remove(nameof(role.Description));
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");

                if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                    return BadRequest("Conflicting role id in parameter and model data");



                ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);


                if (appRole == null)
                    return NotFound(id);


                _mapper.Map<RoleViewModel, ApplicationRole>(role, appRole);

                var (Succeeded, Errors) = await _accountManager.EditRolePermissionsAsync(appRole, role.Permissions?.Where(s => s.Selected)?.Select(p => p.Value).ToArray());
                if (Succeeded)
                    return RedirectToAction("Roles");

                AddError(Errors);

            }

            return View(role);
        }

        [HttpGet]

        public async Task<IActionResult> UpdateRole(string id)
        {
            ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);
            _ = await _accountManager.GetRolePermissionsAsync(appRole);
            RoleViewModel roleViewModel = _mapper.Map<RoleViewModel>(appRole);
            return View(roleViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> UpdateRole(string id, RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");

                if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                    return BadRequest("Conflicting role id in parameter and model data");



                ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

                if (appRole == null)
                    return NotFound(id);


                _mapper.Map<RoleViewModel, ApplicationRole>(role, appRole);

                var (Succeeded, Errors) = await _accountManager.UpdateRoleAsync(appRole);
                if (Succeeded)
                    return RedirectToAction("Roles");

                AddError(Errors);

            }

            return View(role);
        }

        [HttpGet]

        public IActionResult CreateRole()
        {
            ViewBag.AllPermissions = ApplicationPermissions.GetAllPermissionValues()?
                .Select(a => new PermissionViewModel { Name = ApplicationPermissions.GetPermissionByValue(a)?.Name, Value = a }).ToArray();
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> CreateRole(RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");


                ApplicationRole appRole = _mapper.Map<ApplicationRole>(role);

                var (Succeeded, Errors) = await _accountManager.CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
                if (Succeeded)
                {
                    return RedirectToAction("Roles");

                }

                AddError(Errors);
            }

            return View(role);
        }





        [HttpGet("permissions")]

        public IActionResult Permissions()
        {
            return View(_mapper.Map<List<PermissionViewModel>>(ApplicationPermissions.AllPermissions));
        }



        private async Task<UserViewModel> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserViewModel>(userAndRoles.Value.User);
            userVM.Roles = userAndRoles.Value.Roles;

            return userVM;
        }


        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            if (role != null)
                return _mapper.Map<RoleViewModel>(role);


            return null;
        }


        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}

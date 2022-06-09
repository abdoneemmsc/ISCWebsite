using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using CMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;
using CMS.Web.Resources;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Area("cpanel")]
    [Authorize(Authorization.Policies.ManageWebsitePolicy)]
    public class SettingsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<SettingsController> _logger;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SettingsController> _localizer;
        public SettingsController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<SettingsController> logger, IUnitOfWork unitOfWork, IStringLocalizer<SettingsController> localizer)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;

        }
        // GET: Setting
        [HttpGet]
        public IActionResult Index()
        {
            return View(_mapper.Map<SettingViewModel>(_unitOfWork.Settings.Get()));
        }
        [HttpGet]
        public IActionResult PostTypes()
        {
            var PostTypeSettingsViewModel = new
               PostTypeSettingsViewModel
            { PostTypes = _mapper.Map<IEnumerable<PostTypeViewModel>>(_unitOfWork.PostTypes.GetAll())?.ToList() };
            return View(PostTypeSettingsViewModel);
        }
        [HttpPost]
        public IActionResult PostTypes(PostTypeSettingsViewModel PostTypeSettingsVM)
        {
            if (ModelState.IsValid)
            {
                var PostTypeItems = _mapper.Map<IEnumerable<PostType>>(PostTypeSettingsVM.PostTypes);

                foreach (var item in PostTypeItems)
                {
                    _unitOfWork.PostTypes.Update(item);
                }
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(PostTypes));
            }
            return View(PostTypeSettingsVM);
        }
        [HttpPost]
        public IActionResult Index(SettingViewModel settingViewModel)
        {
            if (ModelState.IsValid)
            {
                Setting setting = _unitOfWork.Settings.Get() ?? new Setting { };
                settingViewModel.Id = setting.Id;
                _mapper.Map(settingViewModel, setting);
                if (setting.Id > 0)
                    _unitOfWork.Settings.Update(setting);
                else
                    _unitOfWork.Settings.Add(setting);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(settingViewModel);
        }
    }
}
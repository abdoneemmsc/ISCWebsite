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
    public class MenuController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<MenuController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MenuController> _localizer;

        public MenuController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<MenuController> logger, IUnitOfWork unitOfWork, IStringLocalizer<MenuController> localizer)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;

        }
        public IActionResult Index()
        {
            IEnumerable<Menu> menus = _unitOfWork.Menus.GetAllWithChilds();
            IEnumerable<MenuViewModel> menuViewModels = _mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menus);
            return View(menuViewModels);
        }
        public ActionResult Upsert(int? id)
        {


            Menu menu = _unitOfWork.Menus.Get(id ?? 0);
            MenuViewModel menuViewModel = _mapper.Map<Menu, MenuViewModel>(menu);
            ViewBag.ParentId = new SelectList(_unitOfWork.Menus.GetAll().Where(a => a.Id != menuViewModel?.Id).Select(a => new { a.Id, Name = Lang == "en" ? a.NameEn : a.Name }), "Id", "Name", menuViewModel?.ParentId);

            var postTyepsUrls = _unitOfWork.PostTypes.GetAll().Select(a => new SelectListItem { Text = Lang == "en" ? a.NameEn : a.Name, Value = $"/Posts/Type/{a.Id}" }).ToList();

            var postsUrls = _unitOfWork.Posts.GetAll()?.Where(x => x.Published == true).Select(a => new SelectListItem { Text = Lang == "en" ? a.TitleEn : a.Title, Value = $"/Posts/{a.Id}" }).ToList();


            postsUrls.AddRange(postTyepsUrls.ToList());
            postsUrls.Add(new SelectListItem() { Value = "/Contact", Text = Resource.Contact_Us });

            ViewBag.Urls = new SelectList(postsUrls.Select(a => new { a.Value, a.Text }), "Value", "Text");
            return View(menuViewModel);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(MenuViewModel menuViewModel)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                if (menuViewModel.Id > 0)
                {
                    Menu Menu = _unitOfWork.Menus.Get(menuViewModel.Id);
                    var OldMenuParentId = Menu.ParentId;
                    Menu ChildMenu = _unitOfWork.Menus.Get(menuViewModel.ParentId ?? 0);

                    Menu.NameEn = menuViewModel.NameEn;
                    Menu.Name = menuViewModel.Name;
                    Menu.ParentId = menuViewModel.ParentId;
                    Menu.Order = menuViewModel.Order;
                    Menu.Url = menuViewModel.Url;
                    Menu.IsExtended = menuViewModel.IsExtended;
                    if (ChildMenu?.Id == OldMenuParentId
                        && ChildMenu != null
                        && Menu.ParentId != OldMenuParentId)
                    {
                        ChildMenu.ParentId = Menu.ParentId;
                        _unitOfWork.Menus.Update(ChildMenu);
                    }
                    _unitOfWork.Menus.Update(Menu);
                }
                else
                {
                    Menu menu = _mapper.Map<MenuViewModel, Menu>(menuViewModel);
                    _unitOfWork.Menus.Add(menu);

                }

                _unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(_unitOfWork.Menus.GetAll().Where(a => a.Id != menuViewModel?.Id).Select(a => new { a.Id, Name = Lang == "en" ? a.NameEn : a.Name }), "Id", "Name", menuViewModel?.ParentId);

            var postTyepsUrls = _unitOfWork.PostTypes.GetAll().Select(a => new SelectListItem { Text = Lang == "en" ? a.NameEn : a.Name, Value = $"/Posts/Type/{a.Id}" }).ToList();

            var postsUrls = _unitOfWork.Posts.GetAll().Select(a => new SelectListItem { Text = Lang == "en" ? a.TitleEn : a.Title, Value = $"/Posts/{a.Id}" }).ToList();


            postsUrls.AddRange(postTyepsUrls.ToList());
            postsUrls.Add(new SelectListItem() { Value = "/Contact", Text = Resource.Contact_Us });

            ViewBag.Urls = new SelectList(postsUrls.Select(a => new { a.Value, a.Text }), "Value", "Text");

            return View(menuViewModel);
        }

        // GET: Menu/Delete/5
        public ActionResult Delete(int id)
        {
            Menu menu = _unitOfWork.Menus.Get(id);
            return View(_mapper.Map<MenuViewModel>(menu));
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Menu menu = _unitOfWork.Menus.GeMenuWithChilds(Id);
            menu.Children.ToList().ForEach(a =>
            {
                a.ParentId = null;
                _unitOfWork.Menus.Update(a);
            });
            _unitOfWork.Menus.Remove(menu);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
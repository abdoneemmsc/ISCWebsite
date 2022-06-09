using AutoMapper;
using CMS.Web.ViewModels;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NavbarViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IViewComponentResult Invoke()
        {
            IEnumerable<MenuViewModel> menuViewModels = _mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(_unitOfWork.Menus.GetAllWithChilds());
            ViewBag.Setting = _mapper.Map<SettingViewModel>(_unitOfWork.Settings.Get());
            return View(menuViewModels);
        }
    }
}

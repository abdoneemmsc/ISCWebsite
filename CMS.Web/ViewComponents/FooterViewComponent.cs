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
    public class FooterViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FooterViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IViewComponentResult Invoke()
        {
            SettingViewModel menuViewModels = _mapper.Map<Setting, SettingViewModel>(_unitOfWork.Settings.Get());
            return View(menuViewModels);
        }
    }
}

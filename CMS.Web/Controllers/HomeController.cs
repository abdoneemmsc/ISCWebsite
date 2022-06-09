using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Web.ViewModels;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CMS.Common.Constants;

namespace CMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var SliderType = (byte)PostTypes.MainSlider;
            var Products = (byte)PostTypes.Products;
            var NewsType = (byte)PostTypes.Blog;
            var Services = (byte)PostTypes.Services;
            var ClientType = (byte)PostTypes.Clients;
            var About = (byte)PostTypes.About;
            var Features = (byte)PostTypes.Features;

            HomePageViewModel homePageViewModel = new HomePageViewModel
            {


                About = _mapper.Map<PostViewModel>(_unitOfWork.Posts.GetTop(a => a.TypeId == About && a.Published)?.FirstOrDefault()),

                Sliders = _unitOfWork.Posts.GetTop(a => a.TypeId == SliderType && a.Published == true, 5).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                Features = _unitOfWork.Posts.Find(x => x.TypeId == Features).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                Products = _unitOfWork.Posts.GetTop(a => a.TypeId == Products && a.Published == true, 6).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                News = _unitOfWork.Posts.GetTop(a => a.TypeId == NewsType && a.Published == true, 6).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                Services = _unitOfWork.Posts.GetTop(a => a.TypeId == Services && a.Published == true, 6).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                OurClients = _unitOfWork.Posts.GetTop(a => a.TypeId == ClientType && a.Published == true, 10).Select(a => _mapper.Map<PostViewModel>(a)).ToList(),
                MapUrl = _unitOfWork.Settings.Get().Map,

            };

            return View(homePageViewModel);
        }
    }
}
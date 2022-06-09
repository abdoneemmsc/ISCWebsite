using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Web.Helpers;
using CMS.Web.ViewModels;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace CMS.Web.Controllers
{
    public class PostsController : BaseController
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PostsController(ILogger<PostsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult Type(int id, string Title)
        {
            var Posts = _unitOfWork.Posts.FindWithPostType(a => a.TypeId == id && a.Published == true);
            if (Posts == null)
                return NotFound();
            var postsViewModel = _mapper.Map<IEnumerable<PostViewModel>>(Posts);
            ViewBag.Title = Lang == "en" ? Posts?.FirstOrDefault()?.PostType?.NameEn : Posts?.FirstOrDefault()?.PostType?.Name;


            var title = Lang == "en" ? Posts?.FirstOrDefault()?.PostType?.NameEn : Posts?.FirstOrDefault()?.PostType?.Name;

            if (title?.ToUrlSlug() != Title?.ToUrlSlug())
            {
                return RedirectToAction(nameof(Type), new { Id = id, Title = title.ToUrlSlug() });
            }
            return View(postsViewModel);
        }
        [Route("{culture}/{controller}/{id}")]
        [Route("{culture}/{controller}/{action}/{id}")]
        [Route("{culture}/{controller}/{action}/{id}/{title?}")]
        public IActionResult Index(int id, string Title)
        {

            var Post = _unitOfWork.Posts.GetPostWithChilds(id);
            if (Post == null || Post.Published != true)
                return NotFound();
            var title = Lang == "en" ? Post.TitleEn : Post.Title;

            if (title?.ToUrlSlug() != Title?.ToUrlSlug())
            {
                return RedirectToAction(nameof(Index), new { Id = id, Title = title.ToUrlSlug() });
            }
            PostViewModel postViewModel = _mapper.Map<PostViewModel>(Post);
            return View(postViewModel);
        }
    }
}
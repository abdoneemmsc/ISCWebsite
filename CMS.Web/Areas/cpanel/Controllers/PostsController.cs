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
using CMS.Web.Classes;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Area("cpanel")]
    public class PostsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<PostsController> _logger;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<PostsController> _localizer;

        public PostsController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<PostsController> logger, IUnitOfWork unitOfWork, IStringLocalizer<PostsController> localizer)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;

        }
        // GET: Post

        [Authorize(Authorization.Policies.ManageWebsitePolicy)]
        public IActionResult Index()
        {
            ViewBag.PostStatusTypes = _unitOfWork.PostTypes.GetAll()?.Select(a => new SelectListItem() { Text = Lang == "en" ? a.NameEn : a.Name, Value = a.Id.ToString() }).ToList();
            return View(_unitOfWork.Posts.GetAllWithChilds().OrderByDescending(a => a.CreatedDate)
                .Select(a => _mapper.Map<PostViewModel>(a)));
        }
        public ActionResult Details(int id)
        {
            PostViewModel PostViewModel = new PostViewModel { };
            var post = _unitOfWork.Posts.GetPostWithChilds(id);
            if (post == null)
            {
                return NotFound();
            }
            _mapper.Map(post, PostViewModel);
            return View(PostViewModel);
        }

        // GET: Post/Create
        public ActionResult Upsert(int? Id)
        {
            PostViewModel PostViewModel = new PostViewModel { };
            if (Id > 0)
            {
                var post = _unitOfWork.Posts.GetPostWithChilds(Id ?? 0);
                if (post == null)
                {
                    return NotFound();
                }
                _mapper.Map(post, PostViewModel);
                PostViewModel.MainImageFile = new FileUploaderViewModel((nameof(PostViewModel.MainImageFile)), false) { FileName = Path.GetFileName(PostViewModel.MainImageUrl), IsUploaded = PostViewModel.MainImageUrl != null, FileUrl = PostViewModel.MainImageUrl };

                PostViewModel.IconFile = new FileUploaderViewModel((nameof(PostViewModel.IconFile)), false) { FileName = Path.GetFileName(PostViewModel.IconUrl), IsUploaded = PostViewModel.IconUrl != null, FileUrl = PostViewModel.IconUrl };

            }
            ViewBag.Types = new SelectList(_unitOfWork.PostTypes.GetAll().Select(a => new { Id = a.Id, Title = Lang == "en" ? a.NameEn : a.Name }), "Id", "Title");
            return View(PostViewModel);
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(PostViewModel PostViewModel)
        {
            ModelState.Remove("Id");

            if (PostViewModel.MainImageFile.IsUploaded)
            {
                ModelState.Remove(nameof(PostViewModel.MainImageUrl));
                PostViewModel.MainImageUrl = PostViewModel.MainImageFile.FileUrl;
            }
            if (PostViewModel.IconFile.IsUploaded)
            {
                ModelState.Remove(nameof(PostViewModel.IconUrl));
                PostViewModel.IconUrl = PostViewModel.IconFile.FileUrl;
            }
            if (ModelState.IsValid)
            {
                if (PostViewModel.Id > 0)
                {
                    var post = _unitOfWork.Posts.GetPostWithChilds(PostViewModel.Id ?? 0);
                    if (post == null)
                    {
                        return NotFound();
                    }
                    _mapper.Map(PostViewModel, post);
                    _unitOfWork.Posts.Update(post);
                }
                else
                {
                    Post post = new Post { };
                    _mapper.Map(PostViewModel, post);

                    _unitOfWork.Posts.Add(post);
                }
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Types = new SelectList(_unitOfWork.PostTypes.GetAll().Select(a => new { Id = a.Id, Title = Lang == "en" ? a.NameEn : a.Name }), "Id", "Title", PostViewModel.TypeId);
            return View(PostViewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Post post = _unitOfWork.Posts.Get(id);
            return View(_mapper.Map<PostViewModel>(post));
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int PostToDeleteId)
        {
            var Post = _unitOfWork.Posts.GetPostWithChilds(PostToDeleteId);
            if (Post != null)
            {
                DeleteFile(Post.MainImageUrl);
                DeleteFile(Post.IconUrl);
                if (Post.PostImages?.Any() == true)
                {
                    foreach (var item in Post.PostImages)
                    {
                        DeleteFile(item.ImageUrl);
                    }
                }
                _unitOfWork.Posts.Remove(Post);
                _unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file, string FieldId)
        {

            // full path to file in temp location
            FileUploaderViewModel fileUploaderViewModel = new FileUploaderViewModel(FieldId, false) { };
            var filename = $"{DateTime.Now.Ticks}_{file.FileName}";
            var validExtensions = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf" };
            var ErrorMessages = new List<string>();
            var fileExt = Path.GetExtension(filename);
            long fileMaxSize = 10485760;//10MB

            if (!(validExtensions.Any(_ => _ == fileExt.ToLower())))
            {
                ErrorMessages.Add("امتداد الملف غير صالح فقط pdf,gif,jpeg,jpg,png");
            }
            if (file.Length > fileMaxSize)
            {
                ErrorMessages.Add("أعلى حد لحجم الملف هو 5 ميقابايت");
            }
            if (ErrorMessages?.Count > 0)
            {
                fileUploaderViewModel.ErrorMessages = ErrorMessages;
            }

            else
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", filename);
                UploadingFiles f = new UploadingFiles();
                f.Extensions = new string[] { ".png", ".jpg", ".jpeg", ".gif" };
                f.FileToUpload = file;
                f.MaxFileSize = 20097152;
                f.Height = 0;
                f.Width = 1400;
                f.newFileName = filename;
                f.upload();
                if (f.ErrorMessage?.Any() == true)
                {
                    throw new Exception(string.Join(',', f.ErrorMessage));
                }
                fileUploaderViewModel.FileName = file.FileName?.Split('.')?.FirstOrDefault();
                fileUploaderViewModel.FileUrl = $"/Uploads/{filename}";
                fileUploaderViewModel.IsUploaded = true;
                fileUploaderViewModel.FieldId = FieldId;
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            //return Json(new { data = "success" });
            return PartialView("FileUploader", fileUploaderViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UploadMultiFiles(PostViewModel postViewModel, List<IFormFile> files)
        {
            if (postViewModel.PostImages == null)
                postViewModel.PostImages = new List<PostImageViewModel> { };
            foreach (var file in files)
            {
                var filename = $"{DateTime.Now.Ticks}_{file.FileName}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", filename);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    postViewModel.PostImages.Add(new PostImageViewModel { ImageUrl = $"/Uploads/{filename}" });
                }
            }
            return PartialView("PostImages", postViewModel.PostImages);

        }



        [HttpPost]
        public IActionResult DeleteFile(string FileUrl, string FieldId)
        {
            DeleteFile(FileUrl);
            return PartialView("FileUploader", new FileUploaderViewModel(FieldId, false) { });
        }
        [HttpPost]
        public IActionResult DeleteImage(string Url)
        {
            DeleteFile(Url);
            return NoContent();
        }
    }
}
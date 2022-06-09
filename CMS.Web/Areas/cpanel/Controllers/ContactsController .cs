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
    public class ContactsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ContactsController> _logger;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<ContactsController> _localizer;

        public ContactsController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<ContactsController> logger, IUnitOfWork unitOfWork, IStringLocalizer<ContactsController> localizer)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;

        }
        // GET: Contact

        [Authorize(Authorization.Policies.ManageWebsitePolicy)]
        public IActionResult Index()
        {
            return View(_unitOfWork.Contacts.GetAll()
                .Select(a => _mapper.Map<ContactViewModel>(a)));
        }
        public ActionResult Details(int id)
        {
            ContactViewModel ContactViewModel = new ContactViewModel { };


            var Contact = _unitOfWork.Contacts.Get(id);
            if (Contact == null)
            {
                return NotFound();
            }
            _mapper.Map(Contact, ContactViewModel);
            return View(ContactViewModel);
        }


        // Contact: Contact/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int ContactToDeleteId)
        {
            var Contact = _unitOfWork.Contacts.Get(ContactToDeleteId);
            if (Contact != null)
            {
                _unitOfWork.Contacts.Remove(Contact);
                _unitOfWork.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
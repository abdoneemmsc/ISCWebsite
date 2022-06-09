using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Web.Resources;
using CMS.Web.ViewModels;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace CMS.Web.Controllers
{
    public class ContactController : BaseController
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContactController(ILogger<ContactController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactViewModel contactViewModel)
        {
            if (ModelState.IsValid)
            {
                var contact = _mapper.Map<Contact>(contactViewModel);
                _unitOfWork.Contacts.Add(contact);
                _unitOfWork.SaveChanges();
                TempData["success"] = Resource.Your_message_has_been_sent_successfully__Thank_you__You_will_be_answered_soon_;
                return RedirectToAction(nameof(Index));
            }
            return View(contactViewModel);

        }
    }
}
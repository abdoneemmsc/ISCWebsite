using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    public class AccountController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCode = StatusCodes.Status401Unauthorized });
        }
    }
}
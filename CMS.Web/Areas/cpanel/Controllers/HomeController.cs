using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Authorize]
    [Area("cpanel")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Posts");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            Global.CultureName = culture;
            if (returnUrl != null)
            {
                var newReturnUrl = returnUrl.Split("/");
                if (newReturnUrl[1] == "ar" || newReturnUrl[1] == "en")
                    newReturnUrl[1] = culture;
                else
                    newReturnUrl[0] = culture;

                returnUrl = string.Join("/", newReturnUrl);
                if (!returnUrl.StartsWith("/"))
                    returnUrl = "/" + returnUrl;
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }
}

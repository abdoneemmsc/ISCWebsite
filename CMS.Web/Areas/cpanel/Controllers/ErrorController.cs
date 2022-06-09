using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Area("cpanel")]
    public class ErrorController : Controller
    {
        [Route("cpanel/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case StatusCodes.Status404NotFound:
                    ViewBag.ExceptionMessage = "الصفحة غير موجودة";
                    break;
                case StatusCodes.Status401Unauthorized:
                    ViewBag.ExceptionMessage = "ليس لديك الصلاحيات الكافية للوصول إلى هذه الصفحة!";
                    break;
                default:
                    ViewBag.ExceptionMessage = $"حدث خطأ صفحة {statusCode}";
                    break;

            }
            return View("Index");
        }
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            // Retrieve the exception Details
            var exceptionHandlerPathFeature =
                    HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptionHandlerPathFeature.Path;
            ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;
            return View("Index");
        }
    }
}
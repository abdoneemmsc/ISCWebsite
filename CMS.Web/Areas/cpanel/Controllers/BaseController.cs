using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMS.Web.Areas.cpanel.Controllers
{
    [Area("cpanel")]
    public class BaseController : Controller
    {
        protected int rowNumber = 1;
        public const string LangParam = "culture";
        public const string CookieName = "app.language";
        private const string Cultures = "ar en";
        public string Lang;

        protected int rowCounter(ref int i, int rowCount = 50)
        {
            int pageNumber = 0;
            var row = 1;
            try
            {
                rowCount = Convert.ToInt32((HttpContext.Request.Query["rows"][0] == null) ? "1" : HttpContext.Request.Query["rows"][0]);
            }
            catch (Exception)
            {

            }
            try { pageNumber = Convert.ToInt32((HttpContext.Request.Query["page"][0] == null) ? "1" : HttpContext.Request.Query["page"][0]); } catch { }
            if ((pageNumber) > 1)
            {
                row = (rowCount * (pageNumber - 1)) + 1;
            }
            return row - 1 + i++;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Try getting culture from URL first

            var culture = (string)filterContext.RouteData.Values[LangParam];
            Lang = culture;
            // If not provided, or the culture does not match the list of known cultures, try cookie or browser setting
            if (string.IsNullOrEmpty(culture) || !Cultures.Contains(culture))
            {
                // load the culture info from the cookie
                var cookie = filterContext.HttpContext.Request.Cookies[CookieName];
                if (cookie != null)
                {
                    // set the culture by the cookie content
                    culture = cookie;
                }
                else
                {
                    // set the culture by the location if not specified
                    culture = "ar";
                }
                // set the lang value into route data
                filterContext.RouteData.Values[LangParam] = culture;
            }

            // Keep the part up to the "-" as the primary language
            var language = culture.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0];


            filterContext.RouteData.Values[LangParam] = language;

            // Set the language - ignore specific culture for now
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);

            // save the locale into cookie (full locale)
            filterContext.HttpContext.Response.Cookies.Append(
         CookieRequestCultureProvider.DefaultCookieName,
         CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
         new CookieOptions
         {
             Expires = DateTimeOffset.UtcNow.AddYears(1)
         }
          );


            // Pass on to normal controller processing
            Global.CultureName = language;
            base.OnActionExecuting(filterContext);
            ViewBag.lang = language?.ToLower();

        }
        protected void DeleteFile(string FileUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + FileUrl;
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        protected string GetCurrentUserId()
        {
            return (Utilities.GetUserId(this.User));
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
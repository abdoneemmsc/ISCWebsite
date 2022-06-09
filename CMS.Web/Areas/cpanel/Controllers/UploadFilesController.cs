using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    [Area("cpanel")]
    [Authorize(Authorization.Policies.ManageWebsitePolicy)]
    public class UploadFilesController : BaseController
    {
        const string filesavepath = "~/Uploads/";
        const string baseUrl = @"/Uploads/";

        const string scriptTag = "<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction({0}, '{1}', '{2}')</script>";

        public IActionResult Index(IFormCollection from)
        {

            var funcNum = 0;
            var files = Request.Form.Files;
            int.TryParse(Request.Query["CKEditorFuncNum"], out funcNum);

            if (files == null || files?.Count() < 1)
                return BuildReturnScript(funcNum, null, "No file has been sent");

            string fileName = string.Empty;
            SaveAttatchedFile(filesavepath, files, ref fileName);
            var url = baseUrl + fileName;

            return BuildReturnScript(funcNum, url, null);
        }

        private ContentResult BuildReturnScript(int functionNumber, string url, string errorMessage)
        {


            return Content(
                string.Format(scriptTag, functionNumber, HttpUtility.JavaScriptStringEncode(url ?? ""), HttpUtility.JavaScriptStringEncode(errorMessage ?? "")),
                "text/html"
                );
        }

        private void SaveAttatchedFile(string filepath, IFormFileCollection files, ref string fileName)
        {

            for (int i = 0; i < files?.Count; i++)
            {
                var file = files[i];
                if (file != null && file.Length > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    fileName = Guid.NewGuid() + fileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", fileName);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
        }
    }
}

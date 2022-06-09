using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class FileUploaderViewModel
    {
        public FileUploaderViewModel(string fieldId, bool isRequired)
        {
            ErrorMessages = new List<string>();
            FieldId = fieldId;
            this.IsRequired = isRequired;
        }
        [MaxLength(500)]
        [Display(Name = "الملف")]
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public bool IsUploaded { get; set; }
        public string FieldId { get; set; }
        public bool IsRequired { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}

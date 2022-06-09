using CMS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Username), ResourceType = typeof(Resource))]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Resource.Password), ResourceType = typeof(Resource))]
        public string Password { get; set; }
        [Display(Name = nameof(Resource.RememberMe), ResourceType = typeof(Resource))]
        public bool RememberMe { get; set; }
    }
}

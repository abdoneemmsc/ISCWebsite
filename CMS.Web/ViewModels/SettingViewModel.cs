using CMS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class SettingViewModel
    {
        public int Id { get; set; }

        [Display(Name = nameof(Resource.Map), ResourceType = typeof(Resource))]
        public string Map { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Email), ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Phone), ResourceType = typeof(Resource))]
        public string Phone { get; set; }

        [MaxLength(1000)]
        [Display(Name = nameof(Resource.Address), ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [MaxLength(1000)]
        [Display(Name = nameof(Resource.AddressEn), ResourceType = typeof(Resource))]
        public string AddressEn { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Twitter), ResourceType = typeof(Resource))]
        public string Twitter { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Facebook), ResourceType = typeof(Resource))]
        public string Facebook { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Instagram), ResourceType = typeof(Resource))]
        public string Instagram { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Whatsapp), ResourceType = typeof(Resource))]
        public string Whatsapp { get; set; }

        [MaxLength(250)]
        [Display(Name = nameof(Resource.Youtube), ResourceType = typeof(Resource))]
        public string Youtube { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Snapchat), ResourceType = typeof(Resource))]
        public string Snapchat { get; set; }

        [MaxLength(50)]
        [Display(Name = nameof(Resource.Linkedin), ResourceType = typeof(Resource))]
        public string Linkedin { get; set; }
    }
}

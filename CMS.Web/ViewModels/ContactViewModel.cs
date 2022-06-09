using CMS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class ContactViewModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Name), ResourceType = typeof(Resource))]
        public string Name { get; set; }
        [MaxLength(100)]
        [EmailAddress(ErrorMessageResourceName = nameof(Resource.EmailIsInvalid), ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Email), ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Phone), ResourceType = typeof(Resource))]
        public string Phone { get; set; }

        [MaxLength(1000)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Message), ResourceType = typeof(Resource))]
        public string Message { get; set; }
        [Display(Name = nameof(Resource.CreatedDate), ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}

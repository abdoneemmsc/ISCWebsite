using CMS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            Children = new List<MenuViewModel>();
        }
        public int Id { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Menu_title), ResourceType = typeof(Resource))]
        public string Name { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Menu_title_en), ResourceType = typeof(Resource))]
        public string NameEn { get; set; }
        [Display(Name = nameof(Resource.Parent_menu), ResourceType = typeof(Resource))]
        public int? ParentId { get; set; }
        [MaxLength(500)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.Url), ResourceType = typeof(Resource))]
        public string Url { get; set; }
        public bool? HasChilds { get { return Children?.Count() > 0; } }
        public virtual MenuViewModel Parent { get; set; }
        [Display(Name = nameof(Resource.Order), ResourceType = typeof(Resource))]
        public int? Order { get; set; }

        [Display(Name = nameof(Resource.ExtendedMenu), ResourceType = typeof(Resource))]
        public bool IsExtended { get; set; } = false;

        public virtual List<MenuViewModel> Children { get; set; }
    }

}

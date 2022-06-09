// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using CMS.Web.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace CMS.Web.ViewModels
{


    public class PostViewModel
    {
        public PostViewModel()
        {
            MainImageFile = new FileUploaderViewModel(nameof(MainImageFile), false);
            IconFile = new FileUploaderViewModel(nameof(IconFile), false);
        }
        public int? Id { get; set; }
        [MaxLength(500, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.Less_than__0_))]

        [Display(Name = nameof(Resource.MainImage), ResourceType = typeof(Resource))]
        public string MainImageUrl { get; set; }

        [Display(Name = nameof(Resource.SideImage), ResourceType = typeof(Resource))]
        public string IconUrl { get; set; }

        [Display(Name = nameof(Resource.Title), ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        public string Title { get; set; }
        [Display(Name = nameof(Resource.TitleEn), ResourceType = typeof(Resource))]
        public string TitleEn { get; set; }

        [Display(Name = nameof(Resource.Body), ResourceType = typeof(Resource))]
        public string Body { get; set; }

        [Display(Name = nameof(Resource.BodyEn), ResourceType = typeof(Resource))]
        public string BodyEn { get; set; }

        [Display(Name = nameof(Resource.Type), ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        public byte TypeId { get; set; }

        [Display(Name = nameof(Resource.CreatedDate), ResourceType = typeof(Resource))]
        public DateTime CreatedDate { get; set; }

        [Display(Name = nameof(Resource.UpdatedDate), ResourceType = typeof(Resource))]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = nameof(Resource.Hits), ResourceType = typeof(Resource))]
        public int Hits { get; set; } = 0;
        [Display(Name = nameof(Resource.Published), ResourceType = typeof(Resource))]
        public bool Published { get; set; } = false;

        /*Files*/
        [Display(Name = nameof(Resource.MainImage), ResourceType = typeof(Resource))]
        public FileUploaderViewModel MainImageFile { get; set; }

        [Display(Name = nameof(Resource.SideImage), ResourceType = typeof(Resource))]
        public FileUploaderViewModel IconFile { get; set; }

        public PostTypeViewModel PostType { get; set; }

        [Display(Name = nameof(Resource.More_Images), ResourceType = typeof(Resource))]
        public List<PostImageViewModel> PostImages { get; set; }
        [Display(Name = nameof(Resource.CreatedBy), ResourceType = typeof(Resource))]
        public string CreatedByName { get; set; }

    }

    public class PostTypeViewModel
    {
        public byte Id { get; set; }

        [Display(Name = nameof(Resource.Type), ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        public string Name { get; set; }

        [Display(Name = nameof(Resource.TypeEn), ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        public string NameEn { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.Less_than__0_))]
        [Display(Name = nameof(Resource.Description), ResourceType = typeof(Resource))]

        public string Description { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.Less_than__0_))]
        [Display(Name = nameof(Resource.DescriptionEn), ResourceType = typeof(Resource))]
        public string DescriptionEn { get; set; }
    }
    public class PostTypeSettingsViewModel
    {
        public List<PostTypeViewModel> PostTypes { get; set; }
    }
    public class PostImageViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.FieldIsRequired))]
        [Display(Name = nameof(Resource.ImageUrl), ResourceType = typeof(Resource))]
        public string ImageUrl { get; set; }

    }
}

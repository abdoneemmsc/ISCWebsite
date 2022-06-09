// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using CMS.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Display(Name ="الدور")]
        [Required(ErrorMessage = "اسم الدور مطلوب"), StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        public int UsersCount { get; set; }

        public List<PermissionViewModel> Permissions { get; set; }


    }
}

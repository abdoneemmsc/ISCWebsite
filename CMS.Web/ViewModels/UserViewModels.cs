// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using FluentValidation;
using CMS.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace CMS.Web.ViewModels
{
    public class UserViewModel : UserBaseViewModel
    {
        public bool IsLockedOut { get; set; }

        [MinimumCount(1, ErrorMessage = "Roles cannot be empty")]
        public string[] Roles { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر الحالية")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب")]
        [StringLength(100, ErrorMessage = " {0} على الأقل {2} وعلى الأكثر {1} حروف.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر الجديدة")]
        [Compare("ConfirmPassword", ErrorMessage = "كلمتا السر غير متطابقتين.")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر الجديدة")]
        public string ConfirmPassword { get; set; }
    }

    public class UserEditViewModel : UserBaseViewModel
    {
        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "كلمتا السر غير متطابقتين.")]
        [Display(Name = "كلمة السر")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب")]
        [StringLength(100, ErrorMessage = " {0} على الأقل {2} وعلى الأكثر {1} حروف.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر الجديدة")]
        [Compare("ConfirmPassword", ErrorMessage = "كلمتا السر غير متطابقتين.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب")]
        [Display(Name = "الدور")]
        public string RoleId { get; set; }
        public string[] Roles { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر الجديدة")]
        public string ConfirmPassword { get; set; }
        public bool EditPassword { get; set; } = false;

    }



    public class UserPatchViewModel
    {
        public string FullName { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }
    }



    public abstract class UserBaseViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب"), StringLength(200, MinimumLength = 2, ErrorMessage = "اسم المستخدم يجب أن يحتوي على رمزين على الأقل")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Display(Name = "الاسم كاملا")]
        public string FullName { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "الحقل \"{0}\" مطلوب"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Display(Name = "رقم التلفون")]
        public string PhoneNumber { get; set; }

        [Display(Name = "جهة العمل")]
        public string JobTitle { get; set; }

        public string Configuration { get; set; }

        public bool IsEnabled { get; set; }



    }




    //public class UserViewModelValidator : AbstractValidator<UserViewModel>
    //{
    //    public UserViewModelValidator()
    //    {
    //        //Validation logic here
    //        RuleFor(user => user.UserName).NotEmpty().WithMessage("Username cannot be empty");
    //        RuleFor(user => user.Email).EmailAddress().NotEmpty();
    //        RuleFor(user => user.Password).NotEmpty().WithMessage("Password cannot be empty").Length(4, 20);
    //    }
    //}
}

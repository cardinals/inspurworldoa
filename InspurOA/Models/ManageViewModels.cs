using InspurOA.Identity.Core;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspurOA.Web.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        //public IList<InspurUserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<InspurUserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "'{0}'是必填项。")]
        [StringLength(100, ErrorMessage = "{0} 至少包含{2}个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新 密 码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "必填字段")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项。")]
        [StringLength(100, ErrorMessage = "{0} 至少包含{2}个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新  密  码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "'{0}'是必填项。")]
        [Phone(ErrorMessage = "'{0}'不是有效的电话号码")]
        [Display(Name = "电话号码")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "'{0}'是必填项。")]
        [Display(Name = "验证码")]
        public string Code { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项。")]
        [Phone]
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}
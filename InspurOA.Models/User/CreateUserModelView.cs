using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models.User
{
    public class CreateUserModelView
    {
        [Required]
        [StringLength(20, ErrorMessage = "{0}长度不能小于{2}.", MinimumLength = 2)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0}长度不能小于{2}.", MinimumLength = 2)]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "公司邮箱")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}长度不能小于{2}", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码不匹配")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Compare("PhoneNumber", ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "职位")]
        public string Job { get; set; }
    }
}

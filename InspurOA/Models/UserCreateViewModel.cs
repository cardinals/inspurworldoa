using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspurOA.Web.Models
{
    public class UserCreateViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "'{0}' w。")]
        [StringLength(20, ErrorMessage = "'{0}'最少需要{2}个字符，", MinimumLength = 2)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项。")]
        [EmailAddress(ErrorMessage = "请输入正确的邮箱地址。")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项。")]
        [StringLength(100, ErrorMessage = "'{0}'最少需要{2}位。", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "两次输入的密码不匹配。")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项。")]
        [Display(Name = "所属角色")]
        public string RoleCode { get; set; }
    }
}
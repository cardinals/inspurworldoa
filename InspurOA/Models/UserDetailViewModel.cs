using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspurOA.Web.Models
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "所属角色")]
        public string RoleCode { get; set; }

        public List<string> Permissions { get; set; }
    }
}
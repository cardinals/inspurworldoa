using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class RoleViewModel
    {
        public string RoleId { get; set; }        

        [Required(ErrorMessage ="'{0}'是必填项")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "角色代号")]
        public string RoleCode { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "角色描述")]
        public string RoleDescription { get; set; }

        [Required(ErrorMessage = "至少选择一个'{0}'")]
        [Display(Name = "角色权限")]
        public string Permissions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class PermissionViewModel
    {
        public string PermissionId { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项")]
        [StringLength(20, ErrorMessage = "{0}长度不能小于{2}.", MinimumLength = 4)]
        [Display(Name = "权限代码")]
        public string PermissionCode { get; set; }

        [Required(ErrorMessage = "'{0}'是必填项")]
        [StringLength(50, ErrorMessage = "{0}长度不能小于{2}.", MinimumLength = 4)]
        [Display(Name = "权限描述")]
        public string PermissionDescription { get; set; }

    }
}

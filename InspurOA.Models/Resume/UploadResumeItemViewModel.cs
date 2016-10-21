using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{ 
    public class UploadResumeItemViewModel
    {
        [Required]
        [Display(Name = "语言类型")]
        public string languageType { get; set; }

        [Required]
        [Display(Name = "网站来源")]
        public string sourceSite { get; set; }

        [Required]
        [Display(Name = "岗位名称")]
        public string postName { get; set; }

        public List<string> ErrorMessages { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspurOA.Web.Models
{
     public class ProjectViewModel
    {
        public string Id { get; set; }

        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }
    }
}
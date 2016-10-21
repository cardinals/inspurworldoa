using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class ProjectModel
    {
        [Key]
        public string Id { get; set; }

        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }
    }
}

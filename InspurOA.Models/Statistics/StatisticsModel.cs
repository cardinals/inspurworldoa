using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class StatisticsModel
    {
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name ="数量")]
        public int Count { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class StatisticsViewModel
    {
        [Display(Name = "简历总数")]
        public int TotalCount { get; set; }

        public List<StatisticsModel> GroupList { get; set; }
    }
}

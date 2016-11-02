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

        public string ProjectName { get; set; }
    }
}

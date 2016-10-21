using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class ResumeComment
    {
        [Key]
        public string Id { get; set; }

        public string ResumeId { get; set; }

        [Display(Name = "岗位名称")]
        public string PostName { get; set; }

        [Display(Name = "面试日期")]
        public string InterviewDate { get; set; }

        [Display(Name = "面试反馈")]
        public string InterviewFeedBack { get; set; }

        [Display(Name ="反馈日期")]
        public string FeedBackTime { get; set; }

        [Display(Name ="反馈者")]
        public string UserName { get; set; }
    }
}

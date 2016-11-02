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

        public string PostName { get; set; }

        public string InterviewDate { get; set; }

        public string InterviewFeedBack { get; set; }

        public string FeedBackTime { get; set; }

        public string UserName { get; set; }
    }
}

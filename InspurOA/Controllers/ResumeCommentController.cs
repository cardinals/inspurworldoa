using InspurOA.BLL;
using InspurOA.DAL;
using InspurOA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    public class ResumeCommentController : Controller
    {
        ResumeCommentBLL bll = new ResumeCommentBLL();

        // GET: ResumeComment
        public string ListResumeComments(string resumeId)
        {
            if (string.IsNullOrWhiteSpace(resumeId))
            {
                return string.Empty;
            }

            List<ResumeComment> comments = bll.FindResumeCommentsByResumeId(resumeId);
            return JsonConvert.SerializeObject(comments);
        }

        [HttpPost]
        public string Create(string resumeId, string postName, string interviewDate, string interviewFeedback)
        {
            if (string.IsNullOrWhiteSpace(resumeId) ||
                string.IsNullOrWhiteSpace(postName) ||
                string.IsNullOrWhiteSpace(interviewDate) ||
                string.IsNullOrWhiteSpace(interviewFeedback))
            {
                return "{\"result\":false}";
            }

            ResumeComment comment = new ResumeComment();
            comment.Id = Guid.NewGuid().ToString();
            comment.ResumeId = resumeId;
            comment.PostName = postName;
            comment.InterviewDate = interviewDate;
            comment.InterviewFeedBack = interviewFeedback;
            comment.FeedBackTime = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
            comment.UserName = User.Identity.Name;

            bool result = bll.SaveResumeComment(comment);
            if (result)
            {
                return "{\"result\":true}";
            }
            else
            {
                return "{\"result\":false}";
            }
        }

        // GET: ResumeComment/Delete/5
        public string Delete(string[] ids)
        {
            if (ids == null || ids.Length <= 0)
            {
                return "{\"result\":false}";
            }

            foreach (var id in ids)
            {
                var comment = bll.FindResumeComment(id);
                if (comment != null)
                {
                    bll.DeleteResumeComment(comment);
                }
            }

            return "{\"result\":true}";
        }

    }
}

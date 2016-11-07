using InspurOA.DAL;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.BLL
{
    public class ResumeCommentBLL
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public List<ResumeComment> GetResumeCommentList()
        {
            return db.ResumeCommentSet.ToList();
        }

        public bool SaveResumeComment(ResumeComment resumeComment)
        {
            db.ResumeCommentSet.Add(resumeComment);
            var saved = db.SaveChanges();
            return saved > 0;
        }

        public bool DeleteResumeComment(ResumeComment resumeComment)
        {
            db.ResumeCommentSet.Remove(resumeComment);
            var deleted = db.SaveChanges();
            return deleted > 0;
        }

        public ResumeComment FindResumeComment(string id)
        {
            return db.ResumeCommentSet.Find(id);
        }

        public List<ResumeComment> FindResumeCommentsByResumeId(string resumeId)
        {
            return db.ResumeCommentSet.Where(c => c.ResumeId.Equals(resumeId)).ToList();
        }
    }
}

using InspurOA.DAL;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.ModelBinding;

namespace InspurOA.BLL
{
    public class ResumeBLL
    {
        InspurDbContext dal = new InspurDbContext();

        public List<Resume> GetResumeList()
        {
            return dal.ResumeSet.ToList();
        }

        public IQueryable<Resume> QueryResumeByPage(Expression<Func<Resume, bool>> whereSelector, Expression<Func<Resume, string>> KeySelector, out int totalCount, out int pageCount, int offset, int limit = 10)
        {
            totalCount = 0;
            pageCount = 0;
            if (offset < 0)
            {
                throw new ArgumentException("参数不能小于0", "offset");
            }

            if (limit <= 0)
            {
                throw new ArgumentException("参数必须大于0", "limit");
            }

            return dal.ResumeSet.QueryByPage<Resume>(whereSelector, KeySelector, out totalCount, out pageCount, offset, limit);
        }

        public bool SaveResume(Resume resume)
        {
            dal.ResumeSet.Add(resume);
            var saved = dal.SaveChanges();
            return saved > 0;
        }

        public bool UpdateResume(Resume resume)
        {
            dal.Entry<Resume>(resume).State = EntityState.Modified;
            var updated = dal.SaveChanges();
            return updated > 0;
        }

        public bool DeleteResume(Resume resume)
        {
            dal.ResumeSet.Remove(resume);
            var deleted = dal.SaveChanges();
            return deleted > 0;
        }

        public Resume FindResume(string id)
        {
            return dal.ResumeSet.Find(id);
        }
    }
}

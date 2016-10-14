using InspurOA.DAL;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InspurOA.BLL
{
    public class ResumeBLL
    {
        ResumeDbContext dal = new ResumeDbContext();

        public List<Resume> GetResumeList()
        {
            return dal.ResumeSet.ToList();
        }

        public void SaveResume(Resume resume)
        {
            resume.Id = Guid.NewGuid().ToString();
            dal.ResumeSet.Add(resume);
            dal.SaveChanges();
        }
    }
}

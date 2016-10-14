using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspurOA.DAL;
using InspurOA.Models;
using Microsoft.Office.Interop.Word;
using System.IO;
using InspurOA.Common;
using InspurOA.BLL;
using System.Configuration;
using Newtonsoft.Json;

namespace InspurOA.Controllers
{
    public class ResumeController : Controller
    {
        private Application app;
        private Document doc = null;
        private object unknow = Type.Missing;
        private string localResumeFolderPath = ConfigurationManager.AppSettings["LocalResumeFolderPath"].ToString();

        private ResumeDbContext db = new ResumeDbContext();

        // GET: Resume
        public ActionResult Index()
        {
            return View(db.ResumeSet.OrderByDescending(R => R.UploadTime).ToList());
        }

        public ActionResult TemplateDetail()
        {
            return View("Details");
        }

        // GET: Resume/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resume resume = db.ResumeSet.Find(id);
            if (resume == null)
            {
                return HttpNotFound();
            }
            return View(resume);
        }

        // GET: Resume/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: Resume/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Guid,PersonalInformation,CareerObjective,SelfAssessment,WorkExperience,ProjectExperience,Education,Certificates,HonorsandAwards,SchoolPractice,LanguageSkills,Training,ProfessionalSkills,FilePath,UploadTime,ResumeLanguageType,SourceSite")] Resume resume)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ResumeSet.Add(resume);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(resume);
        //}

        public ActionResult UploadResume(string languageType, string sourceSite, string postName)
        {
            if (string.IsNullOrWhiteSpace(sourceSite) || string.IsNullOrWhiteSpace(languageType) || string.IsNullOrWhiteSpace(postName))
            {
                return View("Create");
            }

            app = new ApplicationClass();

            for (int k = 0; k < Request.Files.Count; k++)
            {
                HttpPostedFileBase file = Request.Files.Get(k);

                List<string> content = new List<string>();
                if (file == null || file.ContentLength <= 0)
                {
                    return Content("没有文件！", "text/plain");
                }

                string fileName = string.Format("{0}--{1}", DateTime.Now.ToString("yyyyMMddhhmmss"), Path.GetFileName(file.FileName));
                string filePath = Path.Combine(localResumeFolderPath, fileName);
                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                fs.Close();
                file.SaveAs(filePath);

                Resume resume;
                try
                {
                    object fileobj = (object)filePath;
                    app.Visible = false;
                    doc = app.Documents.Open(
                        ref fileobj, ref unknow, ref unknow, ref unknow, ref unknow,
                            ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                            ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);

                    for (int i = 1; i <= doc.Paragraphs.Count; i++)
                    {
                        if (doc.Paragraphs[i].Range.Text.Equals("\r\a"))
                        {
                            continue;
                        }

                        content.Add(doc.Paragraphs[i].Range.Text.Replace(" ", " ").Replace("/", "").Replace("\v", "\r"));
                    }

                    doc.Close();

                    if (sourceSite.Equals("ZL"))
                    {
                        resume = ResumeHelper.getZLResumeEntity(content);
                    }
                    else
                    {
                        resume = ResumeHelper.GetWYResumeEntity(content);
                    }

                    resume.FilePath = fileName;
                    resume.UploadTime = DateTime.Now;
                    resume.SourceSite = sourceSite;
                    resume.LanguageType = languageType;
                    resume.PostName = postName;
                    ResumeBLL opt = new ResumeBLL();
                    opt.SaveResume(resume);
                }
                catch (Exception e)
                {
                    if (doc != null)
                    {
                        doc.Close();
                    }
                    if (app != null)
                    {
                        app.Quit();
                    }

                    return Content("上传异常！" + e.Message, "text/plain");
                }
            }

            app.Quit();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public String Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            return JsonConvert.SerializeObject(db.ResumeSet.OrderByDescending(R => R.UploadTime).ToList().Where(t => t.PersonalInformation.Contains(query)));
        }

        //// GET: Resume/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Resume resume = db.ResumeSet.Find(id);
        //    if (resume == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(resume);
        //}

        //// POST: Resume/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Guid,PersonalInformation,CareerObjective,SelfAssessment,WorkExperience,ProjectExperience,Education,Certificates,HonorsandAwards,SchoolPractice,LanguageSkills,Training,ProfessionalSkills,FilePath,UploadTime,ResumeLanguageType,SourceSite")] Resume resume)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(resume).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(resume);
        //}

        //// GET: Resume/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Resume resume = db.ResumeSet.Find(id);
        //    if (resume == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(resume);
        //}

        //// POST: Resume/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Resume resume = db.ResumeSet.Find(id);
        //    db.ResumeSet.Remove(resume);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

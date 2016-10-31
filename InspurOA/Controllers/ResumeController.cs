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

        private ResumeBLL bll = new ResumeBLL();
        private ResumeCommentBLL commentBll = new ResumeCommentBLL();
        private ProjectBLL proBll = new ProjectBLL();

        // GET: Resume
        public ActionResult Index(string query, int pageIndex = 0, int limit = 10)
        {
            int totalCount = 0, pageCount = 0;
            IQueryable<Resume> comments = null;
            if (string.IsNullOrWhiteSpace(query))
            {
                comments = bll.QueryResumeByPage(null,
                 R => R.UploadTime.ToString(),
                 out totalCount,
                 out pageCount,
                 pageIndex,
                 limit);
            }
            else
            {
                comments = bll.QueryResumeByPage(r => r.PersonalInformation.Contains(query) || r.WorkExperience.Contains(query) || r.ProjectExperience.Contains(query) || r.Education.Contains(query),
                  R => R.UploadTime.ToString(),
                  out totalCount,
                  out pageCount,
                  pageIndex,
                  limit);
            }

            ViewData["TotalCount"] = totalCount;
            ViewData["PageCount"] = pageCount;
            ViewData["CurrentPageIndex"] = pageIndex;
            ViewData["Limit"] = limit;
            ViewData["query"] = query;

            return View(comments);
        }

        // GET: Resume/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return Redirect("idnex");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resume resume = bll.FindResume(id);
            if (resume == null)
            {
                return HttpNotFound();
            }

            var projects = proBll.GetAllProjects();
            ViewData["Projects"] = projects;

            return View(resume);
        }

        // GET: Resume/Create
        public ActionResult Create()
        {
            var projects = proBll.GetAllProjects();
            ViewData["Projects"] = projects;
            return View();
        }

        public ActionResult UploadResume(string languageType, string sourceSite, string projectName)//, string postName
        {
            if (string.IsNullOrWhiteSpace(sourceSite) || string.IsNullOrWhiteSpace(languageType) || string.IsNullOrWhiteSpace(projectName))// || string.IsNullOrWhiteSpace(postName)
            {
                var projects = proBll.GetAllProjects();
                ViewData["Projects"] = projects;
                return View("Create");
            }

            if (!sourceSite.Equals("ZL") && !sourceSite.Equals("WY"))
            {
                var projects = proBll.GetAllProjects();
                ViewData["Projects"] = projects;
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

                    switch (sourceSite)
                    {
                        case "ZL":
                            resume.SourceSite = "智联招聘";
                            break;
                        case "WY":
                            resume.SourceSite = "前程无忧";
                            break;
                        default:
                            resume.SourceSite = "";
                            break;
                    }

                    resume.Id = Guid.NewGuid().ToString();
                    resume.FilePath = fileName;
                    resume.UploadTime = DateTime.Now;
                    resume.LanguageType = languageType;
                    resume.ProjectName = projectName;
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
        public string ChangeProjectName(string resumeId, string projectName)
        {
            if (string.IsNullOrWhiteSpace(resumeId) || string.IsNullOrWhiteSpace(projectName))
            {
                return "{\"result\":false}";
            }

            var resume = bll.FindResume(resumeId);
            if (resume == null)
            {
                return "{\"result\":false}";
            }

            resume.ProjectName = projectName;
            bll.UpdateResume(resume);

            return "{\"result\":true}";
        }

        [HttpPost]
        public ActionResult SearchResumes(FormCollection colleciton)
        {
            string query = colleciton.Get("query");
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("index");
            }

            int totalCount = 0, pageCount = 0;

            //var comments = bll.GetResumeList()
            //     .OrderByDescending(R => R.UploadTime)
            //     .Where(t => (t.PersonalInformation.Contains(query) || t.WorkExperience.Contains(query) || t.ProjectExperience.Contains(query) || t.Education.Contains(query)));

            var comments = bll.QueryResumeByPage(
                r => r.PersonalInformation.Contains(query) || r.WorkExperience.Contains(query) || r.ProjectExperience.Contains(query) || r.Education.Contains(query),
                R => R.UploadTime.ToString(),
                out totalCount,
                out pageCount,
                0);
            ViewData["query"] = query;
            return View("index", comments);
        }
        // GET: Resume/Delete/5
        public string Delete(string[] ids)
        {
            if (ids == null || ids.Length <= 0)
            {
                return "{ 'result' : false }";
            }

            foreach (var id in ids)
            {
                var resume = bll.FindResume(id);
                if (resume != null)
                {
                    bll.DeleteResume(resume);
                }

                var comments = commentBll.FindResumeCommentsByResumeId(id);
                foreach (var comment in comments)
                {
                    commentBll.DeleteResumeComment(comment);
                }
            }

            return "{ 'result':true }";
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}

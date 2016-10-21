using InspurOA.BLL;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Web.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectBLL bll = new ProjectBLL();

        // GET: Project
        public ActionResult Index(int pageIndex = 0, int limit = 10)
        {
            int totalCount = 0, pageCount = 0;

            var comments = bll.QueryResumeByPage(null,
                         R => R.ProjectName.ToString(),
                         out totalCount,
                         out pageCount,
                         pageIndex,
                         limit);

            ViewData["TotalCount"] = totalCount;
            ViewData["PageCount"] = pageCount;
            ViewData["CurrentPageIndex"] = pageIndex;
            ViewData["Limit"] = limit;

            return View(comments);
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            string projectName = collection.Get("ProjectName");

            if (string.IsNullOrWhiteSpace(projectName))
            {
                return View();
            }

            try
            {
                ProjectModel model = new ProjectModel();
                model.Id = Guid.NewGuid().ToString();
                model.ProjectName = projectName;

                bll.CreateProject(model);
            }
            catch
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        // GET: Project/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("index");
            }

            var model = bll.Find(id);
            if (model != null)
            {
                return View(model);
            }

            return RedirectToAction("index");
        }

        // POST: Project/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Project/Delete/5
        [HttpPost]
        public string Delete(string[] ids)
        {
            if (ids == null || ids.Length <= 0)
            {
                return "{ 'result' : false }";
            }

            foreach (var id in ids)
            {
                var project = bll.Find(id);
                if (project != null)
                {
                    bll.DeleteProject(project);
                }
            }

            return "{ 'result':true }";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    public class SystemManageController : Controller
    {
        // GET: SystemManage
        public ActionResult Index()
        {
            return View();
        }

        // GET: SystemManage/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SystemManage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemManage/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SystemManage/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SystemManage/Edit/5
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

        // GET: SystemManage/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SystemManage/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

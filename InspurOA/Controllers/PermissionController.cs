using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspurOA.Models;
using InspurOA;
using InspurOA.DAL;

namespace InspurOA.Controllers
{
    [InspurAuthotize(Roles = "Admin")]
    public class PermissionController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: Permissions
        public ActionResult Index()
        {
            return View(dbContext.Permissions.ToList());
        }

        // GET: Permissions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = dbContext.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PermissionDescription")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                permission.PermissionId = Guid.NewGuid().ToString();
                dbContext.Permissions.Add(permission);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permission);
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = dbContext.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermissionId,PermissionDescription")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                dbContext.Entry(permission).State = EntityState.Modified;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permission);
        }

        // GET: Permissions/Delete/5
        [HttpPost]
        public ActionResult Delete(string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            foreach (string id in ids)
            {
                using (var transcation = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        Permission permission = dbContext.Permissions.Find(id);
                        if (permission != null)
                        {
                            dbContext.Permissions.Remove(permission);
                        }

                        dbContext.SaveChanges();

                        IQueryable<RolePermission> RolePermissions = dbContext.RolePermissions.Where(t => t.PermissionId == id);
                        foreach (var rp in RolePermissions)
                        {
                            dbContext.RolePermissions.Remove(rp);
                        }

                        dbContext.SaveChanges();

                        IQueryable<UserPermission> UserPermissions = dbContext.UserPermissions.Where(t => t.PermissionId == id);
                        foreach (var up in UserPermissions)
                        {
                            dbContext.UserPermissions.Remove(up);
                        }

                        dbContext.SaveChanges();

                        transcation.Commit();
                    }
                    catch
                    {
                        transcation.Rollback();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        //// POST: Permissions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Permission permission = dbContext.Permissions.Find(id);
        //    dbContext.Permissions.Remove(permission);
        //    dbContext.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

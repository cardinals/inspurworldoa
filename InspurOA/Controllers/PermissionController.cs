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
using InspurOA.Identity.EntityFramework;
using InspurOA.Attributes;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace InspurOA.Controllers
{
    [InspurAuthorize(Roles = "Admin", Permissions = "ControlPermission")]
    public class PermissionController : Controller
    {
        private ApplicationPermissionManager _permissionManager;
        private ApplicationRolePermissionManager _rolePermissionManager;

        public ApplicationPermissionManager PermissionManager
        {
            get
            {
                return _permissionManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationPermissionManager>();
            }
            private set
            {
                _permissionManager = value;
            }
        }

        public ApplicationRolePermissionManager RolePermissionManager
        {
            get
            {
                return _rolePermissionManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRolePermissionManager>();
            }
            private set
            {
                _rolePermissionManager = value;
            }
        }


        // GET: Permissions
        public ActionResult Index()
        {
            return View(PermissionManager.Permissions.ToList());
        }

        // GET: Permissions/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var p = await PermissionManager.FindByIdAsync(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            return View(p);
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var p = new InspurIdentityPermission();
                p.PermissionId = Guid.NewGuid().ToString();
                p.PermissionCode = model.PermissionCode;
                p.PermissionDescription = model.PermissionDescription;

                await PermissionManager.CreateAsync(p);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Permissions/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var p = await PermissionManager.FindByIdAsync(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            PermissionViewModel model = new PermissionViewModel() { PermissionId = p.PermissionId, PermissionCode = p.PermissionCode, PermissionDescription = p.PermissionDescription };
            return View(model);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var p = new InspurIdentityPermission();
                p.PermissionId = model.PermissionId;
                p.PermissionCode = model.PermissionCode;
                p.PermissionDescription = model.PermissionDescription;

                await PermissionManager.UpdateAsync(p);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Permissions/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            foreach (string id in ids)
            {
                var p = await PermissionManager.FindByIdAsync(id);
                if (p != null)
                {
                    await PermissionManager.DeleteAsync(p);
                }

                await RolePermissionManager.RemovePermissionFromRolePermissionAsync(id);
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

            }

            base.Dispose(disposing);
        }
    }
}

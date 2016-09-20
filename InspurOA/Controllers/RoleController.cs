using InspurOA.Authorization;
using InspurOA.DAL;
using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    [InspurAuthorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: Role
        public ActionResult Index()
        {
            return View(dbContext.Roles.ToList());
        }

        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Role/Create
        public ActionResult Create()
        {
            List<PermissionItemViewModel> PermissionList = new List<PermissionItemViewModel>();
            foreach (var item in dbContext.Permissions.ToList())
            {
                PermissionList.Add(new PermissionItemViewModel() { IsChecked = false, Permission = item });
            }

            ViewData["Permissions"] = PermissionList;
            return View();
        }

        [HttpPost]
        public ActionResult Create(RoleViewModel model, FormCollection collection)
        {
            string PermissionStr = collection.Get("Permissions");
            string[] permissionIdArray = { };
            if (!string.IsNullOrWhiteSpace(PermissionStr))
            {
                permissionIdArray = PermissionStr.Split(',');
            }

            List<PermissionItemViewModel> PermissionList = new List<PermissionItemViewModel>();
            foreach (var item in dbContext.Permissions.ToList())
            {
                if (permissionIdArray.Contains(item.PermissionId))
                {
                    PermissionList.Add(new PermissionItemViewModel() { IsChecked = true, Permission = item });
                }
                else
                {
                    PermissionList.Add(new PermissionItemViewModel() { IsChecked = false, Permission = item });
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Permissions"] = PermissionList;
                return View(model);
            }

            string newRoleId = Guid.NewGuid().ToString();
            var role = new InspurIdentityRole() { RoleId = newRoleId, RoleCode = model.RoleCode, RoleName = model.RoleName, RoleDescription = model.RoleDescription };
            using (var transcation = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.Roles.Add(role);
                    dbContext.SaveChanges();

                    foreach (var permissionId in permissionIdArray)
                    {
                        var RP = new InspurIdentityRolePermission();
                        RP.RoleId = role.RoleId;
                        RP.PermissionId = permissionId;

                        dbContext.RolePermissions.Add(RP);
                    }

                    dbContext.SaveChanges();
                    transcation.Commit();
                }
                catch
                {
                    transcation.Rollback();
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Role/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    RoleViewModel model = new RoleViewModel();
        //    model.RoleCode = collection.Get("RoleCode");
        //    model.RoleName = collection.Get("RoleName");
        //    model.RoleDescription = collection.Get("RoleDescription");

        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    string newRoleId = Guid.NewGuid().ToString();
        //    Role role = new Role() { RoleId = newRoleId, RoleCode = model.RoleCode, RoleName = model.RoleName, RoleDescription = model.RoleDescription };
        //    string permissionStr = collection.Get("Permission");
        //    if (!string.IsNullOrWhiteSpace(permissionStr))
        //    {
        //        using (var transcation = dbContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                string[] permissionIdArray = permissionStr.Split(',');
        //                dbContext.Roles.Add(role);
        //                dbContext.SaveChanges();

        //                foreach (var permissionId in permissionIdArray)
        //                {
        //                    RolePermission RP = new RolePermission();
        //                    RP.RoleId = role.RoleId;
        //                    RP.PermissionId = permissionId;

        //                    dbContext.RolePermissions.Add(RP);
        //                }

        //                dbContext.SaveChanges();
        //                transcation.Commit();
        //            }
        //            catch
        //            {
        //                transcation.Rollback();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        dbContext.Roles.Add(role);
        //        dbContext.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}

        // GET: Role/Edit/5

        public ActionResult Edit(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return RedirectToAction("Index");
                }

                var roleViewModel = new RoleViewModel();
                var Role = dbContext.Roles.Find(id);
                roleViewModel.RoleId = id;
                roleViewModel.RoleCode = Role.RoleCode;
                roleViewModel.RoleName = Role.RoleName;
                roleViewModel.RoleDescription = Role.RoleDescription;
                List<PermissionItemViewModel> permissionList = new List<PermissionItemViewModel>();

                IQueryable<InspurIdentityRolePermission> rolePermissions = dbContext.RolePermissions.Where(t => t.RoleId == id);
                foreach (var item in dbContext.Permissions.ToList())
                {
                    if (rolePermissions.Any(t => t.PermissionId == item.PermissionId))
                    {
                        permissionList.Add(new PermissionItemViewModel() { Permission = item, IsChecked = true });
                    }
                    else
                    {
                        permissionList.Add(new PermissionItemViewModel() { Permission = item, IsChecked = false });
                    }
                }

                ViewData["Permissions"] = permissionList;
                return View(roleViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Role/Edit
        [HttpPost]
        public ActionResult Edit(RoleViewModel model, FormCollection collection)
        {
            var Role = dbContext.Roles.FirstOrDefault(t => t.RoleId == model.RoleId);
            if (Role == null)
            {
                return RedirectToAction("Index");
            }

            string PermissionStr = collection.Get("Permissions");
            string[] permissionIdArray = { };
            if (!string.IsNullOrWhiteSpace(PermissionStr))
            {
                permissionIdArray = PermissionStr.Split(',');
            }

            List<PermissionItemViewModel> PermissionList = new List<PermissionItemViewModel>();
            foreach (var item in dbContext.Permissions.ToList())
            {
                if (permissionIdArray.Contains(item.PermissionId))
                {
                    PermissionList.Add(new PermissionItemViewModel() { IsChecked = true, Permission = item });
                }
                else
                {
                    PermissionList.Add(new PermissionItemViewModel() { IsChecked = false, Permission = item });
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Permissions"] = PermissionList;
                return View(model);
            }

            using (var transcation = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Role.RoleCode = model.RoleCode;
                    Role.RoleName = model.RoleName;
                    Role.RoleDescription = model.RoleDescription;
                    dbContext.Entry(Role).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    var RolePermissions = dbContext.RolePermissions.Where(t => t.RoleId == model.RoleId);
                    foreach (var RP in RolePermissions)
                    {
                        dbContext.RolePermissions.Remove(RP);
                    }

                    dbContext.SaveChanges();

                    foreach (var permissionId in permissionIdArray)
                    {
                        var RP = new InspurIdentityRolePermission();
                        RP.RoleId = Role.RoleId;
                        RP.PermissionId = permissionId;

                        dbContext.RolePermissions.Add(RP);
                    }

                    dbContext.SaveChanges();
                    transcation.Commit();
                }
                catch
                {
                    transcation.Rollback();
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Role/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Role/Delete/5
        [HttpPost]
        public ActionResult Delete(string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return RedirectToAction("Index");
                }

                foreach (string id in ids)
                {
                    using (var transcation = dbContext.Database.BeginTransaction())
                    {
                        var role = dbContext.Roles.Find(id);
                        dbContext.Roles.Remove(role);
                        dbContext.SaveChanges();

                        var RolePermissions = dbContext.RolePermissions.Where(t => t.RoleId == id);
                        foreach (var RP in RolePermissions)
                        {
                            dbContext.RolePermissions.Remove(RP);
                        }

                        dbContext.SaveChanges();
                        transcation.Commit();
                    }
                }
            }
            catch
            {
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}

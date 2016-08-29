using InspurOA.DAL;
using InspurOA.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    [InspurAuthotize(Roles = "Admin")]
    public class RoleController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: Role
        public ActionResult Index()
        {
            return View(dbContext.URoles.ToList());
        }

        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Role/Create
        public ActionResult Create()
        {
            RoleViewModel model = new RoleViewModel();
            model.PermissionViewModelList = new List<PermissionItemViewModel>();
            foreach (var item in dbContext.Permissions.ToList())
            {
                model.PermissionViewModelList.Add(new PermissionItemViewModel() { Permission = item, IsChecked = false });
            }

            return View(model);
        }

        // POST: Role/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            RoleViewModel model = new RoleViewModel();
            model.Name = collection.Get("Name");
            model.RoleName = collection.Get("RoleName");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string newRoleId = Guid.NewGuid().ToString();
            URole role = new URole() { Id = newRoleId, Name = model.Name, RoleName = model.RoleName };
            string permissionStr = collection.Get("Permission");
            if (!string.IsNullOrWhiteSpace(permissionStr))
            {
                using (var transcation = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        string[] permissions = permissionStr.Split(',');
                        dbContext.URoles.Add(role);
                        dbContext.SaveChanges();

                        foreach (var permission in permissions)
                        {
                            RolePermission RP = new RolePermission();
                            RP.RoleId = role.Id;
                            RP.PermissionId = permission;

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
            }
            else
            {
                dbContext.URoles.Add(role);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

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
                var Role = dbContext.URoles.Find(id);
                roleViewModel.Id = id;
                roleViewModel.Name = Role.Name;
                roleViewModel.RoleName = Role.RoleName;
                roleViewModel.PermissionViewModelList = new List<PermissionItemViewModel>();

                IQueryable<RolePermission> rolePermissions = dbContext.RolePermissions.Where(t => t.RoleId == id);
                foreach (var item in dbContext.Permissions.ToList())
                {
                    if (rolePermissions.Any(t => t.PermissionId == item.PermissionId))
                    {
                        roleViewModel.PermissionViewModelList.Add(new PermissionItemViewModel() { Permission = item, IsChecked = true });
                    }
                    else
                    {
                        roleViewModel.PermissionViewModelList.Add(new PermissionItemViewModel() { Permission = item, IsChecked = false });
                    }
                }

                return View(roleViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Role/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {

                string roleName = collection.Get("RoleName");
                var Role = dbContext.URoles.Find(id);

                if (Role == null)
                {
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return View(Role);
                }

                string permissionStr = collection.Get("Permission");
                if (!string.IsNullOrWhiteSpace(permissionStr))
                {
                    using (var transcation = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            string[] permissions = permissionStr.Split(',');

                            Role.RoleName = roleName;
                            dbContext.SaveChanges();

                            var RolePermissions = dbContext.RolePermissions.Where(t => t.RoleId == id);
                            foreach (var RP in RolePermissions)
                            {
                                dbContext.RolePermissions.Remove(RP);
                            }

                            dbContext.SaveChanges();

                            foreach (var permission in permissions)
                            {
                                RolePermission RP = new RolePermission();
                                RP.RoleId = Role.Id;
                                RP.PermissionId = permission;

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
                }
                else
                {
                    Role.RoleName = roleName;
                    dbContext.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
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
                        var role = dbContext.URoles.Find(id);
                        dbContext.URoles.Remove(role);
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

           return  RedirectToAction("Index");
        }
    }
}

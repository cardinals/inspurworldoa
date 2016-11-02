using InspurOA.Attributes;
using InspurOA.BLL;
using InspurOA.DAL;
using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using InspurOA.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    [InspurAuthorize(Roles = "Admin")]//, Permissions = "ControlRole"
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationPermissionManager _permissionManager;
        private ApplicationRolePermissionManager _rolePermissionManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }

            private set
            {
                _roleManager = value;
            }
        }

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

        // GET: Role
        public ActionResult Index(int pageIndex = 0, int limit = 10)
        {
            int totalCount;
            int pageCount;

            var list = RoleManager.Roles.QueryByPage(t => t.RoleName, out totalCount, out pageCount, pageIndex, limit).ToList();
            ViewData["TotalCount"] = totalCount;
            ViewData["PageCount"] = pageCount;
            ViewData["CurrentPageIndex"] = pageIndex;
            ViewData["Limit"] = limit;
            return View(list);
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
            foreach (var item in PermissionManager.Permissions.ToList())
            {
                PermissionList.Add(new PermissionItemViewModel() { IsChecked = false, Permission = item });
            }

            ViewData["Permissions"] = PermissionList;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model, FormCollection collection)
        {
            string PermissionStr = collection.Get("Permissions");
            string[] permissionIdArray = { };
            if (!string.IsNullOrWhiteSpace(PermissionStr))
            {
                permissionIdArray = PermissionStr.Split(',');
            }

            List<PermissionItemViewModel> PermissionList = new List<PermissionItemViewModel>();
            foreach (var item in PermissionManager.Permissions.ToList())
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
            var result = await RoleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                AddErrors(result);
                ViewData["Permissions"] = PermissionList;
                return View(model);
            }

            await RolePermissionManager.AddPermissionToRoleAsync(permissionIdArray, role.RoleId);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return RedirectToAction("Index");
                }

                var roleViewModel = new RoleViewModel();
                var Role = await RoleManager.FindByIdAsync(id);
                roleViewModel.RoleId = id;
                roleViewModel.RoleCode = Role.RoleCode;
                roleViewModel.RoleName = Role.RoleName;
                roleViewModel.RoleDescription = Role.RoleDescription;
                List<PermissionItemViewModel> permissionList = new List<PermissionItemViewModel>();

                IQueryable<InspurIdentityRolePermission> rolePermissions = RolePermissionManager.RolePermissions.Where(t => t.RoleId == id);
                foreach (var item in PermissionManager.Permissions.ToList())
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
        public async Task<ActionResult> Edit(RoleViewModel model, FormCollection collection)
        {
            var Role = await RoleManager.FindByIdAsync(model.RoleId);
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
            foreach (var item in PermissionManager.Permissions.ToList())
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

            Role.RoleCode = model.RoleCode;
            Role.RoleName = model.RoleName;
            Role.RoleDescription = model.RoleDescription;
            var result = await RoleManager.UpdateAsync(Role);
            if (!result.Succeeded)
            {
                AddErrors(result);
                ViewData["Permissions"] = PermissionList;
                return View(model);
            }

            await RolePermissionManager.RemoveRoleFromRolePermissionAsync(model.RoleId);
            await RolePermissionManager.AddPermissionToRoleAsync(permissionIdArray, model.RoleId);

            return RedirectToAction("Index");
        }

        // POST: Role/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return RedirectToAction("Index");
                }

                foreach (string id in ids)
                {
                    var role = await RoleManager.FindByIdAsync(id);
                    if (role != null)
                    {
                        await RoleManager.DeleteAsync(role);
                    }

                    await RolePermissionManager.RemoveRoleFromRolePermissionAsync(id);
                }
            }
            catch
            {
                return View();
            }

            return RedirectToAction("Index");
        }
        
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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

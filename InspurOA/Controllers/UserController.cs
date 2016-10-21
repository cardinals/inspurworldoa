using InspurOA.Attributes;
using InspurOA.Common;
using InspurOA.DAL;
using InspurOA.Identity.Core;
using InspurOA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InspurOA.Identity.Owin.Extensions;
using InspurOA.BLL;

namespace InspurOA.Controllers
{
    [InspurAuthorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationUserRoleManager _userRoleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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

        public ApplicationUserRoleManager UserRoleManager
        {
            get
            {
                return _userRoleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserRoleManager>();
            }
            private set
            {
                _userRoleManager = value;
            }
        }

        // GET: UserController
        public ActionResult Index(int pageIndex = 0, int limit = 10)
        {
            int totalCount;
            int pageCount;

            var list = UserManager.Users.QueryByPage(u => u.UserName, out totalCount, out pageCount, pageIndex, limit).ToList();
            ViewData["TotalCount"] = totalCount;
            ViewData["PageCount"] = pageCount;
            ViewData["CurrentPageIndex"] = pageIndex;
            ViewData["Limit"] = limit;
            return View(list);
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("index");
            }

            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                userDetailViewModel.Id = id;
                userDetailViewModel.UserName = user.UserName;
                userDetailViewModel.Email = user.Email;
                userDetailViewModel.PhoneNumber = user.PhoneNumber;
            }

            userDetailViewModel.RoleCode = RoleHelper.GetRoleNameByUserId(id);

            return View(userDetailViewModel);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            List<SelectListItem> Roles = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Selected = false;
                selectListItem.Text = role.RoleName;
                selectListItem.Value = role.RoleCode;
                Roles.Add(selectListItem);
            }

            ViewData["Roles"] = Roles;

            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<ActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (RoleManager.Roles.Any(t => t.RoleCode.Equals(model.RoleCode)))
                {
                    var user = new InspurUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        InspurUser TUser = UserManager.Users.First(t => t.UserName.Equals(model.UserName));
                        if (TUser != null)
                        {
                            await UserRoleManager.AddToRoleAsync(TUser.Id, model.RoleCode);
                        }

                        return RedirectToAction("Index", "User");
                    }

                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("", "所选角色不存在！");
                }
            }

            List<SelectListItem> Roles = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();

                if (role.RoleCode.Equals(model.RoleCode))
                {
                    selectListItem.Selected = true;
                }
                else
                {
                    selectListItem.Selected = false;
                }

                selectListItem.Text = role.RoleName;
                selectListItem.Value = role.RoleCode;
                Roles.Add(selectListItem);
            }

            ViewData["Roles"] = Roles;
            return View(model);
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("index");
            }

            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);
            string RoleCode = RoleHelper.GetRoleCodeByUserId(id);

            List<SelectListItem> UserRoles = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = role.RoleName;
                selectListItem.Value = role.RoleCode;
                if (role.RoleCode.Equals(RoleCode))
                {
                    selectListItem.Selected = true;
                }
                else
                {
                    selectListItem.Selected = false;
                }

                UserRoles.Add(selectListItem);
            }

            ViewData["Roles"] = UserRoles;
            UserCreateViewModel model = new UserCreateViewModel() { UserId = user.Id, UserName = user.UserName, Email = user.Email };

            return View(model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await UserManager.FindByIdAsync(id);

                if (user != null)
                {
                    user.UserName = collection.Get("UserName");
                    user.Email = collection.Get("Email");

                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await UserRoleManager.AddToRoleAsync(id, collection.Get("RoleCode"));
                    }
                }
            }

            return RedirectToAction("Index", "User");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        public async Task Delete(string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return;
            }

            for (int i = 0; i < ids.Length; i++)
            {
                var user = await UserManager.FindByIdAsync(ids[i]);
                if (user != null)
                {
                    await UserManager.DeleteAsync(user);
                }

                await UserRoleManager.RemoveFromRoleInfoAsync(ids[i]);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}

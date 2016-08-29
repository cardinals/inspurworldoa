using InspurOA.Common;
using InspurOA.DAL;
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

namespace InspurOA.Controllers
{
    [InspurAuthotize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();

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

        // GET: UserController
        public ActionResult Index()
        {
            return View(dbContext.Users.ToList());
        }

        // GET: UserController/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("index");
            }

            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();
            var user = dbContext.Users.Find(id);
            if (user != null)
            {
                userDetailViewModel.Id = id;
                userDetailViewModel.UserName = user.UserName;
                userDetailViewModel.Email = user.Email;
            }

            userDetailViewModel.RoleName = RoleHelper.GetRoleNameByUserId(id);

            return View(userDetailViewModel);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            List<SelectListItem> UserRoles = new List<SelectListItem>();
            foreach (var UserRole in dbContext.URoles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Selected = false;
                selectListItem.Text = UserRole.RoleName;
                selectListItem.Value = UserRole.Name;
                UserRoles.Add(selectListItem);
            }

            ViewData["Roles"] = UserRoles;



            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<ActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Roles.Any(t => t.Name == model.RoleName))
                {
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        ApplicationUser TUser = dbContext.Users.First(t => t.UserName == model.UserName);
                        if (TUser != null)
                        {
                            await UserManager.AddToRoleAsync(TUser.Id, model.RoleName);
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

            List<SelectListItem> UserRoles = new List<SelectListItem>();
            foreach (var UserRole in dbContext.URoles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();

                if (UserRole.Name.Equals(model.RoleName))
                {
                    selectListItem.Selected = true;
                }
                else
                {
                    selectListItem.Selected = false;
                }

                selectListItem.Text = UserRole.RoleName;
                selectListItem.Value = UserRole.Name;
                UserRoles.Add(selectListItem);
            }

            ViewData["Roles"] = UserRoles;
            return View(model);
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("index");
            }

            var user = dbContext.Users.Find(id);
            string RoleName = RoleHelper.GetRoleNameByUserId(id);

            List<SelectListItem> UserRoles = new List<SelectListItem>();
            foreach (var UserRole in dbContext.URoles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = UserRole.RoleName;
                selectListItem.Value = UserRole.Name;
                if (UserRole.Name.Equals(RoleName))
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

            return View(user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
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

        // POST: UserController/Delete/5
        [HttpPost]
        public void Delete(string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return;
                }

                for (int i = 0; i < ids.Length; i++)
                {
                    var user = dbContext.Users.Find(ids[i]);
                    if (user != null)
                    {
                        dbContext.Users.Remove(user);
                    }
                }

                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
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

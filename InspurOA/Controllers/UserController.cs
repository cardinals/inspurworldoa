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

            var user = dbContext.Users.Find(id);

            return View(user);
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
                selectListItem.Value = UserRole.Id;
                UserRoles.Add(selectListItem);
            }

            ViewData["Roles"] = UserRoles;

            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userID = Guid.NewGuid().ToString();
                var user = new ApplicationUser {Id= userID, UserName = model.UserName, Email = model.Email };

                IdentityRole Role = dbContext.Roles.Find(model.RoleId);
                var result = await UserManager.CreateAsync(user, model.Password);
                await UserManager.AddToRoleAsync(userID, Role.Name);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }

                AddErrors(result);
            }

            List<SelectListItem> UserRoles = new List<SelectListItem>();
            foreach (var UserRole in dbContext.URoles.ToList())
            {
                SelectListItem selectListItem = new SelectListItem();

                if (UserRole.Id.Equals(model.RoleId))
                {
                    selectListItem.Selected = true;
                }
                else
                {
                    selectListItem.Selected = false;
                }

                selectListItem.Text = UserRole.RoleName;
                selectListItem.Value = UserRole.Id;
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
        public void Delete(string ids)
        {
            try
            {
                //if (ids == null || ids.Length == 0)
                //{
                //    return;
                //    /*return RedirectToAction("Index");*/
                //}

                if (string.IsNullOrWhiteSpace(ids))
                {
                    return;
                }

                string[] idArray = ids.Split(',');

                for (int i = 0; i < idArray.Length; i++)
                {
                    var user = dbContext.Users.Find(idArray[i]);
                    if (user != null)
                    {
                        dbContext.Users.Remove(user);
                    }
                }

                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                //RedirectToAction("index");
            }

            //return View("Index", dbContext.Users.ToList());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspurOA.Models;
using InspurOA.DAL;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InspurOA.Common
{
    public class RoleHelper
    {
        static ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public static URole GetRoleByUserId(string UserId)
        {
            IdentityUserRole userRole = dbContext.UserRoles.FirstOrDefault(t => t.UserId == UserId);
            if (userRole != null)
            {
                return dbContext.URoles.First(t => t.Id == userRole.RoleId);
            }
            else
            {
                return null;
            }
        }

        public static string GetRoleNameByUserId(string UserId)
        {
            URole uRole = GetRoleByUserId(UserId);
            if (uRole != null)
            {
                return uRole.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<SelectListViewModel>
    }
}

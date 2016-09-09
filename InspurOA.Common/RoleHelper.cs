using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspurOA.Models;
using InspurOA.DAL;
using InspurOA.Identity.EntityFramework;

namespace InspurOA.Common
{
    public class RoleHelper
    {
        static ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public static InspurIdentityRole GetRoleByUserId(string UserId)
        {
            var userRole = dbContext.UserRoles.FirstOrDefault(t => t.UserId == UserId);
            if (userRole != null)
            {
                return dbContext.Roles.First(t => t.RoleId == userRole.RoleId);
            }
            else
            {
                return null;
            }
        }
        public static string GetRoleIdByUserId(string UserId)
        {
            var role = GetRoleByUserId(UserId);
            if (role != null)
            {
                return role.RoleId;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetRoleCodeByUserId(string UserId)
        {
            var uRole = GetRoleByUserId(UserId);
            if (uRole != null)
            {
                return uRole.RoleCode;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetRoleNameByUserId(string UserId)
        {
            var uRole = GetRoleByUserId(UserId);
            if (uRole != null)
            {
                return uRole.RoleName;
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool isRole(string UserId, string RoleCode)
        {
            var role = dbContext.Roles.FirstOrDefault(t => t.RoleCode == RoleCode);
            if (role != null)
            {
                return isUserInRole(UserId, role.RoleId);
            }

            return false;
        }

        public static bool isUserInRole(string UserId, string RoleId)
        {
            var UserRole = dbContext.UserRoles.FirstOrDefault(t => t.UserId == UserId && t.RoleId == RoleId);
            if (UserRole == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool AddUserToRole(InspurUser user, string RoleCode)
        {
            var role = dbContext.Roles.FirstOrDefault(t => t.RoleCode == RoleCode);
            if (role == null)
            {
                return false;
            }
            else
            {

            }



            return false;
        }

    }
}

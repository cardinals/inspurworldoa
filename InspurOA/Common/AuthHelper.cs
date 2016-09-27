using InspurOA.DAL;
using InspurOA.Identity.Core;
using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspurOA.Common
{
    public class AuthHelper
    {
        private static ApplicationDbContext dbContext = new ApplicationDbContext();
        private static InspurUserStore<InspurUser> userStore = new InspurUserStore<InspurUser>(dbContext);
        private static InspurUserRoleStore<InspurUser> userRoleStore = new InspurUserRoleStore<InspurUser>(dbContext);
        private static InspurRoleStore roleStore = new InspurRoleStore(dbContext);
        private static InspurRolePermissionStore<InspurUser> rolePermissionStore = new InspurRolePermissionStore<InspurUser>(dbContext);

        private static InspurUserRoleManager<InspurUser, InspurIdentityRole, InspurIdentityUserRole> userRoleManager
            = new InspurUserRoleManager<InspurUser, InspurIdentityRole, InspurIdentityUserRole>(userStore, roleStore, userRoleStore);

        private static InspurRolePermissionManager<InspurUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission> rolePermissionManager
            = new InspurRolePermissionManager<InspurUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission>(roleStore, rolePermissionStore);

        public static bool IsAuthorizedByRole(string userName, string roleCode)
        {
            return userRoleManager.IsAuthorizedByRolesAsync(userName, new string[] { roleCode }).Result;
        }

        public static bool IsAuthorizedByRoles(string userName, string[] roleCodes)
        {
            return userRoleManager.IsAuthorizedByRolesAsync(userName, roleCodes).Result;
        }

        public static bool IsAuthorizedByPermission(string userName, string permissionCode)
        {
            return rolePermissionManager.IsAuthorizedByPermissionAsync(userName, permissionCode).Result;
        }

        public static bool IsAuthorizedByPermissions(string userName, string[] permissionCodes)
        {
            return rolePermissionManager.IsAuthorizedByPermissionsAsync(userName, permissionCodes).Result;
        }
    }
}
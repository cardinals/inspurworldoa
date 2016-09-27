using InspurOA.DAL;
using InspurOA.Identity.Core;
using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Common
{
    public class InspurAuthorizeHelper
    {
        private static InspurUserRoleManager<InspurUser, InspurIdentityRole, InspurIdentityUserRole> userRoleManager =
            new InspurUserRoleManager<InspurUser, InspurIdentityRole, InspurIdentityUserRole>(
                new InspurUserStore<InspurUser>(),
                new InspurRoleStore(),
                new InspurUserRoleStore<InspurUser>()
            );

        private static InspurRolePermissionManager<InspurUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission> rolePermissionManager =
            new InspurRolePermissionManager<InspurUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission>(
                new InspurRoleStore(),
                new InspurRolePermissionStore<InspurUser>()
                );

        public static bool IsAuthorizedByRole(string userName, string roleCode)
        {
            return userRoleManager.IsAuthorizedByRoleAsync(userName, roleCode).Result;
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

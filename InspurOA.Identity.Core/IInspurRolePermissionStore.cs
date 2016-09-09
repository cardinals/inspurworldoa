using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IRolePermissionStore<TUser, TPermission, in TKey>:
        IUserStore<TUser,TKey>
        where TPermission : class, IPermission<TKey>
        where TUser : class, IUser<TKey>        
    {
        Task AddPermissionToRoleAsync(IList<TPermission> permissionList, string roleName);

        Task RemovePermissionsOfRoleAsync(string roleName);

        Task<IList<TPermission>> GetPermissionAsync(string roleName);

        Task<bool> HasPermission(string roleName, string permissionCode);

        Task<bool> HasPermission(TUser user, string permissionCode);
    }
}

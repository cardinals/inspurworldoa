using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurRolePermissionStore<TUser, TPermission, in TKey>:
        IInspurUserStore<TUser,TKey>
        where TPermission : class, IPermission<TKey>
        where TUser : class, IInspurUser<TKey>        
    {
        Task AddPermissionToRoleAsync(IList<TPermission> permissionList, string roleName);

        Task RemovePermissionsOfRoleAsync(string roleName);

        Task<IList<TPermission>> GetPermissionAsync(string roleName);

        Task<bool> HasPermission(string roleName, string permissionCode);

        Task<bool> HasPermission(TUser user, string permissionCode);
    }
}

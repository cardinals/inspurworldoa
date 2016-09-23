using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurQueryableRolePermissionStore<TRole, TPermission, TRolePermission, in TKey> : IInspurRolePermissionStore<TRole, TPermission, TRolePermission, TKey>
        where TRole : class, IInspurRole<TKey>
        where TPermission : class, IInspurPermission<TKey>
        where TRolePermission : class, IInspurRolePermission<TKey>
    {
        IQueryable<TRolePermission> RolePermissions { get; }
    }
}

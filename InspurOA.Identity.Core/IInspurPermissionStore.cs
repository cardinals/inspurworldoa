using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IPermissionStore<TPermission, in TKey> : 
        IDisposable 
        where TPermission : class, IPermission<TKey>
    {
        Task CreateAsync(TPermission permission);

        Task UpdateAsync(TPermission permission);

        Task DeleteAsync(TPermission permission);

        Task<TPermission> FindByIdAsync(TKey permissionId);

        Task<TPermission> FindByCodeAsync(string permissionCode);
    }
}

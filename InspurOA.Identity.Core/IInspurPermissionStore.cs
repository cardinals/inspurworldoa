using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurPermissionStore<TPermission> : IInspurPermissionStore<TPermission, string>
        where TPermission : class, IInspurPermission<string>
    {

    }

    public interface IInspurPermissionStore<TPermission, in TKey> : IDisposable
        where TPermission : class, IInspurPermission<TKey>
    {
        Task CreateAsync(TPermission permission);

        Task UpdateAsync(TPermission permission);

        Task DeleteAsync(TPermission permission);

        Task<TPermission> FindByIdAsync(TKey permissionId);

        Task<TPermission> FindByCodeAsync(string permissionCode);
    }
}

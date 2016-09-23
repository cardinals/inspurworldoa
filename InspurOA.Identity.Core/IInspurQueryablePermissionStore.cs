using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurQueryablePermissionStore<TPermission> : IInspurQueryablePermissionStore<TPermission, string>
        where TPermission : class, IInspurPermission<string>
    {
    }

    public interface IInspurQueryablePermissionStore<TPermission, in Tkey> : IInspurPermissionStore<TPermission, Tkey>
        where TPermission : class, IInspurPermission<Tkey>
    {
        IQueryable<TPermission> Permissions { get; }
    }
}

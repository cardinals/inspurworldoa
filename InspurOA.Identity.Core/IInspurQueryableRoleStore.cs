using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
   /// <summary>
   ///     Interface that exposes an IQueryable roles
   /// </summary>
   /// <typeparam name="TRole"></typeparam>
    public interface IInspurQueryableRoleStore<TRole> : IInspurQueryableRoleStore<TRole, string> where TRole : IInspurRole<string>
    {
    }

    /// <summary>
    ///     Interface that exposes an IQueryable roles
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurQueryableRoleStore<TRole, in TKey> : IInspurRoleStore<TRole, TKey> where TRole : IInspurRole<TKey>
    {
        /// <summary>
        ///     IQueryable Roles
        /// </summary>
        IQueryable<TRole> Roles { get; }
    }
}

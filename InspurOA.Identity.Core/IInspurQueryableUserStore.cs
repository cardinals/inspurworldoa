using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface that exposes an IQueryable users
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IInspurQueryableUserStore<TUser> : IInspurQueryableUserStore<TUser, string> where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Interface that exposes an IQueryable users
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurQueryableUserStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     IQueryable users
        /// </summary>
        IQueryable<TUser> Users { get; }
    }
}

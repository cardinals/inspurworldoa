using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface that exposes basic user management apis
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IInspurUserStore<TUser> : IInspurUserStore<TUser, string>
        where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Interface that exposes basic user management apis
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserStore<TUser, in TKey> : IDisposable 
        where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Insert a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateAsync(TUser user);

        /// <summary>
        ///     Update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateAsync(TUser user);

        /// <summary>
        ///     Delete a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task DeleteAsync(TUser user);

        /// <summary>
        ///     Finds a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<TUser> FindByIdAsync(TKey userId);

        /// <summary>
        ///     Find a user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<TUser> FindByNameAsync(string userName);

    }
}

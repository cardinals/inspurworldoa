using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface that maps users to login providers, i.e. Google, Facebook, Twitter, Microsoft
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IInspurUserLoginStore<TUser> : IInspurUserLoginStore<TUser, string> where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Interface that maps users to login providers, i.e. Google, Facebook, Twitter, Microsoft
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserLoginStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Adds a user login with the specified provider and key
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        Task AddLoginAsync(TUser user, InspurUserLoginInfo login);

        /// <summary>
        ///     Removes the user login with the specified combination if it exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        Task RemoveLoginAsync(TUser user, InspurUserLoginInfo login);

        /// <summary>
        ///     Returns the linked accounts for this user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<InspurUserLoginInfo>> GetLoginsAsync(TUser user);

        /// <summary>
        ///     Returns the user associated with this login
        /// </summary>
        /// <returns></returns>
        Task<TUser> FindAsync(InspurUserLoginInfo login);
    }
}

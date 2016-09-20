using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
        /// <summary>
        ///     Stores whether two factor authentication is enabled for a user
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserTwoFactorStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Sets whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        Task SetTwoFactorEnabledAsync(TUser user, bool enabled);

        /// <summary>
        ///     Returns whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> GetTwoFactorEnabledAsync(TUser user);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
      /// <summary>
      ///     Stores a user's security stamp
      /// </summary>
      /// <typeparam name="TUser"></typeparam>
    public interface IInspurUserSecurityStampStore<TUser> : IInspurUserSecurityStampStore<TUser, string>
        where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Stores a user's security stamp
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserSecurityStampStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Set the security stamp for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        Task SetSecurityStampAsync(TUser user, string stamp);

        /// <summary>
        ///     Get the user security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GetSecurityStampAsync(TUser user);
    }
}

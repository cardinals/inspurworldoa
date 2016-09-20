using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
     /// <summary>
     ///     Stores user specific claims
     /// </summary>
     /// <typeparam name="TUser"></typeparam>
    public interface IInspurUserClaimStore<TUser> : IInspurUserClaimStore<TUser, string> where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Stores user specific claims
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserClaimStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Returns the claims for the user with the issuer set
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<Claim>> GetClaimsAsync(TUser user);

        /// <summary>
        ///     Add a new user claim
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        Task AddClaimAsync(TUser user, Claim claim);

        /// <summary>
        ///     Remove a user claim
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        Task RemoveClaimAsync(TUser user, Claim claim);
    }
}

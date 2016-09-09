using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface to generate user tokens
    /// </summary>
    public interface IInspurUserTokenProvider<TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Generate a token for a user with a specific purpose
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GenerateAsync(string purpose, InspurUserManager<TUser, TKey> manager, TUser user);

        /// <summary>
        ///     Validate a token for a user with a specific purpose
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string purpose, string token, InspurUserManager<TUser, TKey> manager, TUser user);

        /// <summary>
        ///     Notifies the user that a token has been generated, for example an email or sms could be sent, or 
        ///     this can be a no-op
        /// </summary>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task NotifyAsync(string token, InspurUserManager<TUser, TKey> manager, TUser user);

        /// <summary>
        ///     Returns true if provider can be used for this user, i.e. could require a user to have an email
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> IsValidProviderForUserAsync(InspurUserManager<TUser, TKey> manager, TUser user);
    }
}

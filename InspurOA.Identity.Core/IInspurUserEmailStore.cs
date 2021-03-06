﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
     /// <summary>
     ///     Stores a user's email
     /// </summary>
     /// <typeparam name="TUser"></typeparam>
    public interface IInspurUserEmailStore<TUser> : IInspurUserEmailStore<TUser, string> where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     Stores a user's email
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurUserEmailStore<TUser, in TKey> : IInspurUserStore<TUser, TKey> where TUser : class, IInspurUser<TKey>
    {
        /// <summary>
        ///     Set the user email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task SetEmailAsync(TUser user, string email);

        /// <summary>
        ///     Get the user email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GetEmailAsync(TUser user);

        /// <summary>
        ///     Returns true if the user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> GetEmailConfirmedAsync(TUser user);

        /// <summary>
        ///     Sets whether the user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        Task SetEmailConfirmedAsync(TUser user, bool confirmed);

        /// <summary>
        ///     Returns the user associated with this email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<TUser> FindByEmailAsync(string email);
    }
}

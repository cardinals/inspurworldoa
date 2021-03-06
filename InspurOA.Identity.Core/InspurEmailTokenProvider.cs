﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     TokenProvider that generates tokens from the user's security stamp and notifies a user via their email
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class InspurEmailTokenProvider<TUser> : InspurEmailTokenProvider<TUser, string> where TUser : class, IInspurUser<string>
    {
    }

    /// <summary>
    ///     TokenProvider that generates tokens from the user's security stamp and notifies a user via their email
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class InspurEmailTokenProvider<TUser, TKey> : InspurTotpSecurityStampBasedTokenProvider<TUser, TKey>
        where TUser : class, IInspurUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private string _body;
        private string _subject;

        /// <summary>
        ///     Email subject used when a token notification is received
        /// </summary>
        public string Subject
        {
            get { return _subject ?? string.Empty; }
            set { _subject = value; }
        }

        /// <summary>
        ///     Email body which should contain a formatted string which the token will be the only argument
        /// </summary>
        public string BodyFormat
        {
            get { return _body ?? "{0}"; }
            set { _body = value; }
        }

        /// <summary>
        ///     True if the user has an email set
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public override async Task<bool> IsValidProviderForUserAsync(InspurUserManager<TUser, TKey> manager, TUser user)
        {
            var email = await manager.GetEmailAsync(user.Id).WithCurrentCulture();
            return !String.IsNullOrWhiteSpace(email) && await manager.IsEmailConfirmedAsync(user.Id).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns the email of the user for entropy in the token
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public override async Task<string> GetUserModifierAsync(string purpose, InspurUserManager<TUser, TKey> manager,
            TUser user)
        {
            var email = await manager.GetEmailAsync(user.Id).WithCurrentCulture();
            return "Email:" + purpose + ":" + email;
        }

        /// <summary>
        ///     Notifies the user with a token via email using the Subject and BodyFormat
        /// </summary>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task NotifyAsync(string token, InspurUserManager<TUser, TKey> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return manager.SendEmailAsync(user.Id, Subject, String.Format(CultureInfo.CurrentCulture, BodyFormat, token));
        }
    }
}

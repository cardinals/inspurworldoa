﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    class InspurUserValidator
    {
    }/// <summary>
     ///     Validates users before they are saved
     /// </summary>
     /// <typeparam name="TUser"></typeparam>
    public class InspurUserValidator<TUser> : InspurUserValidator<TUser, string> where TUser : class, IUser<string>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public InspurUserValidator(InspurUserManager<TUser, string> manager)
            : base(manager)
        {
        }
    }

    /// <summary>
    ///     Validates users before they are saved
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class InspurUserValidator<TUser, TKey> : IIdentityValidator<TUser>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public InspurUserValidator(InspurUserManager<TUser, TKey> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            AllowOnlyAlphanumericUserNames = true;
            Manager = manager;
        }

        /// <summary>
        ///     Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        public bool AllowOnlyAlphanumericUserNames { get; set; }

        /// <summary>
        ///     If set, enforces that emails are non empty, valid, and unique
        /// </summary>
        public bool RequireUniqueEmail { get; set; }

        private InspurUserManager<TUser, TKey> Manager { get; set; }

        /// <summary>
        ///     Validates a user before saving
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            await ValidateUserName(item, errors).WithCurrentCulture();
            if (RequireUniqueEmail)
            {
                await ValidateEmailAsync(item, errors).WithCurrentCulture();
            }
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }

        private async Task ValidateUserName(TUser user, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PropertyTooShort, "Name"));
            }
            else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
            {
                // If any characters are not letters or digits, its an illegal user name
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.InvalidUserName, user.UserName));
            }
            else
            {
                var owner = await Manager.FindByNameAsync(user.UserName).WithCurrentCulture();
                if (owner != null && !EqualityComparer<TKey>.Default.Equals(owner.Id, user.Id))
                {
                    errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.DuplicateName, user.UserName));
                }
            }
        }

        // make sure email is not empty, valid, and unique
        private async Task ValidateEmailAsync(TUser user, List<string> errors)
        {
            var email = await Manager.GetEmailStore().GetEmailAsync(user).WithCurrentCulture();
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PropertyTooShort, "Email"));
                return;
            }
            try
            {
                var m = new MailAddress(email);
            }
            catch (FormatException)
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.InvalidEmail, email));
                return;
            }
            var owner = await Manager.FindByEmailAsync(email).WithCurrentCulture();
            if (owner != null && !EqualityComparer<TKey>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.DuplicateEmail, email));
            }
        }
    }
}

using InspurOA.Identity.Core;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace InspurOA.Identity.Owin
{
    public class InspurSignInManager<TUser, TKey> : IDisposable
        where TUser : class, IInspurUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="authenticationManager"></param>
        public InspurSignInManager(InspurUserManager<TUser, TKey> userManager, IAuthenticationManager authenticationManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            if (authenticationManager == null)
            {
                throw new ArgumentNullException("authenticationManager");
            }
            InspurUserManager = userManager;
            AuthenticationManager = authenticationManager;
        }

        private string _authType;
        /// <summary>
        /// AuthenticationType that will be used by sign in, defaults to DefaultAuthenticationTypes.ApplicationCookie
        /// </summary>
        public string AuthenticationType
        {
            get { return _authType ?? DefaultAuthenticationTypes.ApplicationCookie; }
            set { _authType = value; }
        }

        /// <summary>
        /// Used to operate on users
        /// </summary>
        public InspurUserManager<TUser, TKey> InspurUserManager { get; set; }

        /// <summary>
        /// Used to sign in identities
        /// </summary>
        public IAuthenticationManager AuthenticationManager { get; set; }

        /// <summary>
        /// Called to generate the ClaimsIdentity for the user, override to add additional claims before SignIn
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            return InspurUserManager.CreateIdentityAsync(user, AuthenticationType);
        }

        /// <summary>
        /// Convert a TKey userId to a string, by default this just calls ToString()
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual string ConvertIdToString(TKey id)
        {
            return Convert.ToString(id, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert a string id to the proper TKey using Convert.ChangeType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)Convert.ChangeType(id, typeof(TKey), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a user identity and then signs the identity using the AuthenticationManager
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        /// <param name="rememberBrowser"></param>
        /// <returns></returns>
        public virtual async Task SignInAsync(TUser user, bool isPersistent, bool rememberBrowser)
        {
            var userIdentity = await CreateUserIdentityAsync(user).WithCurrentCulture();
            // Clear any partial cookies from external or two factor partial sign ins
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(ConvertIdToString(user.Id));
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
            }
        }

        /// <summary>
        /// Send a two factor code to a user
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            var userId = await GetVerifiedUserIdAsync().WithCurrentCulture();
            if (userId == null)
            {
                return false;
            }

            var token = await InspurUserManager.GenerateTwoFactorTokenAsync(userId, provider).WithCurrentCulture();
            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await InspurUserManager.NotifyTwoFactorTokenAsync(userId, provider, token).WithCurrentCulture();
            return true;
        }

        /// <summary>
        /// Get the user id that has been verified already or null.
        /// </summary>
        /// <returns></returns>
        public async Task<TKey> GetVerifiedUserIdAsync()
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie).WithCurrentCulture();
            if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                return ConvertIdFromString(result.Identity.GetUserId());
            }
            return default(TKey);
        }

        /// <summary>
        /// Has the user been verified (ie either via password or external login)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasBeenVerifiedAsync()
        {
            return await GetVerifiedUserIdAsync().WithCurrentCulture() != null;
        }

        /// <summary>
        /// Two factor verification step
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="code"></param>
        /// <param name="isPersistent"></param>
        /// <param name="rememberBrowser"></param>
        /// <returns></returns>
        public virtual async Task<InspurSignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var userId = await GetVerifiedUserIdAsync().WithCurrentCulture();
            if (userId == null)
            {
                return InspurSignInStatus.Failure;
            }
            var user = await InspurUserManager.FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                return InspurSignInStatus.Failure;
            }
            //if (await InspurUserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
            //{
            //    return InspurSignInStatus.LockedOut;
            //}
            if (await InspurUserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code).WithCurrentCulture())
            {
                // When token is verified correctly, clear the access failed count used for lockout
                await InspurUserManager.ResetAccessFailedCountAsync(user.Id).WithCurrentCulture();
                await SignInAsync(user, isPersistent, rememberBrowser).WithCurrentCulture();
                return InspurSignInStatus.Success;
            }
            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await InspurUserManager.AccessFailedAsync(user.Id).WithCurrentCulture();
            return InspurSignInStatus.Failure;
        }

        /// <summary>
        /// Sign the user in using an associated external login
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<InspurSignInStatus> ExternalSignInAsync(InspurExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await InspurUserManager.FindAsync(loginInfo.Login).WithCurrentCulture();
            if (user == null)
            {
                return InspurSignInStatus.Failure;
            }
            //if (await InspurUserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
            //{
            //    return InspurSignInStatus.LockedOut;
            //}
            return await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
        }

        private async Task<InspurSignInStatus> SignInOrTwoFactor(TUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (await InspurUserManager.GetTwoFactorEnabledAsync(user.Id).WithCurrentCulture()
                && (await InspurUserManager.GetValidTwoFactorProvidersAsync(user.Id).WithCurrentCulture()).Count > 0
                && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id).WithCurrentCulture())
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return InspurSignInStatus.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false).WithCurrentCulture();
            return InspurSignInStatus.Success;
        }

        /// <summary>
        /// Sign in the user in using the user name and password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public virtual async Task<InspurSignInStatus> PasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            if (InspurUserManager == null)
            {
                return InspurSignInStatus.Failure;
            }
            //var user = await InspurUserManager.FindByNameAsync(userName).WithCurrentCulture();
            //if (user == null)
            //{
            //}

            var user = await InspurUserManager.FindByEmailAsync(email).WithCurrentCulture();
            if (user == null)
            {
                return InspurSignInStatus.Failure;
            }

            //if (await InspurUserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
            //{
            //    return InspurSignInStatus.LockedOut;
            //}
            if (await InspurUserManager.CheckPasswordAsync(user, password).WithCurrentCulture())
            {
                //await InspurUserManager.ResetAccessFailedCountAsync(user.Id).WithCurrentCulture();
                return await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await InspurUserManager.AccessFailedAsync(user.Id).WithCurrentCulture();
                //if (await InspurUserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                //{
                //    return InspurSignInStatus.LockedOut;
                //}
            }
            return InspurSignInStatus.Failure;
        }


        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}

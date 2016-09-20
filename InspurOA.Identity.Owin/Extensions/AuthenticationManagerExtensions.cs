using InspurOA.Identity.Core;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InspurOA.Identity.Core.Extensions;

namespace InspurOA.Identity.Owin.Extensions
{
    /// <summary>
    ///     Extensions methods on IAuthenticationManager that add methods for using the default Application and External
    ///     authentication type constants
    /// </summary>
    public static class AuthenticationManagerExtensions
    {
        /// <summary>
        ///     Return the authentication types which are considered external because they have captions
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<AuthenticationDescription> InspurGetExternalAuthenticationTypes(
            this IAuthenticationManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return manager.GetAuthenticationTypes(d => d.Properties != null && d.Properties.ContainsKey("Caption"));
        }

        /// <summary>
        ///     Return the identity associated with the default external authentication type
        /// </summary>
        /// <returns></returns>
        public static async Task<ClaimsIdentity> InspurGetExternalIdentityAsync(this IAuthenticationManager manager,
            string externalAuthenticationType)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = await manager.AuthenticateAsync(externalAuthenticationType).WithCurrentCulture();
            if (result != null && result.Identity != null &&
                result.Identity.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                return result.Identity;
            }
            return null;
        }

        /// <summary>
        /// Return the identity associated with the default external authentication type
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="externalAuthenticationType"></param>
        /// <returns></returns>
        public static ClaimsIdentity InspurGetExternalIdentity(this IAuthenticationManager manager,
            string externalAuthenticationType)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return InspurAsyncHelper.RunSync(() => manager.GetExternalIdentityAsync(externalAuthenticationType));
        }

        private static InspurExternalLoginInfo InspurGetExternalLoginInfo(AuthenticateResult result)
        {
            if (result == null || result.Identity == null)
            {
                return null;
            }
            var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return null;
            }
            // By default we don't allow spaces in user names
            var name = result.Identity.Name;
            if (name != null)
            {
                name = name.Replace(" ", "");
            }
            var email = result.Identity.FindFirstValue(ClaimTypes.Email);
            return new InspurExternalLoginInfo
            {
                ExternalIdentity = result.Identity,
                Login = new InspurUserLoginInfo(idClaim.Issuer, idClaim.Value),
                DefaultUserName = name,
                Email = email
            };
        }

        /// <summary>
        ///     Extracts login info out of an external identity
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static async Task<InspurExternalLoginInfo> InspurGetExternalLoginInfoAsync(this IAuthenticationManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return InspurGetExternalLoginInfo(await manager.AuthenticateAsync(InspurDefaultAuthenticationTypes.ExternalCookie).WithCurrentCulture());
        }

        /// <summary>
        ///     Extracts login info out of an external identity
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static InspurExternalLoginInfo InspurGetExternalLoginInfo(this IAuthenticationManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return InspurAsyncHelper.RunSync(manager.InspurGetExternalLoginInfoAsync);
        }

        /// <summary>
        ///     Extracts login info out of an external identity
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="xsrfKey">key that will be used to find the userId to verify</param>
        /// <param name="expectedValue">
        ///     the value expected to be found using the xsrfKey in the AuthenticationResult.Properties
        ///     dictionary
        /// </param>
        /// <returns></returns>
        public static InspurExternalLoginInfo InspurGetExternalLoginInfo(this IAuthenticationManager manager, string xsrfKey,
            string expectedValue)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return InspurAsyncHelper.RunSync(() => manager.InspurGetExternalLoginInfoAsync(xsrfKey, expectedValue));
        }

        /// <summary>
        ///     Extracts login info out of an external identity
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="xsrfKey">key that will be used to find the userId to verify</param>
        /// <param name="expectedValue">
        ///     the value expected to be found using the xsrfKey in the AuthenticationResult.Properties
        ///     dictionary
        /// </param>
        /// <returns></returns>
        public static async Task<InspurExternalLoginInfo> InspurGetExternalLoginInfoAsync(this IAuthenticationManager manager,
            string xsrfKey, string expectedValue)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = await manager.AuthenticateAsync(InspurDefaultAuthenticationTypes.ExternalCookie).WithCurrentCulture();
            // Verify that the userId is the same as what we expect if requested
            if (result != null &&
                result.Properties != null &&
                result.Properties.Dictionary != null &&
                result.Properties.Dictionary.ContainsKey(xsrfKey) &&
                result.Properties.Dictionary[xsrfKey] == expectedValue)
            {
                return InspurGetExternalLoginInfo(result);
            }
            return null;
        }

        /// <summary>
        ///     Returns true if there is a TwoFactorRememberBrowser cookie for a user
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> InspurTwoFactorBrowserRememberedAsync(this IAuthenticationManager manager,
            string userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result =
                await manager.AuthenticateAsync(InspurDefaultAuthenticationTypes.TwoFactorRememberBrowserCookie).WithCurrentCulture();
            return (result != null && result.Identity != null && result.Identity.GetUserId() == userId);
        }

        /// <summary>
        ///     Returns true if there is a TwoFactorRememberBrowser cookie for a user
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool InspurTwoFactorBrowserRemembered(this IAuthenticationManager manager,
            string userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            return InspurAsyncHelper.RunSync(() => manager.TwoFactorBrowserRememberedAsync(userId));
        }

        /// <summary>
        ///     Creates a TwoFactorRememberBrowser cookie for a user
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ClaimsIdentity InspurCreateTwoFactorRememberBrowserIdentity(this IAuthenticationManager manager,
            string userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var rememberBrowserIdentity = new ClaimsIdentity(InspurDefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            return rememberBrowserIdentity;
        }
    }
}

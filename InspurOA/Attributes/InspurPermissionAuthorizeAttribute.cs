using InspurOA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Attributes
{
    public class InspurPermissionAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static readonly char[] _splitParameter = new[] { ',' };

        private string _permissions;
        private string[] _permissionsSplit = new string[0];

        public string Permissions
        {
            get { return _permissions ?? String.Empty; }
            set
            {
                _permissions = value;
                _permissionsSplit = SplitString(value);
            }
        }

        protected bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (_permissionsSplit.Length > 0 && !AuthHelper.IsAuthorizedByPermissions(user.Identity.Name, _permissionsSplit))
            {
                return false;
            }

            return true;
        }


        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null);
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        protected void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        protected HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthorized = AuthorizeCore(httpContext);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }


        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(_splitParameter)
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}
using InspurOA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace InspurOA.Principal
{
    public class InspurPrincipal : IInspurPrincipal
    {
        public IIdentity Identity { get; private set; }

        public InspurPrincipal(string userName)
        {
            this.Identity = new GenericIdentity(userName);
        }

        public bool IsInRole(string roleCode)
        {
            return AuthHelper.IsAuthorizedByRole(Identity.Name, roleCode);
        }

        public bool IsInPermission(string permissionCode)
        {
            return AuthHelper.IsAuthorizedByPermission(Identity.Name, permissionCode);
        }
    }
}
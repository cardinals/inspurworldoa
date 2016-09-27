using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace InspurOA.Principal
{
    public interface IInspurPrincipal : IPrincipal
    {
        bool IsInPermission(string permissionCode);
    }
}
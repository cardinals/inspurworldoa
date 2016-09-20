using InspurOA.Identity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurIdentityPermission : InspurIdentityPermission<string>
    {
        public InspurIdentityPermission()
        {
            PermissionId = Guid.NewGuid().ToString();
        }

        public InspurIdentityPermission(string permissionCode) : this()
        {
            PermissionCode = permissionCode;
        }
    }

    public class InspurIdentityPermission<TKey> :
        IPermission<TKey>
    {
        public TKey PermissionId { get; set; }

        public string PermissionCode { get; set; }

        public string PermissionDescription { get; set; }
    }
}

using InspurOA.Identity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurIdentityRolePermission : InspurIdentityRolePermission<string>
    {
    }

    public class InspurIdentityRolePermission<TKey> : IInspurRolePermission<TKey>
    {
        public virtual TKey RoleId { get; set; }

        public virtual TKey PermissionId { get; set; }
    }
}

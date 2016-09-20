using InspurOA.Identity.Core;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurIdentityRole :
        InspurIdentityRole<string, InspurIdentityUserRole, InspurIdentityRolePermission>
    {
        public InspurIdentityRole()
        {
            RoleId = Guid.NewGuid().ToString();
        }

        public InspurIdentityRole(string roleName) : this()
        {
            RoleName = roleName;
        }
    }

    public class InspurIdentityRole<TKey, TUserRole, TRolePermission> :
        IInspurRole<TKey>
        where TUserRole : InspurIdentityUserRole<TKey>
        where TRolePermission : InspurIdentityRolePermission<TKey>
    {
        public InspurIdentityRole()
        {
            Users = new List<TUserRole>();
            Permissions = new List<TRolePermission>();
        }

        public ICollection<TUserRole> Users { get; private set; }

        public ICollection<TRolePermission> Permissions { get; private set; }

        public TKey RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }
    }
}

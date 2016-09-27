using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurRolePermission<TKey>
    {
        TKey RoleId { get; set; }

        TKey PermissionId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurPermission<out TKey>
    {
        TKey PermissionId { get; }

        string PermissionCode { get; set; }
    }
}

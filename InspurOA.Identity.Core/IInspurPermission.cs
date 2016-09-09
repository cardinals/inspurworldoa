using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurPermission : IPermission<string>
    {
    }

    public interface IPermission<out TKey>
    {
        TKey PermissionId { get; }

        string PermissionCode { get; set; }
    }
}

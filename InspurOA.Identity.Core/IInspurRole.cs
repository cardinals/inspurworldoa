using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurRole<Tkey>
    {
        Tkey RoleId { get; set; }

        string RoleCode { get; set; }
    }
}

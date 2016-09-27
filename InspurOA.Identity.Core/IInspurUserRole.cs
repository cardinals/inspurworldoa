using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurUserRole<TKey>
    {
        TKey UserId { get; set; }

        TKey RoleId { get; set; }
    }
}

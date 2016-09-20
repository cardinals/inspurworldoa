using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurIdentityUserRole : InspurIdentityUserRole<string>
    {
    }

    public class InspurIdentityUserRole<TKey>
    {
        public TKey UserId { get; set; }

        public TKey RoleId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurQueryableUserRoleStore<TUser,TUserRole, in TKey> : IInspurUserRoleStore<TUser,TUserRole, TKey>
        where TUser :class,IInspurUser<TKey>
        where TUserRole : class, IInspurUserRole<TKey>
    {
        IQueryable<TUserRole> UserRoles { get; }
    }

}

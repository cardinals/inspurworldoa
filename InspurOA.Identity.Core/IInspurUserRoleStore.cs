using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurUserRoleStore<TUser, TUserRole> : IInspurUserRoleStore<TUser, TUserRole, string>
        where TUser : class, IInspurUser<string>
        where TUserRole : class, IInspurUserRole<string>
    {

    }

    public interface IInspurUserRoleStore<TUser, TUserRole, in TKey> : IDisposable
        where TUser : class, IInspurUser<TKey>
        where TUserRole : class, IInspurUserRole<TKey>
    {
        //Task CreateAsync(TUserRole userRole);

        //Task UpdateAsync(TUserRole userRole);

        //Task DeleteAsync(TUserRole userRole);

        Task<IList<TUserRole>> FindUserRolesByUserId(TKey userId);

        Task<IList<TUserRole>> FindUserRolesByRoleId(TKey roleId);

        Task<bool> IsUserRoleExisted(TKey userId, TKey roleId);

        Task AddToRoleAsync(TUser user, string roleCode, bool onlyAllowSingleRoles = true);

        Task RemoveFromRoleAsync(TUser user, string roleCode);

        Task<IList<TUserRole>> GetUserRolesAsync(TUser user);

        Task<IList<string>> GetRoleCodesAsync(TUser user);

        Task<bool> IsInRoleAsync(TUser user, string roleCode);
    }
}

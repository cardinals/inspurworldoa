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

        Task AddToRoleAsync(string userId, string roleCode, bool onlyAllowSingleRoles = true);

        Task RemoveFromRoleAsync(string userId, string roleCode);

        Task RemoveUserFromUserRoleAsync(string userId);

        Task RemoveRoleFromUserRoleAsync(string roleId);

        Task<IList<TUserRole>> GetUserRolesAsync(TUser user);

        Task<IList<string>> GetRoleCodesAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string roleCode);
    }
}

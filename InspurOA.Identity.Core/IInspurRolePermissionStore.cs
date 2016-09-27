using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurRolePermissionStore<TRole, TPermission, TRolePermission> : IInspurRolePermissionStore<TRole, TPermission, TRolePermission, string>
           where TRole : class, IInspurRole<string>
        where TPermission : class, IInspurPermission<string>
        where TRolePermission : class, IInspurRolePermission<string>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRolePermission">The type of the role permission.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IInspurRolePermissionStore<TRole, TPermission, TRolePermission, in TKey> : IDisposable
        where TRole : class, IInspurRole<TKey>
        where TPermission : class, IInspurPermission<TKey>
        where TRolePermission : class, IInspurRolePermission<TKey>
    {

        ///// <summary>
        ///// Create a new rolePermission
        ///// </summary>
        ///// <param name="rolePermission">The role permission.</param>
        ///// <returns></returns>
        //Task CreateAsync(TRolePermission rolePermission);

        ///// <summary>
        ///// Update a rolePermission
        ///// </summary>
        ///// <param name="rolePermission">The role permission.</param>
        ///// <returns></returns>
        //Task UpdateAsync(TRolePermission rolePermission);

        ///// <summary>
        ///// Delete a rolePermission
        ///// </summary>
        ///// <param name="rolePermission">The role permission.</param>
        ///// <returns></returns>
        //Task DeleteAsync(TRolePermission rolePermission);

        /// <summary>
        /// Find a rolePermission by id
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        Task<IList<TRolePermission>> FindRolePermissionsByRoleId(TKey roleId);

        /// <summary>
        /// Finds the by permission identifier asynchronous.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns></returns>
        Task<IList<TRolePermission>> FindRolePermissionsByPermissionId(TKey permissionId);

        Task AddPermissionToRoleAsync(IList<TPermission> permissionList, string roleCode);

        Task AddPermissionToRoleAsync(IList<TPermission> permissionList, TRole role);

        Task RemovePermissionsOfRoleAsync(string roleCode);

        Task RemovePermissionsOfRoleAsync(TRole role);

        Task RemoveRoleFromRolePermissionAsync(string roleId);

        Task RemovePermissionFromRolePermissionAsync(string permissionId);

        Task<IList<TPermission>> GetPermissionAsync(string roleCode);

        Task<IList<TPermission>> GetPermissionAsync(TRole role);

        Task<bool> HasPermissionAsync(string roleCode, string permissionCode);

        //Task<bool> IsAuthorizedByPermissionAsync(TUser user, string permissionCode);

        Task<bool> IsAuthorizedByPermissionsAsync(string userName, string[] permissionCodes);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface that exposes basic role management
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public interface IInspurRoleStore<TRole> : IInspurRoleStore<TRole, string>
        where TRole : IInspurRole<string>
    {
    }

    /// <summary>
    ///     Interface that exposes basic role management
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurRoleStore<TRole, in TKey> : IDisposable 
        where TRole : IInspurRole<TKey>
    {
        /// <summary>
        ///     Create a new role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task CreateAsync(TRole role);

        /// <summary>
        ///     Update a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task UpdateAsync(TRole role);

        /// <summary>
        ///     Delete a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task DeleteAsync(TRole role);

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<TRole> FindByIdAsync(TKey roleId);

        /// <summary>
        ///     Find a role by code
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        Task<TRole> FindByCodeAsync(string roleCode);
    }
}

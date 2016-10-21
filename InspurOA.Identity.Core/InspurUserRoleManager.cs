using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    //public class InspurUserRoleManager<TUser, TRole, TUserRole> : InspurUserRoleManager<TUser, TRole, TUserRole>
    //       where TUser : class, IInspurUser<string>
    //    where TRole : class, IInspurRole<string>
    //    where TUserRole : class, IInspurUserRole<string>
    //{
    //    public InspurUserRoleManager(
    //        IInspurUserStore<TUser, string> userStore,
    //        IInspurRoleStore<TRole, string> roleStore, 
    //        IInspurUserRoleStore<TUser, TUserRole, string> store)
    //        : base(userStore, roleStore, store)
    //    {
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TUserRole">The type of the user role.</typeparam>
    /// <typeparam name="string">The type of the key.</typeparam>
    public class InspurUserRoleManager<TUser, TRole, TUserRole> : IDisposable
        where TUser : class, IInspurUser<string>
        where TRole : class, IInspurRole<string>
        where TUserRole : class, IInspurUserRole<string>
    {
        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspurUserRoleManager{TUser, TRole, TUserRole, string}"/> class.
        /// </summary>
        /// <param name="userStore">The userstore.</param>
        /// <param name="roleStore">The role store.</param>
        /// <param name="store">The store.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public InspurUserRoleManager(
            IInspurUserStore<TUser, string> userStore,
            IInspurRoleStore<TRole, string> roleStore,
            IInspurUserRoleStore<TUser, TUserRole, string> store)
        {
            if (userStore == null)
            {
                throw new ArgumentNullException("userStore");
            }

            if (roleStore == null)
            {
                throw new ArgumentNullException("roleStore");
            }

            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            UserStore = userStore;
            RoleStore = roleStore;
            Store = store;
        }

        /// <summary>
        /// Gets or sets the user store.
        /// </summary>
        /// <value>
        /// The user store.
        /// </value>
        protected internal IInspurUserStore<TUser, string> UserStore { get; set; }

        /// <summary>
        /// Gets or sets the role store.
        /// </summary>
        /// <value>
        /// The role store.
        /// </value>
        protected internal IInspurRoleStore<TRole, string> RoleStore { get; set; }

        /// <summary>
        /// Gets or sets the store.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        protected internal IInspurUserRoleStore<TUser, TUserRole, string> Store { get; set; }

        /// <summary>
        ///     Returns true if the store is an IInspurUserRoleStore
        /// </summary>
        public virtual bool SupportsUserRole
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserRoleStore<TUser, TUserRole, string>;
            }
        }

        public virtual IQueryable<TUserRole> UserRoles
        {
            get
            {
                var queryableStore = Store as IInspurQueryableUserRoleStore<TUser, TUserRole, string>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException(InspurResources.StoreNotIQueryableUserRoleStore);
                }

                return queryableStore.UserRoles;
            }
        }

        /// <summary>
        /// Add a user to a role
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<IdentityResult> AddToRoleAsync(string userId, string roleCode)
        {
            ThrowIfDisposed();
            var userRoleCodes = await Store.GetRoleCodesAsync(userId).WithCurrentCulture();
            if (userRoleCodes.Contains(roleCode))
            {
                return new IdentityResult(InspurResources.UserAlreadyInRole);
            }

            await Store.AddToRoleAsync(userId, roleCode).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        /// Method to add user to multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">roles</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<IdentityResult> AddToRolesAsync(string userId, params string[] roleCodes)
        {
            ThrowIfDisposed();
            if (roleCodes == null || roleCodes.Length == 0)
            {
                throw new ArgumentNullException("roles");
            }

            var userRoleCodes = await Store.GetRoleCodesAsync(userId).WithCurrentCulture();
            foreach (var code in roleCodes)
            {
                if (userRoleCodes.Contains(code))
                {
                    return new IdentityResult(InspurResources.UserAlreadyInRole);
                }

                await Store.AddToRoleAsync(userId, code).WithCurrentCulture();
            }
            return IdentityResult.Success;
        }

        /// <summary>
        /// Remove user from multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">roles</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles)
        {
            ThrowIfDisposed();
            if (roles == null || roles.Length == 0)
            {
                throw new ArgumentNullException("roles");
            }

            var userRoleCodes = await Store.GetRoleCodesAsync(userId).WithCurrentCulture();
            foreach (var role in roles)
            {
                if (!userRoleCodes.Contains(role))
                {
                    return new IdentityResult(InspurResources.UserNotInRole);
                }

                await Store.RemoveFromRoleAsync(userId, role).WithCurrentCulture();
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Remove a user from a role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleCode">The role code.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleCode)
        {
            ThrowIfDisposed();
            if (!await Store.IsInRoleAsync(userId, roleCode).WithCurrentCulture())
            {
                return new IdentityResult(InspurResources.UserNotInRole);
            }

            await Store.RemoveFromRoleAsync(userId, roleCode).WithCurrentCulture();
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> RemoveFromRoleInfoAsync(string userId)
        {
            ThrowIfDisposed();
            await Store.RemoveUserFromUserRoleAsync(userId).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        /// Returns the roles for the user
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<IList<string>> GetRoleCodesAsync(string userId)
        {
            ThrowIfDisposed();
            return await Store.GetRoleCodesAsync(userId).WithCurrentCulture();
        }

        /// <summary>
        /// Returns true if the user is in the specified role
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleCode">The role code.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> IsInRoleAsync(string userId, string roleCode)
        {
            ThrowIfDisposed();
            return await Store.IsInRoleAsync(userId, roleCode).WithCurrentCulture();
        }

        /// <summary>
        /// Determines whether [is authorized by roles asynchronous] [the specified user name].
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="roleCodes">The role codes.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">userName</exception>
        /// <exception cref="ArgumentException">ValueCannotBeNullOrEmpty;roleCodes</exception>
        public async Task<bool> IsAuthorizedByRolesAsync(string userName, string[] roleCodes)
        {
            try
            {
                ThrowIfDisposed();
                if (String.IsNullOrWhiteSpace(userName))
                {
                    return await Task.FromResult(false);
                }

                if (roleCodes == null || roleCodes.Length == 0)
                {
                    throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCodes");
                }

                var user = await UserStore.FindByNameAsync(userName).WithCurrentCulture();
                if (user != null)
                {
                    foreach (string roleCode in roleCodes)
                    {
                        var role = await RoleStore.FindByCodeAsync(roleCode).WithCurrentCulture();
                        if (await Store.IsUserRoleExisted(user.Id, role.RoleId))
                        {
                            return await Task.FromResult(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(false);
        }

        /// <summary>
        /// Throws if disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     When disposing, actually dipose the store
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
                UserStore.Dispose();
                RoleStore.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        ///     Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public class InspurRolePermissionManager<TUser, TRole, TPermission, TRolePermission>
        : InspurRolePermissionManager<TUser, TRole, TPermission, string, TRolePermission>
        where TUser : class, IInspurUser<string>
        where TRole : class, IInspurRole<string>
        where TPermission : class, IInspurPermission<string>
        where TRolePermission : class, IInspurRolePermission<string>
    {
        public InspurRolePermissionManager(
            IInspurRoleStore<TRole, string> roleStore,
            IInspurRolePermissionStore<TRole, TPermission, TRolePermission, string> store)
            : base(roleStore, store)
        {
        }
    }

    public class InspurRolePermissionManager<TUser, TRole, TPermission, TKey, TRolePermission> : IDisposable
        where TUser : class, IInspurUser<TKey>
        where TRole : class, IInspurRole<TKey>
        where TPermission : class, IInspurPermission<TKey>
        where TRolePermission : class, IInspurRolePermission<TKey>
    {
        private bool _disposed;

        public InspurRolePermissionManager(
            IInspurRoleStore<TRole, TKey> roleStore,
            IInspurRolePermissionStore<TRole, TPermission, TRolePermission, TKey> store)
        {
            if (roleStore == null)
            {
                throw new ArgumentNullException("roleStore");
            }

            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            RoleStore = roleStore;
            Store = store;
        }

        protected internal IInspurRoleStore<TRole, TKey> RoleStore;

        protected internal IInspurRolePermissionStore<TRole, TPermission, TRolePermission, TKey> Store;

        public virtual IQueryable<TRolePermission> RolePermissions
        {
            get
            {
                var queryableStore = Store as IInspurQueryableRolePermissionStore<TRole, TPermission, TRolePermission, TKey>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException(InspurResources.StoreNotIQueryableRolePemrissionStore);
                }

                return queryableStore.RolePermissions;
            }
        }

        public async Task<IList<TRolePermission>> FindRolePermissionsByRoleId(TKey roleId)
        {
            ThrowIfDisposed();
            if (roleId == null)
            {
                throw new ArgumentNullException("roleId");
            }

            return await Store.FindRolePermissionsByRoleId(roleId);
        }

        public async Task<IList<TRolePermission>> FindRolePermissionsByPermissionId(TKey permissionId)
        {
            ThrowIfDisposed();
            if (permissionId == null)
            {
                throw new ArgumentNullException("permissionId");
            }

            return await Store.FindRolePermissionsByPermissionId(permissionId);
        }

        public async Task AddPermissionToRoleAsync(IList<TPermission> permissionList, string roleCode)
        {
            ThrowIfDisposed();
            if (permissionList == null || permissionList.Count == 0)
            {
                throw new ArgumentNullException("permissionList", "Null or has no element.");
            }

            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            await Store.AddPermissionToRoleAsync(permissionList, roleCode);
        }

        public async Task AddPermissionToRoleAsync(IList<TPermission> permissionList, TRole role)
        {
            ThrowIfDisposed();
            if (permissionList == null || permissionList.Count == 0)
            {
                throw new ArgumentNullException("permissionList", "Null or has no element.");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await Store.AddPermissionToRoleAsync(permissionList, role);
        }

        public async Task RemovePermissionsOfRoleAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            await Store.RemovePermissionsOfRoleAsync(roleCode);
        }

        public async Task RemovePermissionsOfRoleAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await Store.RemovePermissionsOfRoleAsync(role);
        }

        public async Task RemoveRoleFromRolePermissionAsync(string roleId)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleId");
            }

            await Store.RemoveRoleFromRolePermissionAsync(roleId);
        }

        public async Task RemovePermissionFromRolePermissionAsync(string permissionId)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(permissionId))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "permissionId");
            }

            await Store.RemovePermissionFromRolePermissionAsync(permissionId);
        }

        public async Task<IList<TPermission>> GetPermissionAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            return await Store.GetPermissionAsync(roleCode);
        }

        public async Task<IList<TPermission>> GetPermissionAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            return await Store.GetPermissionAsync(role);
        }

        public async Task<bool> HasPermissionAsync(string roleCode, string permissionCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "permissionCode");
            }

            return await Store.HasPermissionAsync(roleCode, permissionCode);
        }

        //public async Task<bool> IsAuthorizedByPermissionAsync(TUser user, string permissionCode)
        //{
        //    ThrowIfDisposed();
        //    if (user == null)
        //    {
        //        throw new ArgumentNullException("user");
        //    }

        //    if (string.IsNullOrWhiteSpace(permissionCode))
        //    {
        //        throw new ArgumentException("ValueCannotBeNullOrEmpty", "permissionCode");
        //    }

        //    var userRole = await _userRoleStore.EntitySet.SingleOrDefaultAsync(ur => ur.UserId.Equals(user.Id)).WithCurrentCulture();
        //    if (userRole != null)
        //    {
        //        var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleId.Equals(userRole.RoleId)).WithCurrentCulture();
        //        if (role != null)
        //        {
        //            return await HasPermissionAsync(role.RoleCode, permissionCode);
        //        }
        //    }

        //    return await Task.FromResult(false);
        //}

        public async Task<bool> IsAuthorizedByPermissionAsync(string userName, string permissionCode)
        {
            var result = Store.IsAuthorizedByPermissionsAsync(userName, new string[] { permissionCode }).Result;
            if (result)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> IsAuthorizedByPermissionsAsync(string userName, string[] permissionCodes)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName");
            }

            if (permissionCodes == null || permissionCodes.Length == 0)
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "permissionCodes");
            }
            var result = Store.IsAuthorizedByPermissionsAsync(userName, permissionCodes).Result;
            if (result)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
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

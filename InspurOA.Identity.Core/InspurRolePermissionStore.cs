using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspurOA.Identity.EntityFramework;

namespace InspurOA.Identity.Core
{
    public class InspurRolePermissionStore<TUser, TRole, TPermission, TKey, TRolePermission> : IInspurQueryableRolePermissionStore<TRolePermission,TKey>
          where TUser : class, IInspurUser<TKey>
        where TRole : class, IInspurRole<TKey>
        where TPermission : class, IInspurPermission<TKey>
        where TRolePermission : class, IInspurRolePermission<TKey>
    {
        private bool _disposed;
        private EntityStore<TRolePermission> _rolePermissionStore;

        public InspurRolePermissionStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            AutoSaveChanges = true;
            Context = context;
            _rolePermissionStore = new EntityStore<TRolePermission>(context);
        }

        public DbContext Context { get; private set; }

        public bool DisposeContext { get; set; }

        public bool AutoSaveChanges { get; set; }

        public IQueryable<TRolePermission> RolePermissions
        {
            get { return _rolePermissionStore.EntitySet; }
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

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper().Equals(roleCode.ToUpper())).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            foreach (var permission in permissionList)
            {
                _rolePermissionStore.Create(new TRolePermission { RoleId = roleEntity.RoleId, PermissionId = permission.PermissionId });
            }

            await SaveChanges().WithCurrentCulture();
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

            foreach (var permission in permissionList)
            {
                _rolePermissionStore.Create(new TRolePermission { RoleId = role.RoleId, PermissionId = permission.PermissionId });
            }

            await SaveChanges().WithCurrentCulture();
        }

        public async Task RemovePermissionsOfRoleAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper().Equals(roleCode.ToUpper())).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            var userRoles = _rolePermissions.Where(rp => rp.RoleId.Equals(roleEntity.RoleId));
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await SaveChanges().WithCurrentCulture();
        }

        public async Task RemovePermissionsOfRoleAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var userRoles = _rolePermissions.Where(rp => rp.RoleId.Equals(role.RoleId));
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await SaveChanges().WithCurrentCulture();
        }

        public async Task<IList<TPermission>> GetPermissionAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper().Equals(roleCode.ToUpper())).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            IList<TPermission> permissionList = new List<TPermission>();
            var userRoles = _rolePermissions.Where(rp => rp.RoleId.Equals(roleEntity.RoleId));
            foreach (var userRole in userRoles)
            {
                var permission = await _permissionStore.DbEntitySet.SingleOrDefaultAsync(p => p.PermissionId.Equals(userRole.PermissionId)).WithCurrentCulture();
                if (permission != null)
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        public async Task<IList<TPermission>> GetPermissionAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            IList<TPermission> permissionList = new List<TPermission>();
            var userRoles = _rolePermissions.Where(rp => rp.RoleId.Equals(role.RoleId));
            foreach (var userRole in userRoles)
            {
                var permission = await _permissionStore.DbEntitySet.SingleOrDefaultAsync(p => p.PermissionId.Equals(userRole.PermissionId)).WithCurrentCulture();
                if (permission != null)
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
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

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            var permissionEntity = await _permissionStore.DbEntitySet.SingleOrDefaultAsync(p => p.PermissionCode == permissionCode).WithCurrentCulture();
            if (permissionEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", permissionCode));
            }

            return await _rolePermissions.AnyAsync(rp => rp.RoleId.Equals(roleEntity.RoleId) && rp.PermissionId.Equals(permissionEntity.PermissionId));
        }

        public async Task<bool> IsAuthorizedByPermissionAsync(TUser user, string permissionCode)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "permissionCode");
            }

            var userRole = _userRoles.SingleOrDefault(ur => ur.UserId.Equals(user.Id));
            if (userRole != null)
            {
                var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleId.Equals(userRole.RoleId)).WithCurrentCulture();
                if (role != null)
                {
                    return await HasPermissionAsync(role.RoleCode, permissionCode);
                }
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> IsAuthorizedByPermissionAsync(string userName, string permissionCode)
        {
            return await IsAuthorizedByPermissionsAsync(userName, new string[] { permissionCode });
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

            try
            {
                var user = await _userStore.DbEntitySet.SingleOrDefaultAsync(u => u.UserName.Equals(userName)).WithCurrentCulture();

                if (user != null)
                {
                    IQueryable<TUserRole> userRoles = _userRoles.Where(ur => ur.UserId.Equals(user.Id));
                    if (userRoles != null)
                    {
                        foreach (var userRole in userRoles.ToList())
                        {
                            var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleId.Equals(userRole.RoleId)).WithCurrentCulture();
                            if (role != null)
                            {
                                foreach (var permissionCode in permissionCodes)
                                {
                                    if (await HasPermissionAsync(role.RoleCode, permissionCode))
                                    {
                                        return await Task.FromResult(true);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return await Task.FromResult(false);
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }

            _disposed = true;
            Context = null;
            _userStore = null;
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

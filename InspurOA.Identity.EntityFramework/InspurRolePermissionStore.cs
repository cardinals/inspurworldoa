using InspurOA.Identity.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurRolePermissionStore<TUser> : InspurRolePermissionStore<TUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityUserRole, InspurIdentityRolePermission>,
        IInspurRolePermissionStore<InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission>

        where TUser : class, IInspurUser
    {
        public InspurRolePermissionStore(DbContext context) : base(context)
        {

        }

        public InspurRolePermissionStore() : this(new InspurIdentityDbContext())
        {
            DisposeContext = true;
        }
    }

    public class InspurRolePermissionStore<TUser, TRole, TPermission, TUserRole, TRolePermission> : IInspurQueryableRolePermissionStore<TRole, TPermission, TRolePermission, string>
        where TUser : class, IInspurUser<string>
        where TUserRole : class, IInspurUserRole<string>
        where TRole : class, IInspurRole<string>
        where TPermission : class, IInspurPermission<string>
        where TRolePermission : class, IInspurRolePermission<string>, new()
    {
        private bool _disposed;
        private EntityStore<TUser> _userStore;
        private EntityStore<TUserRole> _userRoleStore;
        private EntityStore<TRole> _roleStore;
        private EntityStore<TRolePermission> _rolePermissionStore;
        private EntityStore<TPermission> _permissionStore;

        public InspurRolePermissionStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Context = context;
            _userStore = new EntityStore<TUser>(context);
            _userRoleStore = new EntityStore<TUserRole>(context);
            _roleStore = new EntityStore<TRole>(context);
            _rolePermissionStore = new EntityStore<TRolePermission>(context);
            _permissionStore = new EntityStore<TPermission>(context);
        }

        public DbContext Context { get; private set; }

        public bool DisposeContext { get; set; }

        public IQueryable<TRolePermission> RolePermissions
        {
            get { return _rolePermissionStore.EntitySet; }
        }

        /// <summary>
        /// Create a new rolePermission
        /// </summary>
        /// <param name="rolePermission">The role permission.</param>
        /// <returns></returns>
        public async Task CreateAsync(TRolePermission rolePermission)
        {
            ThrowIfDisposed();
            if (rolePermission == null)
            {
                throw new ArgumentNullException("rolePermission");
            }

            _rolePermissionStore.Create(rolePermission);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        /// Update a rolePermission
        /// </summary>
        /// <param name="rolePermission">The role permission.</param>
        /// <returns></returns>
        public async Task UpdateAsync(TRolePermission rolePermission)
        {
            ThrowIfDisposed();
            if (rolePermission == null)
            {
                throw new ArgumentNullException("rolePermission");
            }

            _rolePermissionStore.Update(rolePermission);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        /// Delete a rolePermission
        /// </summary>
        /// <param name="rolePermission">The role permission.</param>
        /// <returns></returns>
        public async Task DeleteAsync(TRolePermission rolePermission)
        {
            ThrowIfDisposed();
            if (rolePermission == null)
            {
                throw new ArgumentNullException("rolePermission");
            }

            _rolePermissionStore.Delete(rolePermission);
            await Context.SaveChangesAsync().WithCurrentCulture();

        }

        /// <summary>
        /// Find a rolePermission by id
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public async Task<IList<TRolePermission>> FindRolePermissionsByRoleId(string roleId)
        {
            ThrowIfDisposed();
            var query = from rolePermission in _rolePermissionStore.EntitySet
                        where rolePermission.RoleId == roleId
                        select rolePermission;

            return await query.ToListAsync().WithCurrentCulture();
        }

        /// <summary>
        /// Finds the by permission identifier asynchronous.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns></returns>
        public async Task<IList<TRolePermission>> FindRolePermissionsByPermissionId(string permissionId)
        {
            ThrowIfDisposed();
            var query = from rolePermission in _rolePermissionStore.EntitySet
                        where rolePermission.PermissionId == permissionId
                        select rolePermission;

            return await query.ToListAsync().WithCurrentCulture();
        }

        public async Task AddPermissionToRoleAsync(IList<string> permissionIdList, string roleId)
        {
            ThrowIfDisposed();
            if (permissionIdList == null || permissionIdList.Count == 0)
            {
                throw new ArgumentNullException("permissionIdList", "Null or has no element.");
            }

            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleId");
            }

            var roleEntity = await _roleStore.DbEntitySet.FirstOrDefaultAsync(r => r.RoleId == roleId).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleId));
            }

            foreach (var id in permissionIdList)
            {
                _rolePermissionStore.Create(new TRolePermission { RoleId = roleEntity.RoleId, PermissionId = id });
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task AddPermissionToRoleAsync(IList<string> permissionIdList, TRole role)
        {
            ThrowIfDisposed();
            if (permissionIdList == null || permissionIdList.Count == 0)
            {
                throw new ArgumentNullException("permissionList", "Null or has no element.");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            foreach (var id in permissionIdList)
            {
                _rolePermissionStore.Create(new TRolePermission { RoleId = role.RoleId, PermissionId = id });
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task RemovePermissionsOfRoleAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.RoleId == roleEntity.RoleId);
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task RemovePermissionsOfRoleAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.RoleId == role.RoleId);
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task RemoveRoleFromRolePermissionAsync(string roleId)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.RoleId == roleId);
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task RemovePermissionFromRolePermissionAsync(string permissionId)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(permissionId))
            {
                throw new ArgumentNullException("permissionId");
            }

            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.PermissionId == permissionId);
            foreach (var userRole in userRoles)
            {
                _rolePermissionStore.Delete(userRole);
            }

            await Context.SaveChangesAsync().WithCurrentCulture();
        }


        public async Task<IList<TPermission>> GetPermissionAsync(string roleCode)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            IList<TPermission> permissionList = new List<TPermission>();
            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.RoleId == roleEntity.RoleId);
            foreach (var userRole in userRoles)
            {
                var permission = await _permissionStore.EntitySet.SingleOrDefaultAsync(p => p.PermissionId == userRole.PermissionId).WithCurrentCulture();
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
            var userRoles = _rolePermissionStore.EntitySet.Where(rp => rp.RoleId == role.RoleId);
            foreach (var userRole in userRoles)
            {
                var permission = await _permissionStore.EntitySet.SingleOrDefaultAsync(p => p.PermissionId == userRole.PermissionId).WithCurrentCulture();
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

            var permissionEntity = await _permissionStore.EntitySet.SingleOrDefaultAsync(p => p.PermissionCode == permissionCode).WithCurrentCulture();
            if (permissionEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", permissionCode));
            }

            return await _rolePermissionStore.EntitySet.AnyAsync(rp => rp.RoleId == roleEntity.RoleId && rp.PermissionId == permissionEntity.PermissionId);
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

            var user = await _userStore.EntitySet.SingleOrDefaultAsync(u => u.UserName == userName).WithCurrentCulture();
            if (user != null)
            {
                string userId = user.Id;
                IQueryable<TUserRole> userRoles = _userRoleStore.EntitySet.Where(ur => ur.UserId == userId);
                if (userRoles != null)
                {
                    foreach (var userRole in userRoles.ToList())
                    {
                        var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleId == userRole.RoleId).WithCurrentCulture();
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
            _userRoleStore = null;
            _roleStore = null;
            _rolePermissionStore = null;
            _permissionStore = null;
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

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
    public class InspurUserRoleStore<TUser> : InspurUserRoleStore<TUser, InspurIdentityRole, InspurIdentityUserRole, string>
        where TUser : class, IInspurUser<string>
    {
        public InspurUserRoleStore()
            : this(new InspurIdentityDbContext())
        {
            DisposeContext = true;
        }

        public InspurUserRoleStore(DbContext context) : base(context)
        {

        }
    }

    public class InspurUserRoleStore<TUser, TRole, TUserRole, TKey> : IInspurQueryableUserRoleStore<TUser, TUserRole, TKey>
        where TUser : class, IInspurUser<TKey>
        where TRole : class, IInspurRole<TKey>
        where TUserRole : class, IInspurUserRole<TKey>, new()

    {
        private bool _disposed;
        private EntityStore<TUser> _userStore;
        private EntityStore<TRole> _roleStore;
        private EntityStore<TUserRole> _userRoleStore;

        public InspurUserRoleStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Context = context;
            _userStore = new EntityStore<TUser>(context);
            _roleStore = new EntityStore<TRole>(context);
            _userRoleStore = new EntityStore<TUserRole>(context);
        }

        public DbContext Context { get; private set; }
        public bool DisposeContext { get; set; }

        public IQueryable<TUserRole> UserRoles
        {
            get { return _userRoleStore.EntitySet; }
        }

        //public async Task CreateAsync(TUserRole userRole)
        //{
        //    ThrowIfDisposed();
        //    if (userRole == null)
        //    {
        //        throw new ArgumentNullException("userRole");
        //    }

        //    _userRoleStore.Create(userRole);
        //    await Context.SaveChangesAsync().WithCurrentCulture();
        //}

        //public async Task UpdateAsync(TUserRole userRole)
        //{
        //    ThrowIfDisposed();
        //    if (userRole == null)
        //    {
        //        throw new ArgumentNullException("userRole");
        //    }

        //    _userRoleStore.Update(userRole);
        //    await Context.SaveChangesAsync().WithCurrentCulture();
        //}

        //public async Task DeleteAsync(TUserRole userRole)
        //{
        //    ThrowIfDisposed();
        //    if (userRole == null)
        //    {
        //        throw new ArgumentNullException("userRole");
        //    }

        //    _userRoleStore.Delete(userRole);
        //    await Context.SaveChangesAsync().WithCurrentCulture();
        //}

        public async Task<IList<TUserRole>> FindUserRolesByUserId(TKey userId)
        {
            ThrowIfDisposed();
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.UserId.Equals(userId)
                        select userRole;

            return await query.ToListAsync().WithCurrentCulture();
        }

        public async Task<IList<TUserRole>> FindUserRolesByRoleId(TKey roleId)
        {
            ThrowIfDisposed();
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.RoleId.Equals(roleId)
                        select userRole;

            return await query.ToListAsync().WithCurrentCulture();
        }

        public async Task<bool> IsUserRoleExisted(TKey userId, TKey roleId)
        {
            ThrowIfDisposed();
            return await _userRoleStore.EntitySet.AnyAsync(ur => ur.UserId.Equals(userId) && ur.RoleId.Equals(roleId));
        }

        /// <summary>
        ///     Add a user to a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public async Task AddToRoleAsync(TUser user, string roleCode, bool onlyAllowSingleRoles = true)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "RoleNotFound", roleCode));
            }

            if (onlyAllowSingleRoles)
            {
                var oldUr = _userRoleStore.EntitySet.SingleOrDefault(ur => ur.UserId.Equals(user.Id));
                if (oldUr != null)
                {
                    _userRoleStore.Delete(oldUr);
                }
            }

            var newUr = new TUserRole() { UserId = user.Id, RoleId = roleEntity.RoleId };
            _userRoleStore.Create(newUr);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Remove a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public async Task RemoveFromRoleAsync(TUser user, string roleCode)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper().Equals(roleCode.ToUpper())).WithCurrentCulture();
            if (roleEntity != null)
            {
                var roleId = roleEntity.RoleId;
                var userId = user.Id;
                var userRole = await _userRoleStore.EntitySet.FirstOrDefaultAsync(r => roleId.Equals(r.RoleId) && r.UserId.Equals(userId)).WithCurrentCulture();
                if (userRole != null)
                {
                    _userRoleStore.Delete(userRole);
                    await Context.SaveChangesAsync().WithCurrentCulture();
                }
            }
        }

        public async Task<IList<TUserRole>> GetUserRolesAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.UserId.Equals(user.Id)
                        select userRole;


            return await query.ToListAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Get the names of the roles a user is a member of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetRoleCodesAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userId = user.Id;
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.UserId.Equals(userId)
                        join role in _roleStore.DbEntitySet on userRole.RoleId equals role.RoleId
                        select role.RoleCode;

            return await query.ToListAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Returns true if the user is in the named role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public async Task<bool> IsInRoleAsync(TUser user, string roleCode)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper().Equals(roleCode.ToUpper())).WithCurrentCulture();
            if (role != null)
            {
                var userId = user.Id;
                var roleId = role.RoleId;
                return await _userRoleStore.EntitySet.AnyAsync(ur => ur.RoleId.Equals(roleId) && ur.UserId.Equals(userId)).WithCurrentCulture();
            }

            return false;
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
            _userRoleStore = null;
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

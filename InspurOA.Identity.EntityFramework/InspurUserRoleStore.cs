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
    public class InspurUserRoleStore<TUser> : InspurUserRoleStore<TUser, InspurIdentityRole, InspurIdentityUserRole>,
        IInspurUserRoleStore<TUser, InspurIdentityUserRole>
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

    public class InspurUserRoleStore<TUser, TRole, TUserRole> : IInspurQueryableUserRoleStore<TUser, TUserRole, string>
        where TUser : class, IInspurUser<string>
        where TRole : class, IInspurRole<string>
        where TUserRole : class, IInspurUserRole<string>, new()
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

        public async Task<IList<TUserRole>> FindUserRolesByUserId(string userId)
        {
            ThrowIfDisposed();
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.UserId == userId
                        select userRole;

            return await query.ToListAsync().WithCurrentCulture();
        }

        public async Task<IList<TUserRole>> FindUserRolesByRoleId(string roleId)
        {
            ThrowIfDisposed();
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.RoleId == roleId
                        select userRole;

            return await query.ToListAsync().WithCurrentCulture();
        }

        public async Task<bool> IsUserRoleExisted(string userId, string roleId)
        {
            ThrowIfDisposed();
            return await _userRoleStore.EntitySet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        /// <summary>
        ///     Add a user to a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public async Task AddToRoleAsync(string userId, string roleCode, bool onlyAllowSingleRoles = true)
        {
            ThrowIfDisposed();
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
                var oldUr = _userRoleStore.EntitySet.SingleOrDefault(ur => ur.UserId == userId);
                if (oldUr != null)
                {
                    _userRoleStore.Delete(oldUr);
                }
            }

            var newUr = new TUserRole() { UserId = userId, RoleId = roleEntity.RoleId };
            _userRoleStore.Create(newUr);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Remove a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public async Task RemoveFromRoleAsync(string userId, string roleCode)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var roleEntity = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (roleEntity != null)
            {
                var roleId = roleEntity.RoleId;
                var userRole = await _userRoleStore.EntitySet.FirstOrDefaultAsync(r => roleId == r.RoleId && r.UserId == userId).WithCurrentCulture();
                if (userRole != null)
                {
                    _userRoleStore.Delete(userRole);
                    await Context.SaveChangesAsync().WithCurrentCulture();
                }
            }
        }

        public async Task RemoveUserFromUserRoleAsync(string userId)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "userId");
            }

            var userRoles = _userRoleStore.EntitySet.Where(ur => ur.UserId == userId);
            if (userRoles != null)
            {
                foreach (var userRole in userRoles.ToList())
                {
                    _userRoleStore.Delete(userRole);
                }

                await Context.SaveChangesAsync().WithCurrentCulture();
            }
        }

        public async Task RemoveRoleFromUserRoleAsync(string roleId)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleId");
            }

            var userRoles = _userRoleStore.EntitySet.Where(ur => ur.RoleId == roleId);
            if (userRoles != null)
            {
                foreach (var userRole in userRoles.ToList())
                {
                    _userRoleStore.Delete(userRole);
                }

                await Context.SaveChangesAsync().WithCurrentCulture();
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
                        where userRole.UserId == user.Id
                        select userRole;


            return await query.ToListAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Get the names of the roles a user is a member of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetRoleCodesAsync(string userId)
        {
            ThrowIfDisposed();
            var query = from userRole in _userRoleStore.EntitySet
                        where userRole.UserId == userId
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
        public async Task<bool> IsInRoleAsync(string userId, string roleCode)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(roleCode))
            {
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleCode");
            }

            var role = await _roleStore.DbEntitySet.SingleOrDefaultAsync(r => r.RoleCode.ToUpper() == roleCode.ToUpper()).WithCurrentCulture();
            if (role != null)
            {
                var roleId = role.RoleId;
                return await _userRoleStore.EntitySet.AnyAsync(ur => ur.RoleId == roleId && ur.UserId == userId).WithCurrentCulture();
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

using InspurOA.Identity.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    /// <summary>
    ///     EntityFramework based implementation
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public class InspurRoleStore : InspurRoleStore<InspurIdentityRole, InspurIdentityUserRole, InspurIdentityRolePermission>,
        IInspurRoleStore<InspurIdentityRole>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public InspurRoleStore()
            : base(new InspurIdentityDbContext())
        {
            DisposeContext = true;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context"></param>
        public InspurRoleStore(DbContext context) : base(context)
        {
        }
    }

    /// <summary>
    ///     EntityFramework based implementation
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    public class InspurRoleStore<TRole, TUserRole, TRolePermission> : IInspurQueryableRoleStore<TRole>
        where TRole : InspurIdentityRole<string, TUserRole, TRolePermission>, new()
        where TUserRole : InspurIdentityUserRole<string>, new()
        where TRolePermission : InspurIdentityRolePermission<string>, new()
    {
        private bool _disposed;
        private EntityStore<TRole> _roleStore;

        /// <summary>
        ///     Constructor which takes a db context and wires up the stores with default instances using the context
        /// </summary>
        /// <param name="context"></param>
        public InspurRoleStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            _roleStore = new EntityStore<TRole>(context);
        }

        /// <summary>
        ///     Context for the store
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        ///     If true will call dispose on the DbContext during Dipose
        /// </summary>
        public bool DisposeContext { get; set; }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<TRole> FindByIdAsync(string roleId)
        {
            ThrowIfDisposed();
            return _roleStore.GetByIdAsync(roleId);
        }

        /// <summary>
        ///     Find a role by name
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public Task<TRole> FindByCodeAsync(string roleCode)
        {
            ThrowIfDisposed();
            return _roleStore.EntitySet.FirstOrDefaultAsync(u => u.RoleCode.ToUpper() == roleCode.ToUpper());
        }

        /// <summary>
        ///     Insert an entity
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task CreateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Create(role);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Mark an entity for deletion
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task DeleteAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Delete(role);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Update an entity
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task UpdateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Update(role);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        ///     Returns an IQueryable of users
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get { return _roleStore.EntitySet; }
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
            _roleStore = null;
        }
    }
}

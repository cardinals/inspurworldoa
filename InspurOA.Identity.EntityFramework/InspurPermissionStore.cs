using InspurOA.Identity.Core;
using InspurOA.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurPermissionStore : InspurPermissionStore<InspurIdentityPermission>,
        IInspurPermissionStore<InspurIdentityPermission>
    {
        public InspurPermissionStore(DbContext context) : base(context)
        {

        }

        public InspurPermissionStore() : this(new InspurIdentityDbContext())
        {
            DisposeContext = true;
        }
    }

    public class InspurPermissionStore<TPermission> : IInspurQueryablePermissionStore<TPermission, string>
        where TPermission : InspurIdentityPermission<string>, new()
    {
        private bool _disposed;
        private EntityStore<TPermission> _permissionStore;

        public InspurPermissionStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Context = context;
            _permissionStore = new EntityStore<TPermission>(context);
        }

        public DbContext Context { get; private set; }

        public bool DisposeContext { get; set; }

        public Task<TPermission> FindByIdAsync(string permissionId)
        {
            ThrowIfDisposed();
            return _permissionStore.GetByIdAsync(permissionId);
        }

        public Task<TPermission> FindByCodeAsync(string permissionCode)
        {
            ThrowIfDisposed();
            return _permissionStore.EntitySet.FirstOrDefaultAsync(p => permissionCode.ToUpper() == permissionCode.ToUpper());
        }

        public async Task CreateAsync(TPermission permission)
        {
            try
            {
                ThrowIfDisposed();
                if (permission == null)
                {
                    throw new ArgumentNullException("permission");
                }

                _permissionStore.Create(permission);
                await Context.SaveChangesAsync().WithCurrentCulture();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteAsync(TPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            _permissionStore.Delete(permission);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public async Task UpdateAsync(TPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            _permissionStore.Update(permission);
            await Context.SaveChangesAsync().WithCurrentCulture();
        }

        public IQueryable<TPermission> Permissions
        {
            get { return _permissionStore.EntitySet; }
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
            _permissionStore = null;
        }
    }
}

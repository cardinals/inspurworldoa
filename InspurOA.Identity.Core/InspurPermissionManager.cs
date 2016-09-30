using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public class InspurPermissionManager<TPermission> : InspurPermissionManager<TPermission, string>
        where TPermission : class, IInspurPermission<string>
    {
        public InspurPermissionManager(IInspurPermissionStore<TPermission, string> store)
            : base(store)
        {

        }
    }

    public class InspurPermissionManager<TPermission, TKey> : IDisposable
        where TPermission : class, IInspurPermission<TKey>
        where TKey : IEquatable<TKey>
    {
        private bool _disposed;
        private IIdentityValidator<TPermission> _permissionValidator;

        public InspurPermissionManager(IInspurPermissionStore<TPermission, TKey> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            Store = store;
            InspurPermissionValidator = new InspurPermissionValidator<TPermission, TKey>(this);
        }

        protected internal IInspurPermissionStore<TPermission, TKey> Store;

        public IQueryable<TPermission> Permissions
        {
            get
            {
                var queryableStore = Store as IInspurQueryablePermissionStore<TPermission, TKey>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException(InspurResources.StoreNotIQueryablePemrissionStore);
                }

                return queryableStore.Permissions;
            }
        }

        public IIdentityValidator<TPermission> InspurPermissionValidator
        {
            get
            {
                ThrowIfDisposed();
                return _permissionValidator;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _permissionValidator = value;
            }
        }

        public async Task<IdentityResult> CreateAsync(TPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            var result =await InspurPermissionValidator.ValidateAsync(permission);
            if (!result.Succeeded)
            {
                return result;
            }

            await Store.CreateAsync(permission).WithCurrentCulture();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            var result = await InspurPermissionValidator.ValidateAsync(permission);
            if (!result.Succeeded)
            {
                return result;
            }

            await Store.UpdateAsync(permission);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TPermission permission)
        {
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            await Store.DeleteAsync(permission);
            return IdentityResult.Success;
        }

        public Task<TPermission> FindByIdAsync(TKey permissionId)
        {
            ThrowIfDisposed();
            if (permissionId == null || permissionId.Equals(""))
            {
                throw new ArgumentNullException("permissionId");
            }

            return Store.FindByIdAsync(permissionId);
        }

        public Task<TPermission> FindByCodeAsync(string permissionCode)
        {
            ThrowIfDisposed();
            if (permissionCode == null || permissionCode.Equals(""))
            {
                throw new ArgumentNullException("permissionId");
            }

            return Store.FindByCodeAsync(permissionCode);
        }

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

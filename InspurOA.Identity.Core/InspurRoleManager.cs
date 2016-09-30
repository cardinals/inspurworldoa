using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public class InspurRoleManager<TRole> : InspurRoleManager<TRole, string>
        where TRole : class, IInspurRole<string>
    {
        public InspurRoleManager(IInspurRoleStore<TRole, string> store)
            : base(store)
        {

        }
    }


    public class InspurRoleManager<TRole, TKey> : IDisposable
        where TRole : class, IInspurRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private bool _disposed;
        private IIdentityValidator<TRole> _roleValidator;

        public InspurRoleManager(IInspurRoleStore<TRole, TKey> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            Store = store;
            InspurRoleValidator = new InspurRoleValidator<TRole, TKey>(this);
        }

        protected internal IInspurRoleStore<TRole, TKey> Store;

        public IQueryable<TRole> Roles
        {
            get
            {
                var quertableStore = Store as IInspurQueryableRoleStore<TRole, TKey>;
                if (quertableStore == null)
                {
                    throw new NotSupportedException(InspurResources.StoreNotIQueryableRoleStore);
                }

                return quertableStore.Roles;
            }
        }

        public IIdentityValidator<TRole> InspurRoleValidator
        {
            get
            {
                ThrowIfDisposed();
                return _roleValidator;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _roleValidator = value;
            }
        }

        public async Task<IdentityResult> CreateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var result = await InspurRoleValidator.ValidateAsync(role).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }

            await Store.CreateAsync(role).WithCurrentCulture();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var result = await InspurRoleValidator.ValidateAsync(role).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }

            await Store.UpdateAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await Store.DeleteAsync(role);
            return IdentityResult.Success;
        }

        public async Task<TRole> FindByIdAsync(TKey roleId)
        {
            ThrowIfDisposed();
            return await Store.FindByIdAsync(roleId);
        }

        public async Task<TRole> FindByCodeAsync(string roleCode)
        {
            ThrowIfDisposed();
            return await Store.FindByCodeAsync(roleCode);
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

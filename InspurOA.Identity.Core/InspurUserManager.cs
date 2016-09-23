using InspurOA.Identity.Core;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     InspurUserManager for users where the primary key for the User is of type string
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class InspurUserManager<TUser> : InspurUserManager<TUser, string>
        where TUser : class, IInspurUser<string>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store"></param>
        public InspurUserManager(IInspurUserStore<TUser> store)
            : base(store)
        {
        }
    }

    /// <summary>
    ///     Exposes user related api which will automatically save changes to the UserStore
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class InspurUserManager<TUser, TKey> : IDisposable
        where TUser : class, IInspurUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<string, IInspurUserTokenProvider<TUser, TKey>> _factors =
            new Dictionary<string, IInspurUserTokenProvider<TUser, TKey>>();

        private IInspurClaimsIdentityFactory<TUser, TKey> _claimsFactory;
        private TimeSpan _defaultLockout = TimeSpan.Zero;
        private bool _disposed;
        private IPasswordHasher _passwordHasher;
        private IIdentityValidator<string> _passwordValidator;
        private IIdentityValidator<TUser> _userValidator;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The IUserStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
        public InspurUserManager(IInspurUserStore<TUser, TKey> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            Store = store;
            InspurUserValidator = new InspurUserValidator<TUser, TKey>(this);
            PasswordValidator = new MinimumLengthValidator(6);
            PasswordHasher = new PasswordHasher();
            ClaimsIdentityFactory = new InspurClaimsIdentityFactory<TUser, TKey>();
        }

        /// <summary>
        ///     Persistence abstraction that the InspurUserManager operates against
        /// </summary>
        protected internal IInspurUserStore<TUser, TKey> Store { get; set; }

        /// <summary>
        ///     Used to hash/verify passwords
        /// </summary>
        public IPasswordHasher PasswordHasher
        {
            get
            {
                ThrowIfDisposed();
                return _passwordHasher;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _passwordHasher = value;
            }
        }

        /// <summary>
        ///     Used to validate users before changes are saved
        /// </summary>
        public IIdentityValidator<TUser> InspurUserValidator
        {
            get
            {
                ThrowIfDisposed();
                return _userValidator;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _userValidator = value;
            }
        }

        /// <summary>
        ///     Used to validate passwords before persisting changes
        /// </summary>
        public IIdentityValidator<string> PasswordValidator
        {
            get
            {
                ThrowIfDisposed();
                return _passwordValidator;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _passwordValidator = value;
            }
        }

        /// <summary>
        ///     Used to create claims identities from users
        /// </summary>
        public IInspurClaimsIdentityFactory<TUser, TKey> ClaimsIdentityFactory
        {
            get
            {
                ThrowIfDisposed();
                return _claimsFactory;
            }
            set
            {
                ThrowIfDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _claimsFactory = value;
            }
        }

        /// <summary>
        ///     Used to send email
        /// </summary>
        public IIdentityMessageService EmailService { get; set; }

        /// <summary>
        ///     Used to send a sms message
        /// </summary>
        public IIdentityMessageService SmsService { get; set; }

        /// <summary>
        ///     Used for generating reset password and confirmation tokens
        /// </summary>
        public IInspurUserTokenProvider<TUser, TKey> UserTokenProvider { get; set; }

        /// <summary>
        ///     If true, will enable user lockout when users are created
        /// </summary>
        public bool UserLockoutEnabledByDefault { get; set; }

        /// <summary>
        ///     Number of access attempts allowed before a user is locked out (if lockout is enabled)
        /// </summary>
        public int MaxFailedAccessAttemptsBeforeLockout { get; set; }

        /// <summary>
        ///     Default amount of time that a user is locked out for after MaxFailedAccessAttemptsBeforeLockout is reached
        /// </summary>
        public TimeSpan DefaultAccountLockoutTimeSpan
        {
            get { return _defaultLockout; }
            set { _defaultLockout = value; }
        }

        /// <summary>
        ///     Returns true if the store is an IUserTwoFactorStore
        /// </summary>
        public virtual bool SupportsUserTwoFactor
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserTwoFactorStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IInspurUserPasswordStore
        /// </summary>
        public virtual bool SupportsUserPassword
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserPasswordStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IUserSecurityStore
        /// </summary>
        public virtual bool SupportsUserSecurityStamp
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserSecurityStampStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IInspurUserRoleStore
        /// </summary>
        public virtual bool SupportsUserRole
        {
            get
            {
                ThrowIfDisposed();
                //return Store is IInspurUserRoleStore<TUser, TKey>;
                return true;
            }
        }

        ///// <summary>
        /////     Returns true if the store is an IInspurUserLoginStore
        ///// </summary>
        //public virtual bool SupportsUserLogin
        //{
        //    get
        //    {
        //        ThrowIfDisposed();
        //        return Store is IInspurUserLoginStore<TUser, TKey>;
        //    }
        //}

        /// <summary>
        ///     Returns true if the store is an IInspurUserEmailStore
        /// </summary>
        public virtual bool SupportsUserEmail
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserEmailStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IInspurUserPhoneNumberStore
        /// </summary>
        public virtual bool SupportsUserPhoneNumber
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserPhoneNumberStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IInspurUserClaimStore
        /// </summary>
        public virtual bool SupportsUserClaim
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserClaimStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IInspurUserLockoutStore
        /// </summary>
        public virtual bool SupportsUserLockout
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurUserLockoutStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns true if the store is an IQueryableUserStore
        /// </summary>
        public virtual bool SupportsQueryableUsers
        {
            get
            {
                ThrowIfDisposed();
                return Store is IInspurQueryableUserStore<TUser, TKey>;
            }
        }

        /// <summary>
        ///     Returns an IQueryable of users if the store is an IQueryableUserStore
        /// </summary>
        public virtual IQueryable<TUser> Users
        {
            get
            {
                var queryableStore = Store as IInspurQueryableUserStore<TUser, TKey>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException(InspurResources.StoreNotIQueryableUserStore);
                }
                return queryableStore.Users;
            }
        }

        /// <summary>
        /// Maps the registered two-factor authentication providers for users by their id
        /// </summary>
        public IDictionary<string, IInspurUserTokenProvider<TUser, TKey>> TwoFactorProviders
        {
            get { return _factors; }
        }

        /// <summary>
        ///     Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Creates a ClaimsIdentity representing the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public virtual Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return ClaimsIdentityFactory.CreateAsync(this, user, authenticationType);
        }

        /// <summary>
        ///     Create a user with no password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            var result = await InspurUserValidator.ValidateAsync(user).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            if (UserLockoutEnabledByDefault && SupportsUserLockout)
            {
                await GetUserLockoutStore().SetLockoutEnabledAsync(user, true).WithCurrentCulture();
            }
            await Store.CreateAsync(user).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        ///     Update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var result = await InspurUserValidator.ValidateAsync(user).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            await Store.UpdateAsync(user).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        ///     Delete a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            await Store.DeleteAsync(user).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        ///     Find a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByIdAsync(TKey userId)
        {
            ThrowIfDisposed();
            return Store.FindByIdAsync(userId);
        }

        /// <summary>
        ///     Find a user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            return Store.FindByNameAsync(userName);
        }

        // IInspurUserPasswordStore methods
        private IInspurUserPasswordStore<TUser, TKey> GetPasswordStore()
        {
            var cast = Store as IInspurUserPasswordStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserPasswordStore);
            }
            return cast;
        }

        /// <summary>
        ///     Create a user with the given password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            var result = await UpdatePassword(passwordStore, user, password).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            return await CreateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Return a user with the specified username and password or null if there is no match.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<TUser> FindAsync(string userName, string password)
        {
            ThrowIfDisposed();
            var user = await FindByNameAsync(userName).WithCurrentCulture();
            if (user == null)
            {
                return null;
            }
            return await CheckPasswordAsync(user, password).WithCurrentCulture() ? user : null;
        }

        /// <summary>
        ///     Returns true if the password is valid for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            if (user == null)
            {
                return false;
            }
            return await VerifyPasswordAsync(passwordStore, user, password).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns true if the user has a password
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasPasswordAsync(TKey userId)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await passwordStore.HasPasswordAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Add a user password only if one does not already exist
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddPasswordAsync(TKey userId, string password)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            var hash = await passwordStore.GetPasswordHashAsync(user).WithCurrentCulture();
            if (hash != null)
            {
                return new IdentityResult(InspurResources.UserAlreadyHasPassword);
            }
            var result = await UpdatePassword(passwordStore, user, password).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Change a user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ChangePasswordAsync(TKey userId, string currentPassword,
            string newPassword)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (await VerifyPasswordAsync(passwordStore, user, currentPassword).WithCurrentCulture())
            {
                var result = await UpdatePassword(passwordStore, user, newPassword).WithCurrentCulture();
                if (!result.Succeeded)
                {
                    return result;
                }
                return await UpdateAsync(user).WithCurrentCulture();
            }
            return IdentityResult.Failed(InspurResources.PasswordMismatch);
        }

        /// <summary>
        ///     Remove a user's password
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> RemovePasswordAsync(TKey userId)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await passwordStore.SetPasswordHashAsync(user, null).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        protected virtual async Task<IdentityResult> UpdatePassword(IInspurUserPasswordStore<TUser, TKey> passwordStore,
            TUser user, string newPassword)
        {
            var result = await PasswordValidator.ValidateAsync(newPassword).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            await
                passwordStore.SetPasswordHashAsync(user, PasswordHasher.HashPassword(newPassword)).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return IdentityResult.Success;
        }

        /// <summary>
        ///     By default, retrieves the hashed password from the user store and calls PasswordHasher.VerifyHashPassword
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual async Task<bool> VerifyPasswordAsync(IInspurUserPasswordStore<TUser, TKey> store, TUser user,
            string password)
        {
            var hash = await store.GetPasswordHashAsync(user).WithCurrentCulture();
            return PasswordHasher.VerifyHashedPassword(hash, password) != PasswordVerificationResult.Failed;
        }

        // IUserSecurityStampStore methods
        private IInspurUserSecurityStampStore<TUser, TKey> GetSecurityStore()
        {
            var cast = Store as IInspurUserSecurityStampStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserSecurityStampStore);
            }
            return cast;
        }

        /// <summary>
        ///     Returns the current security stamp for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<string> GetSecurityStampAsync(TKey userId)
        {
            ThrowIfDisposed();
            var securityStore = GetSecurityStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await securityStore.GetSecurityStampAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Generate a new security stamp for a user, used for SignOutEverywhere functionality
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> UpdateSecurityStampAsync(TKey userId)
        {
            ThrowIfDisposed();
            var securityStore = GetSecurityStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await securityStore.SetSecurityStampAsync(user, NewSecurityStamp()).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Generate a password reset token for the user using the UserTokenProvider
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<string> GeneratePasswordResetTokenAsync(TKey userId)
        {
            ThrowIfDisposed();
            return GenerateUserTokenAsync("ResetPassword", userId);
        }

        /// <summary>
        ///     Reset a user's password using a reset password token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ResetPasswordAsync(TKey userId, string token, string newPassword)
        {
            ThrowIfDisposed();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            // Make sure the token is valid and the stamp matches
            if (!await VerifyUserTokenAsync(userId, "ResetPassword", token).WithCurrentCulture())
            {
                return IdentityResult.Failed(InspurResources.InvalidToken);
            }
            var passwordStore = GetPasswordStore();
            var result = await UpdatePassword(passwordStore, user, newPassword).WithCurrentCulture();
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateAsync(user).WithCurrentCulture();
        }

        // Update the security stamp if the store supports it
        internal async Task UpdateSecurityStampInternal(TUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp()).WithCurrentCulture();
            }
        }

        private static string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }

        // IInspurUserLoginStore methods
        private IInspurUserLoginStore<TUser, TKey> GetLoginStore()
        {
            var cast = Store as IInspurUserLoginStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserLoginStore);
            }
            return cast;
        }

        /// <summary>
        ///     Returns the user associated with this login
        /// </summary>
        /// <returns></returns>
        public virtual Task<TUser> FindAsync(InspurUserLoginInfo login)
        {
            ThrowIfDisposed();
            return GetLoginStore().FindAsync(login);
        }

        /// <summary>
        ///     Remove a user login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> RemoveLoginAsync(TKey userId, InspurUserLoginInfo login)
        {
            ThrowIfDisposed();
            var loginStore = GetLoginStore();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await loginStore.RemoveLoginAsync(user, login).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Associate a login with a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddLoginAsync(TKey userId, InspurUserLoginInfo login)
        {
            ThrowIfDisposed();
            var loginStore = GetLoginStore();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            var existingUser = await FindAsync(login).WithCurrentCulture();
            if (existingUser != null)
            {
                return IdentityResult.Failed(InspurResources.ExternalLoginExists);
            }
            await loginStore.AddLoginAsync(user, login).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Gets the logins for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IList<InspurUserLoginInfo>> GetLoginsAsync(TKey userId)
        {
            ThrowIfDisposed();
            var loginStore = GetLoginStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await loginStore.GetLoginsAsync(user).WithCurrentCulture();
        }

        // IInspurUserClaimStore methods
        private IInspurUserClaimStore<TUser, TKey> GetClaimStore()
        {
            var cast = Store as IInspurUserClaimStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserClaimStore);
            }
            return cast;
        }

        /// <summary>
        ///     Add a user claim
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddClaimAsync(TKey userId, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await claimStore.AddClaimAsync(user, claim).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Remove a user claim
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> RemoveClaimAsync(TKey userId, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await claimStore.RemoveClaimAsync(user, claim).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Get a users's claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync(TKey userId)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await claimStore.GetClaimsAsync(user).WithCurrentCulture();
        }

        private IInspurUserStore<TUser, TKey> GetUserStore()
        {
            var cast = Store as IInspurUserStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserStore);
            }
            return cast;
        }
     

        // IInspurUserEmailStore methods
        internal IInspurUserEmailStore<TUser, TKey> GetEmailStore()
        {
            var cast = Store as IInspurUserEmailStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserEmailStore);
            }
            return cast;
        }

        /// <summary>
        ///     Get a user's email
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<string> GetEmailAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetEmailAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Set a user's email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> SetEmailAsync(TKey userId, string email)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await store.SetEmailAsync(user, email).WithCurrentCulture();
            await store.SetEmailConfirmedAsync(user, false).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Find a user by his email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }
            return store.FindByEmailAsync(email);
        }

        /// <summary>
        ///     Get the email confirmation token for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<string> GenerateEmailConfirmationTokenAsync(TKey userId)
        {
            ThrowIfDisposed();
            return GenerateUserTokenAsync("Confirmation", userId);
        }

        /// <summary>
        ///     Confirm the user's email with confirmation token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ConfirmEmailAsync(TKey userId, string token)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!await VerifyUserTokenAsync(userId, "Confirmation", token).WithCurrentCulture())
            {
                return IdentityResult.Failed(InspurResources.InvalidToken);
            }
            await store.SetEmailConfirmedAsync(user, true).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns true if the user's email has been confirmed
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsEmailConfirmedAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetEmailConfirmedAsync(user).WithCurrentCulture();
        }

        // IInspurUserPhoneNumberStore methods
        internal IInspurUserPhoneNumberStore<TUser, TKey> GetPhoneNumberStore()
        {
            var cast = Store as IInspurUserPhoneNumberStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserPhoneNumberStore);
            }
            return cast;
        }

        /// <summary>
        ///     Get a user's phoneNumber
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<string> GetPhoneNumberAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetPhoneNumberStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetPhoneNumberAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Set a user's phoneNumber
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> SetPhoneNumberAsync(TKey userId, string phoneNumber)
        {
            ThrowIfDisposed();
            var store = GetPhoneNumberStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await store.SetPhoneNumberAsync(user, phoneNumber).WithCurrentCulture();
            await store.SetPhoneNumberConfirmedAsync(user, false).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Set a user's phoneNumber with the verification token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ChangePhoneNumberAsync(TKey userId, string phoneNumber, string token)
        {
            ThrowIfDisposed();
            var store = GetPhoneNumberStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (await VerifyChangePhoneNumberTokenAsync(userId, token, phoneNumber).WithCurrentCulture())
            {
                await store.SetPhoneNumberAsync(user, phoneNumber).WithCurrentCulture();
                await store.SetPhoneNumberConfirmedAsync(user, true).WithCurrentCulture();
                await UpdateSecurityStampInternal(user).WithCurrentCulture();
                return await UpdateAsync(user).WithCurrentCulture();
            }
            return IdentityResult.Failed(InspurResources.InvalidToken);
        }

        /// <summary>
        ///     Returns true if the user's phone number has been confirmed
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsPhoneNumberConfirmedAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetPhoneNumberStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetPhoneNumberConfirmedAsync(user).WithCurrentCulture();
        }

        // Two factor APIS

        internal async Task<SecurityToken> CreateSecurityTokenAsync(TKey userId)
        {
            return
                new SecurityToken(Encoding.Unicode.GetBytes(await GetSecurityStampAsync(userId).WithCurrentCulture()));
        }

        /// <summary>
        ///     Generate a code that the user can use to change their phone number to a specific number
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public virtual async Task<string> GenerateChangePhoneNumberTokenAsync(TKey userId, string phoneNumber)
        {
            ThrowIfDisposed();
            return
                Rfc6238AuthenticationService.GenerateCode(await CreateSecurityTokenAsync(userId).WithCurrentCulture(), phoneNumber)
                    .ToString("D6", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Verify the code is valid for a specific user and for a specific phone number
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyChangePhoneNumberTokenAsync(TKey userId, string token, string phoneNumber)
        {
            ThrowIfDisposed();
            var securityToken = await CreateSecurityTokenAsync(userId).WithCurrentCulture();
            int code;
            if (securityToken != null && Int32.TryParse(token, out code))
            {
                return Rfc6238AuthenticationService.ValidateCode(securityToken, code, phoneNumber);
            }
            return false;
        }

        /// <summary>
        ///     Verify a user token with the specified purpose
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyUserTokenAsync(TKey userId, string purpose, string token)
        {
            ThrowIfDisposed();
            if (UserTokenProvider == null)
            {
                throw new NotSupportedException(InspurResources.NoTokenProvider);
            }
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            // Make sure the token is valid
            return await UserTokenProvider.ValidateAsync(purpose, token, this, user).WithCurrentCulture();
        }

        /// <summary>
        ///     Get a user token for a specific purpose
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<string> GenerateUserTokenAsync(string purpose, TKey userId)
        {
            ThrowIfDisposed();
            if (UserTokenProvider == null)
            {
                throw new NotSupportedException(InspurResources.NoTokenProvider);
            }
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await UserTokenProvider.GenerateAsync(purpose, this, user).WithCurrentCulture();
        }

        /// <summary>
        ///     Register a two factor authentication provider with the TwoFactorProviders mapping
        /// </summary>
        /// <param name="twoFactorProvider"></param>
        /// <param name="provider"></param>
        public virtual void RegisterTwoFactorProvider(string twoFactorProvider, IInspurUserTokenProvider<TUser, TKey> provider)
        {
            ThrowIfDisposed();
            if (twoFactorProvider == null)
            {
                throw new ArgumentNullException("twoFactorProvider");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            TwoFactorProviders[twoFactorProvider] = provider;
        }

        /// <summary>
        ///     Returns a list of valid two factor providers for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IList<string>> GetValidTwoFactorProvidersAsync(TKey userId)
        {
            ThrowIfDisposed();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            var results = new List<string>();
            foreach (var f in TwoFactorProviders)
            {
                if (await f.Value.IsValidProviderForUserAsync(this, user).WithCurrentCulture())
                {
                    results.Add(f.Key);
                }
            }
            return results;
        }

        /// <summary>
        ///     Verify a two factor token with the specified provider
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="twoFactorProvider"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider, string token)
        {
            ThrowIfDisposed();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!_factors.ContainsKey(twoFactorProvider))
            {
                throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, InspurResources.NoTwoFactorProvider,
                    twoFactorProvider));
            }
            // Make sure the token is valid
            var provider = _factors[twoFactorProvider];
            return await provider.ValidateAsync(twoFactorProvider, token, this, user).WithCurrentCulture();
        }

        /// <summary>
        ///     Get a token for a specific two factor provider
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="twoFactorProvider"></param>
        /// <returns></returns>
        public virtual async Task<string> GenerateTwoFactorTokenAsync(TKey userId, string twoFactorProvider)
        {
            ThrowIfDisposed();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!_factors.ContainsKey(twoFactorProvider))
            {
                throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, InspurResources.NoTwoFactorProvider,
                    twoFactorProvider));
            }
            return await _factors[twoFactorProvider].GenerateAsync(twoFactorProvider, this, user).WithCurrentCulture();
        }

        /// <summary>
        ///     Notify a user with a token using a specific two-factor authentication provider's Notify method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="twoFactorProvider"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> NotifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider,
            string token)
        {
            ThrowIfDisposed();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!_factors.ContainsKey(twoFactorProvider))
            {
                throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, InspurResources.NoTwoFactorProvider,
                    twoFactorProvider));
            }
            await _factors[twoFactorProvider].NotifyAsync(token, this, user).WithCurrentCulture();
            return IdentityResult.Success;
        }

        // IUserFactorStore methods
        internal IInspurUserTwoFactorStore<TUser, TKey> GetUserTwoFactorStore()
        {
            var cast = Store as IInspurUserTwoFactorStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserTwoFactorStore);
            }
            return cast;
        }

        /// <summary>
        ///     Get whether two factor authentication is enabled for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> GetTwoFactorEnabledAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserTwoFactorStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetTwoFactorEnabledAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Set whether a user has two factor authentication enabled
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> SetTwoFactorEnabledAsync(TKey userId, bool enabled)
        {
            ThrowIfDisposed();
            var store = GetUserTwoFactorStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await store.SetTwoFactorEnabledAsync(user, enabled).WithCurrentCulture();
            await UpdateSecurityStampInternal(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        // SMS/Email methods

        /// <summary>
        ///     Send an email to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual async Task SendEmailAsync(TKey userId, string subject, string body)
        {
            ThrowIfDisposed();
            if (EmailService != null)
            {
                var msg = new IdentityMessage
                {
                    Destination = await GetEmailAsync(userId).WithCurrentCulture(),
                    Subject = subject,
                    Body = body,
                };
                await EmailService.SendAsync(msg).WithCurrentCulture();
            }
        }

        /// <summary>
        ///     Send a user a sms message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendSmsAsync(TKey userId, string message)
        {
            ThrowIfDisposed();
            if (SmsService != null)
            {
                var msg = new IdentityMessage
                {
                    Destination = await GetPhoneNumberAsync(userId).WithCurrentCulture(),
                    Body = message
                };
                await SmsService.SendAsync(msg).WithCurrentCulture();
            }
        }

        // IInspurUserLockoutStore methods
        internal IInspurUserLockoutStore<TUser, TKey> GetUserLockoutStore()
        {
            var cast = Store as IInspurUserLockoutStore<TUser, TKey>;
            if (cast == null)
            {
                throw new NotSupportedException(InspurResources.StoreNotIUserLockoutStore);
            }
            return cast;
        }

        /// <summary>
        ///     Returns true if the user is locked out
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsLockedOutAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!await store.GetLockoutEnabledAsync(user).WithCurrentCulture())
            {
                return false;
            }
            var lockoutTime = await store.GetLockoutEndDateAsync(user).WithCurrentCulture();
            return lockoutTime >= DateTimeOffset.UtcNow;
        }

        /// <summary>
        ///     Sets whether lockout is enabled for this user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> SetLockoutEnabledAsync(TKey userId, bool enabled)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            await store.SetLockoutEnabledAsync(user, enabled).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns whether lockout is enabled for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> GetLockoutEnabledAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetLockoutEnabledAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns when the user is no longer locked out, dates in the past are considered as not being locked out
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<DateTimeOffset> GetLockoutEndDateAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetLockoutEndDateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Sets the when a user lockout ends
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> SetLockoutEndDateAsync(TKey userId, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            if (!await store.GetLockoutEnabledAsync(user).WithCurrentCulture())
            {
                return IdentityResult.Failed(InspurResources.LockoutNotEnabled);
            }
            await store.SetLockoutEndDateAsync(user, lockoutEnd).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        /// Increments the access failed count for the user and if the failed access account is greater than or equal
        /// to the MaxFailedAccessAttempsBeforeLockout, the user will be locked out for the next DefaultAccountLockoutTimeSpan
        /// and the AccessFailedCount will be reset to 0. This is used for locking out the user account.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AccessFailedAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            // If this puts the user over the threshold for lockout, lock them out and reset the access failed count
            var count = await store.IncrementAccessFailedCountAsync(user).WithCurrentCulture();
            if (count >= MaxFailedAccessAttemptsBeforeLockout)
            {
                await
                    store.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(DefaultAccountLockoutTimeSpan))
                        .WithCurrentCulture();
                await store.ResetAccessFailedCountAsync(user).WithCurrentCulture();
            }
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Resets the access failed count for the user to 0
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ResetAccessFailedCountAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }

            if (await GetAccessFailedCountAsync(user.Id).WithCurrentCulture() == 0)
            {
                return IdentityResult.Success;
            }

            await store.ResetAccessFailedCountAsync(user).WithCurrentCulture();
            return await UpdateAsync(user).WithCurrentCulture();
        }

        /// <summary>
        ///     Returns the number of failed access attempts for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<int> GetAccessFailedCountAsync(TKey userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, InspurResources.UserIdNotFound,
                    userId));
            }
            return await store.GetAccessFailedCountAsync(user).WithCurrentCulture();
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
    }
}

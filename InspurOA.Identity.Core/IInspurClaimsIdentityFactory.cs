using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    /// <summary>
    ///     Interface for creating a ClaimsIdentity from an IInspurUser
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IInspurClaimsIdentityFactory<TUser, TKey>
        where TUser : class, IInspurUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Create a ClaimsIdentity from an user using a UserManager
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> CreateAsync(InspurUserManager<TUser, TKey> manager, TUser user, string authenticationType);
    }

    /// <summary>
    ///     Interface for creating a ClaimsIdentity from a user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IInspurClaimsIdentityFactory<TUser> where TUser : class, IInspurUser
    {
        /// <summary>
        ///     Create a ClaimsIdentity from an user using a UserManager
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> CreateAsync(InspurUserManager<TUser> manager, TUser user, string authenticationType);
    }
}

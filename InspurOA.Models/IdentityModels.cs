using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspurOA.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class URole : IdentityRole
    {
        public string RoleName { get; set; }
    }

    public class Permission
    {
        public string PermissionId { get; set; }

        public string PermissionDescription { get; set; }
    }

    public class RolePermission
    {
        public string RoleId { get; set; }

        public string PermissionId { get; set; }
    }

    public class UserPermission
    {
        public string UserId { get; set; }

        public string PermissionId { get; set; }
    }   
}
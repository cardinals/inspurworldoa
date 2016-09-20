using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;

namespace InspurOA.DAL
{
    public class ApplicationDbContext : InspurIdentityDbContext<InspurUser>
    {
        public DbSet<InspurIdentityUserRole> UserRoles { get; set; }

        public DbSet<InspurIdentityRolePermission> RolePermissions { get; set; }

        public ApplicationDbContext()
            : base("ConnectionString", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}

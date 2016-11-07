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

        public DbSet<Resume> ResumeSet { get; set; }

        public DbSet<ResumeComment> ResumeCommentSet { get; set; }

        public DbSet<ProjectModel> ProjectSet { get; set; }

        public ApplicationDbContext()
            : base("ConnectionString", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Needed to ensure subclasses share the same table
            var user = modelBuilder.Entity<InspurUser>()
                .ToTable("UserInfo")
                .HasKey(u => u.Id);
            //user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            //user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            //user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            // CONSIDER: u.Email is Required if set on options?
            user.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<InspurIdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("UserRoles");

            //modelBuilder.Entity<TUserLogin>()
            //    .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
            //    .ToTable("AspNetUserLogins");

            //modelBuilder.Entity<TUserClaim>()
            //    .ToTable("AspNetUserClaims");

            var role = modelBuilder.Entity<InspurIdentityRole>()
                .ToTable("RoleInfo")
                .HasKey(r => r.RoleId);
            role.Property(r => r.RoleCode)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleCodeIndex") { IsUnique = true }));
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);


            modelBuilder.Entity<InspurIdentityRolePermission>()
            .HasKey(r => new { r.RoleId, r.PermissionId })
            .ToTable("RolePermissions");

            var permission = modelBuilder.Entity<InspurIdentityPermission>()
                 .ToTable("PermissionInfo")
                 .HasKey(p => p.PermissionId);
            permission.Property(p => p.PermissionCode)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("PermissionCodeIndex") { IsUnique = true }));

            modelBuilder.Entity<Resume>().ToTable("ResumeInfo");
            modelBuilder.Entity<ResumeComment>().ToTable("ResumeCommentInfo");
            modelBuilder.Entity<ProjectModel>().ToTable("ProjectInfo");
            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}

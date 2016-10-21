using InspurOA.Models;
using System.Data.Entity;

namespace InspurOA.DAL
{
    public class InspurDbContext : System.Data.Entity.DbContext
    {
        public InspurDbContext() : base("ConnectionString")
        {
        }

        public DbSet<Resume> ResumeSet { get; set; }

        public DbSet<ResumeComment> ResumeCommentSet { get; set; }

        public DbSet<ProjectModel> ProjectSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resume>().ToTable("ResumeInfo");
            modelBuilder.Entity<ResumeComment>().ToTable("ResumeCommentInfo");
            modelBuilder.Entity<ProjectModel>().ToTable("ProjectInfo");
            base.OnModelCreating(modelBuilder);
        }
    }
}

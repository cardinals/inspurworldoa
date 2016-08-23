using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.DAL
{
    public class ResumeDAL:DbContext 
    {
        public ResumeDAL() : base("ConnectionString")
        {
        }

        public DbSet<Resume> ResumeSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resume>().ToTable("ResumeInfo");
            base.OnModelCreating(modelBuilder);
        }
    }
}

namespace InspurOA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDb3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResumeInfo", "ProjectName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResumeInfo", "ProjectName");
        }
    }
}

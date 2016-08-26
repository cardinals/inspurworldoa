namespace InspurOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Roles", newName: "RoleInfo");
            RenameTable(name: "dbo.UserRoles", newName: "UserRole");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.UserRole", newName: "UserRoles");
            RenameTable(name: "dbo.RoleInfo", newName: "Roles");
        }
    }
}

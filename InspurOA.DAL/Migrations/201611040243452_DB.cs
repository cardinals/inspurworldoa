namespace InspurOA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PermissionInfo",
                c => new
                    {
                        PermissionId = c.String(nullable: false, maxLength: 128),
                        PermissionCode = c.String(nullable: false, maxLength: 256),
                        PermissionDescription = c.String(),
                    })
                .PrimaryKey(t => t.PermissionId)
                .Index(t => t.PermissionCode, unique: true, name: "PermissionCodeIndex");
            
            CreateTable(
                "dbo.ProjectInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProjectName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResumeCommentInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ResumeId = c.String(),
                        PostName = c.String(),
                        InterviewDate = c.String(),
                        InterviewFeedBack = c.String(),
                        FeedBackTime = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResumeInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PersonalInformation = c.String(),
                        CareerObjective = c.String(),
                        SelfAssessment = c.String(),
                        WorkExperience = c.String(),
                        ProjectExperience = c.String(),
                        Education = c.String(),
                        Certificates = c.String(),
                        HonorsandAwards = c.String(),
                        SchoolPractice = c.String(),
                        LanguageSkills = c.String(),
                        Training = c.String(),
                        ProfessionalSkills = c.String(),
                        FilePath = c.String(),
                        UploadTime = c.DateTime(nullable: false),
                        SourceSite = c.String(),
                        LanguageType = c.String(),
                        ProjectName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        PermissionId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PermissionId })
                .ForeignKey("dbo.RoleInfo", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoleInfo",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        RoleName = c.String(),
                        RoleCode = c.String(nullable: false, maxLength: 256),
                        RoleDescription = c.String(),
                    })
                .PrimaryKey(t => t.RoleId)
                .Index(t => t.RoleCode, unique: true, name: "RoleCodeIndex");
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.RoleInfo", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        UserName = c.String(nullable: false, maxLength: 256),
                        PasswordHash = c.String(),
                        Department = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.RoleInfo");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.RoleInfo");
            DropIndex("dbo.UserInfo", "UserNameIndex");
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.RoleInfo", "RoleCodeIndex");
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.PermissionInfo", "PermissionCodeIndex");
            DropTable("dbo.UserInfo");
            DropTable("dbo.UserRoles");
            DropTable("dbo.RoleInfo");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.ResumeInfo");
            DropTable("dbo.ResumeCommentInfo");
            DropTable("dbo.ProjectInfo");
            DropTable("dbo.PermissionInfo");
        }
    }
}

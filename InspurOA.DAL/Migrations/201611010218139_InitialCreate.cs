namespace InspurOA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ResumeInfo");
            DropTable("dbo.ResumeCommentInfo");
            DropTable("dbo.ProjectInfo");
        }
    }
}

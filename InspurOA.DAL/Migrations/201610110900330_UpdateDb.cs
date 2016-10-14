namespace InspurOA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDb : DbMigration
    {
        public override void Up()
        {
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
                        PostName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ResumeInfo");
        }
    }
}

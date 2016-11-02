using System;
using System.ComponentModel.DataAnnotations;

namespace InspurOA.Models
{
    public class Resume
    {
        public Resume()
        {
            PersonalInformation = string.Empty;
            CareerObjective = string.Empty;
            SelfAssessment = string.Empty;
            WorkExperience = string.Empty;
            ProjectExperience = string.Empty;
            Education = string.Empty;
            Certificates = string.Empty;
            HonorsandAwards = string.Empty;
            SchoolPractice = string.Empty;
            LanguageSkills = string.Empty;
            Training = string.Empty;
        }

        [Key]
        public string Id { get; set; }

        public string PersonalInformation { get; set; }

        public string CareerObjective { get; set; }

        public string SelfAssessment { get; set; }

        public string WorkExperience { get; set; }

        public string ProjectExperience { get; set; }

        public string Education { get; set; }

        public string Certificates { get; set; }

        public string HonorsandAwards { get; set; }

        public string SchoolPractice { get; set; }

        public string LanguageSkills { get; set; }

        public string Training { get; set; }

        public string ProfessionalSkills { get; set; }

        public string FilePath { get; set; }

        public DateTime UploadTime { get; set; }

        public string SourceSite { get; set; }

        public string LanguageType { get; set; }

        public string ProjectName { get; set; }
    }
}

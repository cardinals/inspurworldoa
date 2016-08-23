using System;
using System.ComponentModel.DataAnnotations;

namespace InspurOA.Models
{
    public class Resume
    {
        [Key]
        public string Guid { get; set; }

        [Display(Name ="个人信息")]
        public string PersonalInformation { get; set; }

        [Display(Name = "求职意向")]
        public string CareerObjective { get; set; }

        [Display(Name = "自我介绍")]
        public string SelfAssessment { get; set; }

        [Display(Name = "工作经历")]
        public string WorkExperience { get; set; }

        [Display(Name = "项目经历")]
        public string ProjectExperience { get; set; }

        [Display(Name = "教育经历")]
        public string Education { get; set; }

        [Display(Name = "证书")]
        public string Certificates { get; set; }

        [Display(Name = "在校学习情况")]
        public string HonorsandAwards { get; set; }

        [Display(Name = "在校实践")]
        public string SchoolPractice { get; set; }

        [Display(Name = "语言能力")]
        public string LanguageSkills { get; set; }

        [Display(Name = "培训经历")]
        public string Training { get; set; }

        [Display(Name = "专业技能")]
        public string ProfessionalSkills { get; set; }

        [Display(Name = "文件名称")]
        public string FilePath { get; set; }

        [Display(Name = "上传日期")]
        public DateTime UploadTime { get; set; }

        [Display(Name = "语言")]
        public ResumeLanguageType ResumeLanguageType { get; set; }

        [Display(Name = "来源")]
        public string SourceSite { get; set; }
    }
}

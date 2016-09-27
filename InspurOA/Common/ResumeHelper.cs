using InspurOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Common
{
    public class ResumeHelper
    {
        private static List<string> CNZLTitleArray = new List<string>() { "求职意向", "自我评价", "工作经历", "项目经历", "教育经历", "证书", "培训经历", "在校学习情况", "在校实践经历", "语言能力", "专业技能", "兴趣爱好" };
        private static List<string> ENZLTitleArray = new List<string>() { "CareerObjective", "Self-Assessment", "WorkExperience", "ProjectExperience", "Education", "Certifications", "Training", "HonorsandAwards", "在校实践经历", "LanguageSkills", "ProfessionalSkills" };
        private static List<string> CNWYTitleArray = new List<string>() { "求职意向", "自我评价", "工作经验", "项目经验", "教育经历", "所获奖项", "学生实践经验", "校内职务", "培训经历", "证书", "语言能力", "IT技能", "其他信息" };
        private static List<string> ENWYTitleArray = new List<string>() { "CareerObjective", "SelfAssessment", "WorkExperience", "ProjectExperience", "Education", "Certifications", "Training", "HonorsandAwards", "LanguageSkills", "ITSkills" };

        public static bool isZLTitle(string content)
        {
            return CNZLTitleArray.Contains(content) || ENZLTitleArray.Contains(content);
        }

        public static bool isWYTitle(string content)
        {
            return CNWYTitleArray.Contains(content) || ENWYTitleArray.Contains(content);
        }

        public static Resume getZLResumeEntity(List<string> resumeContent)
        {
            Resume resume = new Resume();

            string caption = string.Empty;
            StringBuilder contentBuilder = new StringBuilder();
            for (int i = 0; i < resumeContent.Count; i++)
            {
                string lineContent = resumeContent[i].Replace(" ", "").Replace(" ", "").Replace("\r", "").Replace("\a", "");
                if (isZLTitle(lineContent))
                {
                    if (contentBuilder.Length > 0)
                    {
                        setResumePropertyValue(ref resume, caption, contentBuilder.ToString());
                        contentBuilder.Clear();
                    }

                    caption = lineContent;
                }
                else if (lineContent.StartsWith("ID"))
                {
                    if (contentBuilder.Length > 0)
                    {
                        setResumePropertyValue(ref resume, caption, contentBuilder.ToString());
                        contentBuilder.Clear();
                    }

                    caption = "PersonalInformation";
                }
                else
                {
                    contentBuilder.Append(resumeContent[i].Replace("\a", ""));
                }

                if (i == resumeContent.Count - 1)
                {
                    setResumePropertyValue(ref resume, caption, contentBuilder.ToString());
                }
            }

            return resume;
        }

        public static Resume GetWYResumeEntity(List<string> resumeContent)
        {
            Resume resume = new Resume();

            string caption = "PersonalInformation";
            StringBuilder contentBuilder = new StringBuilder();
            for (int i = 0; i < resumeContent.Count; i++)
            {
                string lineContent = resumeContent[i].Replace(" ", "").Replace(" ", "").Replace("\r", "").Replace("\a", "");
                if (isWYTitle(lineContent))
                {
                    if (contentBuilder.Length > 0)
                    {
                        setResumePropertyValue(ref resume, caption, contentBuilder.ToString());
                        contentBuilder.Clear();
                    }

                    caption = lineContent;
                }
                else
                {
                    contentBuilder.Append(resumeContent[i].Replace("\a", ""));
                }

                if (i == resumeContent.Count - 1)
                {
                    setResumePropertyValue(ref resume, caption, contentBuilder.ToString());
                }
            }

            return resume;
        }


        public static void setResumePropertyValue(ref Resume resume, string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case "PersonalInformation":
                    resume.PersonalInformation = propertyValue;
                    break;
                case "求职意向":
                case "CareerObjective":
                case "SelfAssessment":
                    resume.CareerObjective = propertyValue;
                    break;
                case "自我评价":
                case "Self-Assessment":
                    resume.SelfAssessment = propertyValue;
                    break;
                case "工作经历":
                case "WorkExperience":
                case "工作经验":
                    resume.WorkExperience = propertyValue;
                    break;
                case "项目经历":
                case "ProjectExperience":
                case "项目经验":
                    resume.ProjectExperience = propertyValue;
                    break;
                case "教育经历":
                case "Education":
                    resume.Education = propertyValue;
                    break;
                case "证书":
                case "Certifications":
                    resume.Certificates = propertyValue;
                    break;
                case "在校学习情况":
                case "HonorsandAwards":
                    resume.HonorsandAwards = propertyValue;
                    break;
                case "在校实践经历":
                    //case "":
                    resume.SchoolPractice = propertyValue;
                    break;
                case "语言能力":
                case "LanguageSkills":
                    resume.LanguageSkills = propertyValue;
                    break;
                case "培训经历":
                case "Training":
                    resume.Training = propertyValue;
                    break;
                case "专业技能":
                case "ProfessionalSkills":
                    resume.ProfessionalSkills = propertyValue;
                    break;
                default:
                    break;
            }
        }
    }
}

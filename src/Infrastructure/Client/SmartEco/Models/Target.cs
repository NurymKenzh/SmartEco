using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class Target
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public PollutionEnvironment PollutionEnvironment { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public int PollutionEnvironmentId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = NameRU;
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "en")
                {
                    name = NameEN;
                }
                return name;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TypeOfAchievement")]
        public bool TypeOfAchievement { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TypeOfAchievement")]
        public string TypeOfAchievementName
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    TypeOfAchievementName = "";
                if (TypeOfAchievement)
                {
                    TypeOfAchievementName = Resources.Controllers.SharedResources.Direct;
                }
                if (!TypeOfAchievement)
                {
                    TypeOfAchievementName = Resources.Controllers.SharedResources.Reverse;
                }
                return TypeOfAchievementName;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameterUnit")]
        public MeasuredParameterUnit MeasuredParameterUnit { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameterUnit")]
        public int MeasuredParameterUnitId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public Project Project { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public int? ProjectId { get; set; }
    }
}

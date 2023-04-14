using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class Layer
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerWorkspace")]
        public string GeoServerWorkspace { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerName")]
        public string GeoServerName { get; set; }

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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public PollutionEnvironment PollutionEnvironment { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public int? PollutionEnvironmentId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public MeasuredParameter MeasuredParameter { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public int? MeasuredParameterId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public KATO KATO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public int? KATOId { get; set; }
        [NotMapped]
        public int? KATOId2 { get; set; }
        [NotMapped]
        public int? KATOId3 { get; set; }
        [NotMapped]
        public int? KATOId4 { get; set; }
        [NotMapped]
        public int? KATOId5 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Season")]
        public Season? Season { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Hour")]
        public int? Hour { get; set; }
    }
}

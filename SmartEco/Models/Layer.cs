using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class Layer
    {
        public int Id { get; set; }

        public string GeoServerWorkspace { get; set; }
        public string GeoServerName { get; set; }

        public string NameKK { get; set; }
        public string NameRU { get; set; }
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

        public PollutionEnvironment PollutionEnvironment { get; set; }
        public int? PollutionEnvironmentId { get; set; }

        public MeasuredParameter MeasuredParameter { get; set; }
        public int? MeasuredParameterId { get; set; }

        public KATO KATO { get; set; }
        public int? KATOId { get; set; }
        [NotMapped]
        public int? KATOId2 { get; set; }
        [NotMapped]
        public int? KATOId3 { get; set; }
        [NotMapped]
        public int? KATOId4 { get; set; }
        [NotMapped]
        public int? KATOId5 { get; set; }

        public Season? Season { get; set; }

        public int? Hour { get; set; }
    }
}

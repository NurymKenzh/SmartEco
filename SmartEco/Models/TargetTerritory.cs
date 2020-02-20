using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class TargetTerritory
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TerritoryType")]
        public TerritoryType TerritoryType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TerritoryType")]
        public int TerritoryTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public KATO KATO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public int? KATOId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }

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
                return name;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GISConnectionCode")]
        public string GISConnectionCode { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformationKK")]
        public string AdditionalInformationKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformationRU")]
        public string AdditionalInformationRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformation")]
        public string AdditionalInformation
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    AdditionalInformation = AdditionalInformationRU;
                if (language == "kk")
                {
                    AdditionalInformation = AdditionalInformationKK;
                }
                return AdditionalInformation;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringPost")]
        public MonitoringPost MonitoringPost { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringPost")]
        public int? MonitoringPostId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KazHydrometSoilPost")]
        public KazHydrometSoilPost KazHydrometSoilPost { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KazHydrometSoilPost")]
        public int? KazHydrometSoilPostId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Post")]
        public string Post
        {
            get
            {
                string Post = "";
                if (MonitoringPost != null)
                {
                    Post = MonitoringPost.Name;
                }
                if (KazHydrometSoilPost != null)
                {
                    Post = KazHydrometSoilPost.Name;
                }
                return Post;
            }
        }
    }
}

using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class TargetValue
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Target")]
        public Target Target { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Target")]
        public int TargetId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TargetTerritory")]
        public TargetTerritory TargetTerritory { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TargetTerritory")]
        public int TargetTerritoryId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        [Range(Constants.YearMin, Constants.YearMax, ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "ErrorNumberRangeMustBe")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TargetValueType")]
        public bool TargetValueType { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TargetValueType")]
        public string TargetValueTypeName
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    TargetValueTypeName = "";
                if (TargetValueType)
                {
                    TargetValueTypeName = Resources.Controllers.SharedResources.Actual;
                }
                if (!TargetValueType)
                {
                    TargetValueTypeName = Resources.Controllers.SharedResources.Planned;
                }
                return TargetValueTypeName;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal Value { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformationKK")]
        public string AdditionalInformationKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformationRU")]
        public string AdditionalInformationRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public Project Project { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public int? ProjectId { get; set; }
    }
}

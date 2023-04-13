using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class TreesByFacilityManagementMeasuresList
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GreemPlantsPassport")]
        public GreemPlantsPassport GreemPlantsPassport { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GreemPlantsPassport")]
        public int GreemPlantsPassportId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsType")]
        public PlantationsType PlantationsType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsType")]
        public int PlantationsTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "BusinessEvents")]
        public PlantationsType BusinessEventsPlantationsType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "BusinessEvents")]
        public int? BusinessEventsPlantationsTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SanitaryPruning")]
        public int? SanitaryPruning { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CrownFormation")]
        public int? CrownFormation { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SanitaryFelling")]
        public int? SanitaryFelling { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaintenanceWork")]
        public string MaintenanceWork { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Quantity")]
        public string Quantity { get; set; }
        
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "BusinessEvents")]
        public bool BusinessEvents { get; set; }

        public TreesByFacilityManagementMeasuresList()
        {
            BusinessEvents = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class TreesByFacilityManagementMeasuresList
    {
        public int Id { get; set; }
        
        public GreemPlantsPassport GreemPlantsPassport { get; set; }
        public int GreemPlantsPassportId { get; set; }
        
        public PlantationsType PlantationsType { get; set; }
        public int PlantationsTypeId { get; set; }
        
        public PlantationsType BusinessEventsPlantationsType { get; set; }
        public int? BusinessEventsPlantationsTypeId { get; set; }
        
        public int? SanitaryPruning { get; set; }
        
        public int? CrownFormation { get; set; }
        
        public int? SanitaryFelling { get; set; }
        
        public string MaintenanceWork { get; set; }
        
        public string Quantity { get; set; }

        [NotMapped]
        public bool BusinessEvents { get; set; }
    }
}

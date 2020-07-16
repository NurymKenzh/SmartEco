using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class AActivity
    {
        public int Id { get; set; }

        public Target Target { get; set; }
        public int TargetId { get; set; }

        public TargetTerritory TargetTerritory { get; set; }
        public int TargetTerritoryId { get; set; }

        public Event Event { get; set; }
        public int EventId { get; set; }

        public int Year { get; set; }

        public bool ActivityType { get; set; }

        public decimal ImplementationPercentage { get; set; }

        public string AdditionalInformationKK { get; set; }

        public string AdditionalInformationRU { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}

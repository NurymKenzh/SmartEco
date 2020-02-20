using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class TargetValue
    {
        public int Id { get; set; }

        public Target Target { get; set; }
        public int TargetId { get; set; }

        public TargetTerritory TargetTerritory { get; set; }
        public int TargetTerritoryId { get; set; }

        public int Year { get; set; }

        public bool TargetValueType { get; set; }

        public decimal Value { get; set; }

        public string AdditionalInformationKK { get; set; }

        public string AdditionalInformationRU { get; set; }
    }
}

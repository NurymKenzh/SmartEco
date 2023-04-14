using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class SourceEmission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public SourceAirPollution SourceAirPollution { get; set; }
        public int SourceAirPollutionId { get; set; }
    }
}

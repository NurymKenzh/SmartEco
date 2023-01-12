using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class SourceAirPollution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public Manufactory Manufactory { get; set; }
        public int ManufactoryId { get; set; }
    }
}

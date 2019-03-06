using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class KazHydrometSoilPost
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string AdditionalInformation { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
    }
}

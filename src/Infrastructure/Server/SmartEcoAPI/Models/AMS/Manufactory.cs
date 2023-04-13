using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class Manufactory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public Enterprise Enterprise { get; set; }
        public int EnterpriseId { get; set; }
    }
}

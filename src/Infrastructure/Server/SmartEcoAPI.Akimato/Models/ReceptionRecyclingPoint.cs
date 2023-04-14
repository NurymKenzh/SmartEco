using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class ReceptionRecyclingPoint
    {
        public int Id { get; set; }
        
        public string Organization { get; set; }
        
        public string Address { get; set; }
        
        public string TypesRaw { get; set; }
        
        public decimal? NorthLatitude { get; set; }
        
        public decimal? EastLongitude { get; set; }
        
        public Project Project { get; set; }
        public int? ProjectId { get; set; }
    }
}

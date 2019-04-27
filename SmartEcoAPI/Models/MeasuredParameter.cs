using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class MeasuredParameter
    {
        public int Id { get; set; }
        public string NameKK { get; set; }
        public string NameRU { get; set; }
        public string NameEN { get; set; }
        public int? EcomonCode { get; set; }
        public decimal? MPC { get; set; } // maximum permissible concentration
    }
}

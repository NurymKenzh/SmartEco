using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class PollutionSourceData
    {
        public long Id { get; set; }
        public int PollutantId { get; set; }
        public Pollutant Pollutant { get; set; }
        public int PollutionSourceId { get; set; }
        public PollutionSource PollutionSource { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
    }
}

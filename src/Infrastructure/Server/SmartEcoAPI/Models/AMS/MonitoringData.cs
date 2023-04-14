using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class MonitoringData
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
        public int MonitoringParameterId { get; set; }
        public MonitoringParameter MonitoringParameter { get; set; }
        public int SourceEmissionId { get; set; }
        public SourceEmission SourceEmission { get; set; }
    }

    public class MonitoringDataViewModel 
    { 
        public List<MonitoringData> MonitoringDatas { get; set; } 
        public int MonitoringDatasCount { get; set; } 
    };
}

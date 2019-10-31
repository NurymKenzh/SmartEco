using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class MonitoringPostMeasuredParameters
    {
        public int Id { get; set; }
        public int MonitoringPostId { get; set; }
        public MonitoringPost MonitoringPost { get; set; }
        public bool Sensor { get; set; }
        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? MinMeasuredValue { get; set; }
        public decimal? MaxMeasuredValue { get; set; }
    }
}

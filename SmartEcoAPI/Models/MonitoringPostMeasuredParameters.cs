using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class MonitoringPostMeasuredParameters
    {
        public int Id { get; set; }
        public int MonitoringPostId { get; set; }
        public MonitoringPost MonitoringPost { get; set; }
        [NotMapped]
        public bool Sensor { get; set; }
        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }
        [NotMapped]
        public string MeasuredParameterNameRU { get; set; }
        [NotMapped]
        public string MeasuredParameterNameKK { get; set; }
        [NotMapped]
        public string MeasuredParameterNameEN { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class MeasuredData
    {
        public long Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public int MeasuredParameterId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public MeasuredParameter MeasuredParameter { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public DateTime DateTime { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal Value { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EcomonMonitoringPoint")]
        public int? EcomonMonitoringPointId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EcomonMonitoringPoint")]
        public EcomonMonitoringPoint EcomonMonitoringPoint { get; set; }

        [Display(Name = "Ecomontimestamp_ms")]
        public long? Ecomontimestamp_ms { get; set; }
    }
}

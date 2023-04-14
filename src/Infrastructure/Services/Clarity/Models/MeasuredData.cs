using System;
using System.Collections.Generic;
using System.Text;

namespace Clarity.Models
{
    public class MeasuredData
    {
        public long Id { get; set; }
        public int MeasuredParameterId { get; set; }
        public DateTime? DateTime { get; set; }
        public decimal? Value { get; set; }
        public int? MonitoringPostId { get; set; }
        public bool? Averaged { get; set; }
    }
}

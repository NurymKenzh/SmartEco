using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class MeasuredData
    {
        public long Id { get; set; }

        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }

        public DateTime? DateTime { get; set; }

        public decimal? Value { get; set; }

        public long? Ecomontimestamp_ms { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }

        public int? MaxValueMonth { get; set; } // per year
        public int? MaxValueDay { get; set; } // per month

        public decimal? MaxValuePerYear { get; set; }
        public decimal? MaxValuePerMonth { get; set; }

        public int? MonitoringPostId { get; set; }
        public MonitoringPost MonitoringPost { get; set; }

        public int? PollutionSourceId { get; set; }
        public PollutionSource PollutionSource { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class MeasuredData
    {
        public long Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public int MeasuredParameterId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameter")]
        public MeasuredParameter MeasuredParameter { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public DateTime? DateTime { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal? Value { get; set; }

        [Display(Name = "Ecomontimestamp_ms")]
        public long? Ecomontimestamp_ms { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaxValueMonth")]
        public int? MaxValueMonth { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaxValueDay")]
        public int? MaxValueDay { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaxValuePerYear")]
        public decimal? MaxValuePerYear { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaxValuePerMonth")]
        public decimal? MaxValuePerMonth { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public string DateTimeOrYearMonth
        {
            get
            {
                if(DateTime!=null)
                {
                    return DateTime.ToString();
                }
                else if(Month != null)
                {
                    if(Month<=9)
                    {
                        return $"0{Month?.ToString()}.{Year?.ToString()}";
                    }
                    else
                    {
                        return $"{Month?.ToString()}.{Year?.ToString()}";
                    }
                }
                return $"{Year?.ToString()}";
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringPost")]
        public int? MonitoringPostId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringPost")]
        public MonitoringPost MonitoringPost { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionSource")]
        public int? PollutionSourceId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionSource")]
        public PollutionSource PollutionSource { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Averaged")]
        public bool? Averaged { get; set; }
    }
}

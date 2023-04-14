using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class PollutionSourceData
    {
        public long Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pollutant")]
        public int PollutantId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pollutant")]
        public Pollutant Pollutant { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionSource")]
        public int PollutionSourceId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionSource")]
        public PollutionSource PollutionSource { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public DateTime DateTime { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal Value { get; set; }
    }
}

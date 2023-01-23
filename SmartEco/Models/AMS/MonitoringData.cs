using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace SmartEco.Models.AMS
{
    public class MonitoringData
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public DateTime DateTime { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        public decimal Value { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringParameter")]
        public int MonitoringParameterId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MonitoringParameter")]
        public MonitoringParameter MonitoringParameter { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SourceEmission")]
        public int SourceEmissionId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SourceEmission")]
        public SourceEmission SourceEmission { get; set; }
    }



    public class MonitoringDataViewModel
    {
        public List<MonitoringData> MonitoringDatas { get; set; }
        public int MonitoringDatasCount { get; set; }
    };
}

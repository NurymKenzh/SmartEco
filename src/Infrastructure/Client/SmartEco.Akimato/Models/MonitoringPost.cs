using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class MonitoringPost
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Number")]
        public int Number { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TurnOnOff")]
        public bool TurnOnOff { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Automatic")]
        public bool Automatic { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NorthLatitude")]
        public decimal NorthLatitude { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EastLongitude")]
        public decimal EastLongitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AdditionalInformation")]
        public string AdditionalInformation { get; set; }

        public string MN { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KazhydrometID")]
        public int? KazhydrometID { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DataProvider")]
        public DataProvider DataProvider { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DataProvider")]
        public int DataProviderId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public PollutionEnvironment PollutionEnvironment { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PollutionEnvironment")]
        public int PollutionEnvironmentId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public Project Project { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public int? ProjectId { get; set; }
    }
}

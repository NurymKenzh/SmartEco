using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartEco.Models.AMS
{
    public class SourceAirPollution
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NorthLatitude")]
        public decimal NorthLatitude { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EastLongitude")]
        public decimal EastLongitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Manufactory")]
        public Manufactory Manufactory { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Manufactory")]
        public int ManufactoryId { get; set; }
    }
}

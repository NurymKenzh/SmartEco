using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartEco.Models.ASM
{
    public class Manufactory
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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Enterprise")]
        public Enterprise Enterprise { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Enterprise")]
        public int EnterpriseId { get; set; }
    }
}

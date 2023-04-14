using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class ReceptionRecyclingPoint
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Organization")]
        public string Organization { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TypesRaw")]
        public string TypesRaw { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NorthLatitude")]
        public decimal? NorthLatitude { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EastLongitude")]
        public decimal? EastLongitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public Project Project { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Project")]
        public int? ProjectId { get; set; }
    }
}

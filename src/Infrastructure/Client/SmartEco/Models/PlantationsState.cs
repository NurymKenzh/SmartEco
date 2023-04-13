using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class PlantationsState
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public KATO KATO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KATO")]
        public int KATOId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsStateType")]
        public PlantationsStateType PlantationsStateType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsStateType")]
        public int PlantationsStateTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TreesNumber")]
        public decimal TreesNumber { get; set; }

    }
}

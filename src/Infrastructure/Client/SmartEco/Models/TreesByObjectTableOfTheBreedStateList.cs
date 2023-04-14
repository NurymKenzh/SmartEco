using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class TreesByObjectTableOfTheBreedStateList
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GreemPlantsPassport")]
        public GreemPlantsPassport GreemPlantsPassport { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GreemPlantsPassport")]
        public int GreemPlantsPassportId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsType")]
        public PlantationsType PlantationsType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PlantationsType")]
        public int PlantationsTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "StateOfCSR15")]
        public PlantationsType StateOfCSR15PlantationsType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "StateOfCSR15")]
        public int? StateOfCSR15PlantationsTypeId { get; set; }

        [Display(Name = "1")]
        public int? StateOfCSR15_1 { get; set; }

        [Display(Name = "2")]
        public int? StateOfCSR15_2 { get; set; }

        [Display(Name = "3")]
        public int? StateOfCSR15_3 { get; set; }

        [Display(Name = "4")]
        public int? StateOfCSR15_4 { get; set; }

        [Display(Name = "5")]
        public int? StateOfCSR15_5 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Quantity")]
        public string Quantity { get; set; }
        
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "StateOfCSR15")]
        public bool StateOfCSR15Type { get; set; }

        public TreesByObjectTableOfTheBreedStateList()
        {
            StateOfCSR15Type = true;
        }
    }
}

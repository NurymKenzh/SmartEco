using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class TreesByObjectTableOfTheBreedStateList
    {
        public int Id { get; set; }
        
        public GreemPlantsPassport GreemPlantsPassport { get; set; }
        public int GreemPlantsPassportId { get; set; }
        
        public PlantationsType PlantationsType { get; set; }
        public int PlantationsTypeId { get; set; }
        
        public PlantationsType StateOfCSR15PlantationsType { get; set; }
        public int? StateOfCSR15PlantationsTypeId { get; set; }
        
        public int? StateOfCSR15_1 { get; set; }
        
        public int? StateOfCSR15_2 { get; set; }
        
        public int? StateOfCSR15_3 { get; set; }
        
        public int? StateOfCSR15_4 { get; set; }
        
        public int? StateOfCSR15_5 { get; set; }
        
        public string Quantity { get; set; }

        [NotMapped]
        public bool StateOfCSR15Type { get; set; }
    }
}

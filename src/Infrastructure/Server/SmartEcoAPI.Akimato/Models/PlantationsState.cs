using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class PlantationsState
    {
        public int Id { get; set; }

        public KATO KATO { get; set; }
        public int KATOId { get; set; }

        public PlantationsStateType PlantationsStateType { get; set; }
        public int PlantationsStateTypeId { get; set; }
        public decimal TreesNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class SpeciesDiversity
    {
        public int Id { get; set; }

        public KATO KATO { get; set; }
        public int KATOId { get; set; }

        public PlantationsType PlantationsType { get; set; }
        public int PlantationsTypeId { get; set; }

        public int TreesNumber { get; set; }
    }
}

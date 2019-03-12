using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class KATO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public int AreaType { get; set; }
        public int EgovId { get; set; }
        public int ParentEgovId { get; set; }
        public string NameKK { get; set; }
        public string NameRU { get; set; }
    }
}

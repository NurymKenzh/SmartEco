using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class Legend
    {
        public int Id { get; set; }

        public string TextKK { get; set; }
        public string TextRU { get; set; }
        public string TextEN { get; set; }

        [NotMapped]
        public List<LegendItem> Items;
    }

    public class LegendItem
    {
        public int Id { get; set; }

        public int LegendId { get; set; }
        public Legend Legend { get; set; }

        public string TextKK { get; set; }
        public string TextRU { get; set; }
        public string TextEN { get; set; }

        public int IndexNumber { get; set; }

        public string Color { get; set; }
    }
}

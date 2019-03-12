using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class KATO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int Level { get; set; }

        public int AreaType { get; set; }

        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Number")]
        public int EgovId { get; set; }

        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Number")]
        public int ParentEgovId { get; set; }

        public string NameKK { get; set; }

        public string NameRU { get; set; }

        public string Name
        {
            get
            {
                // переписать (пример в class MeasuredParameter)
                return NameRU;
            }
        }
    }
}

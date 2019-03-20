using Microsoft.AspNetCore.Builder;
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
        public int? ParentEgovId { get; set; }

        public string NameKK { get; set; }

        public string NameRU { get; set; }

        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                   name = NameRU;
                if (language == "kk")
                {
                    name = NameKK;
                }
                return name;
            }
        }
    }
}

using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class KATO
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Code")]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Level")]
        public int Level { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AreaType")]
        public int AreaType { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EgovId")]
        public int EgovId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ParentEgovId")]
        public int? ParentEgovId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
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

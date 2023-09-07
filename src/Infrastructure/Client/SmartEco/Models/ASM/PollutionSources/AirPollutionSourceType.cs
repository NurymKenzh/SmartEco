using SmartEco.Models.ASM.Filsters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class AirPollutionSourceType
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Организованный источник")]
        public bool IsOrganized { get; set; }
    }

    public class AirPollutionSourceTypeListViewModel
    {
        public AirPollutionSourceTypeFilter Filter { get; set; }
        public List<AirPollutionSourceType> Items { get; set; }
        public Pager Pager { get; set; }
    }

    public class AirPollutionSourceTypeViewModel
    {
        public AirPollutionSourceTypeFilter Filter { get; set; }
        public AirPollutionSourceType Item { get; set; }
    }
}

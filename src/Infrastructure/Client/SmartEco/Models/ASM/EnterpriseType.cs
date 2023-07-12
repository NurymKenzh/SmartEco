using SmartEco.Models.ASM.Filsters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM
{
    public class EnterpriseType
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }
    }

    public class EnterpriseTypeListViewModel
    {
        public EnterpriseTypeFilter Filter { get; set; }
        public List<EnterpriseType> Items { get; set; }
        public Pager Pager { get; set; }
    }

    public class EnterpriseTypeViewModel
    {
        public EnterpriseTypeFilter Filter { get; set; }
        public EnterpriseType Item { get; set; }
    }
}

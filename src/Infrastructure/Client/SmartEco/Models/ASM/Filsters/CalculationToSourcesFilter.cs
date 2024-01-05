using System.Collections.Generic;

namespace SmartEco.Models.ASM.Filsters
{
    public class CalculationToSourcesFilter : BaseFilter
    {
        public int CalculationId { get; set; }
        public List<int> EnterpriseIds { get; set; }
    }
}

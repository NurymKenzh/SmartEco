using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationStatusesResponse
    {

        public List<CalculationStatus> CalculationStatuses { get; set; }
        public int Count { get; set; }
    }
}

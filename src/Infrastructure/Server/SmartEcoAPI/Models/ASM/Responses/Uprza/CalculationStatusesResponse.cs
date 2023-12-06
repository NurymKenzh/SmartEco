using SmartEcoAPI.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class CalculationStatusesResponse
    {
        public CalculationStatusesResponse(List<CalculationStatus> calculationStatuses, int count) 
        {
            CalculationStatuses = calculationStatuses;
            Count = count;
        }

        public List<CalculationStatus> CalculationStatuses { get; set; }
        public int Count { get; set; }
    }
}

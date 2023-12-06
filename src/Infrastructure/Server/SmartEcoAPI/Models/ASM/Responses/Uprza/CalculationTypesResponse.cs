using SmartEcoAPI.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class CalculationTypesResponse
    {
        public CalculationTypesResponse(List<CalculationType> calculationTypes, int count) 
        {
            CalculationTypes = calculationTypes;
            Count = count;
        }

        public List<CalculationType> CalculationTypes { get; set; }
        public int Count { get; set; }
    }
}

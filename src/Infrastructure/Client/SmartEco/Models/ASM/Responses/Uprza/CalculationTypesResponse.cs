using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses.Uprza
{
    public class CalculationTypesResponse
    {
        public List<CalculationType> CalculationTypes { get; set; }
        public int Count { get; set; }
    }
}

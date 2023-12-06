using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationsResponse
    {
        public List<Calculation> Calculations { get; set; }
        public int Count { get; set; }
    }
}

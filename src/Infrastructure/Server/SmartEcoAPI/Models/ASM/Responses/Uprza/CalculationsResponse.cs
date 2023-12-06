using SmartEcoAPI.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class CalculationsResponse
    {
        public CalculationsResponse(List<Calculation> calculations, int count) 
        {
            Calculations = calculations;
            Count = count;
        }

        public List<Calculation> Calculations { get; set; }
        public int Count { get; set; }
    }
}

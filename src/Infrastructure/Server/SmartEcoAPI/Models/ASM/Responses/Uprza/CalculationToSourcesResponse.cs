using SmartEcoAPI.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class CalculationToSourcesResponse
    {
        public CalculationToSourcesResponse(List<AirPollutionSourceInvolved> sources, bool isInvolvedAllSorces, int count) 
        {
            Sources = sources;
            IsInvolvedAllSorces = isInvolvedAllSorces;
            Count = count;
        }

        public List<AirPollutionSourceInvolved> Sources { get; set; }
        public bool IsInvolvedAllSorces { get; set; }
        public int Count { get; set; }
    }
}

using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationToSourcesResponse
    {
        public CalculationToSourcesResponse(List<PollutionSourceInvolved> sources, int count)
        {
            Sources = sources;
            Count = count;
        }

        public List<PollutionSourceInvolved> Sources { get; set; }
        public bool IsInvolvedAllSorces { get; set; }
        public int Count { get; set; }
    }
}

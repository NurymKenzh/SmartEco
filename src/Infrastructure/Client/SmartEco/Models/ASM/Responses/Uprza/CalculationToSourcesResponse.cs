using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationToSourcesResponse
    {
        public CalculationToSourcesResponse(
            List<AirPollutionSourceInvolved> sources, 
            bool isInvolvedAllSorces, 
            int count, 
            List<AirPollutant> airPollutants) 
        {
            Sources = sources;
            IsInvolvedAllSorces = isInvolvedAllSorces;
            Count = count;

            AirPollutants = airPollutants;
        }

        public List<AirPollutionSourceInvolved> Sources { get; set; }
        public bool IsInvolvedAllSorces { get; set; }
        public int Count { get; set; }

        public List<AirPollutant> AirPollutants { get; set; }
    }
}

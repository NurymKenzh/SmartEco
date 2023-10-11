using SmartEcoAPI.Models.ASM.PollutionSources;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class AirPollutantsResponse
    {
        public AirPollutantsResponse(List<AirPollutant> airPollutants, int count) 
        {
            AirPollutants = airPollutants;
            Count = count;
        }

        public List<AirPollutant> AirPollutants { get; set; }
        public int Count { get; set; }
    }
}

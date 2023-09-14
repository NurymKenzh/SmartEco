using SmartEco.Models.ASM.PollutionSources;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class AirPollutionSourcesResponse
    {
        public AirPollutionSourcesResponse(List<AirPollutionSource> airPollutionSources, int count) 
        {
            AirPollutionSources = airPollutionSources;
            Count = count;
        }

        public List<AirPollutionSource> AirPollutionSources { get; set; }
        public int Count { get; set; }
    }
}

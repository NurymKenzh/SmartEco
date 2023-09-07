using SmartEcoAPI.Models.ASM.PollutionSources;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class AirPollutionSourceTypesResponse
    {
        public AirPollutionSourceTypesResponse(List<AirPollutionSourceType> airPollutionSourceTypes, int count) 
        {
            AirPollutionSourceTypes = airPollutionSourceTypes;
            Count = count;
        }

        public List<AirPollutionSourceType> AirPollutionSourceTypes { get; set; }
        public int Count { get; set; }
    }
}

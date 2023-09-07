using SmartEco.Models.ASM.PollutionSources;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class AirPollutionSourceTypesResponse
    {
        public List<AirPollutionSourceType> AirPollutionSourceTypes { get; set; }
        public int Count { get; set; }
    }
}

using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class OperationMode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkedTime { get; set; }

        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }

        public GasAirMixture GasAirMixture { get; set; }

        public List<AirEmission> Emissions { get; set; }
    }
}

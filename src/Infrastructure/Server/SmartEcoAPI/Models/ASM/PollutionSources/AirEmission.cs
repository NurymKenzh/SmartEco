using System;

namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class AirEmission
    {
        public int Id { get; set; }

        public int PollutantId { get; set; }
        public AirPollutant Pollutant { get; set; }

        public int OperationModeId { get; set; }
        public OperationMode OperationMode { get; set; }

        public decimal MaxGramSec { get; set; }
        public decimal? MaxMilligramMeter { get; set; }
        public decimal GrossTonYear { get; set; }
        public int SettlingCoef { get; set; }
        public DateTime EnteredDate { get; set; }
    }
}

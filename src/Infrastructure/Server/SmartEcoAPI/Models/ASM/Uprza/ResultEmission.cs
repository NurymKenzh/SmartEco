using SmartEcoAPI.Models.ASM.PollutionSources;

namespace SmartEcoAPI.Models.ASM.Uprza
{
    public class ResultEmission
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int AirPollutantId { get; set; }
        public AirPollutant AirPollutant { get; set; }

        public string FeatureCollection { get; set; }
    }
}

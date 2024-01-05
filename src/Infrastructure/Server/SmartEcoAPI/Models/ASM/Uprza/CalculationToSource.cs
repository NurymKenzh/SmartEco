using SmartEcoAPI.Models.ASM.PollutionSources;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM.Uprza
{
    public class CalculationToSource
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }
    }

    //[NotMapped]
    public class AirPollutionSourceInvolved : AirPollutionSource
    {
        public bool IsInvolved { get; set; }
    }
}

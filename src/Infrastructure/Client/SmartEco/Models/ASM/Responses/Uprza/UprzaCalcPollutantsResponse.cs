using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses.Uprza
{
    public class UprzaCalcPollutantsResponse
    {
        public List<UprzaCalculationPollutant> CalculationPollutants { get; set; }
    }

    public class UprzaCalculationPollutant
    {
        public int JobId { get; set; }
        public int Code { get; set; }
    }

}

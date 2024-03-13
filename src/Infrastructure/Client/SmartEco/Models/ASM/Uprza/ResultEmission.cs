using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models.ASM.PollutionSources;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Uprza
{
    public class ResultEmission
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int AirPollutantId { get; set; }
        public AirPollutant AirPollutant { get; set; }

        public string RectanglesFeatures { get; set; }

        public string PointsFeatures { get; set; }
    }

    public class ResultEmissionViewModel
    {
        public int CalculationId { get; set; }
        public SelectList AirPollutantsSelectList { get; set; }
        public List<IndSiteEnterprise> IndSiteEnterprises { get; set; }
    }

    public enum ResultEmissionType
    {
        Rectangle,
        Point
    }
}

using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationSettingsResponse
    {
        public CalculationSettingsResponse(
            List<CalculationPoint> calcPoints, 
            List<CalculationRectangle> calcRectangles,
            CalculationSetting calcSetting,
            StateCalculation stateCalculation) 
        {
            CalcPoints = calcPoints;
            CalcRectangles = calcRectangles;
            CalcSetting = calcSetting;
            StateCalculation = stateCalculation;
        }

        public List<CalculationPoint> CalcPoints { get; set; }
        public List<CalculationRectangle> CalcRectangles { get; set; }
        public CalculationSetting CalcSetting { get; set; }
        public StateCalculation StateCalculation { get; set; }
    }
}

using SmartEcoAPI.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class CalculationSettingsResponse
    {
        public CalculationSettingsResponse(List<CalculationPoint> calcPoints)
        {
            CalcPoints = calcPoints;
        }

        public CalculationSettingsResponse(List<CalculationRectangle> calcRectangles)
        {
            CalcRectangles = calcRectangles;
        }

        public CalculationSettingsResponse(
            List<CalculationPoint> calcPoints, 
            List<CalculationRectangle> calcRectangles, 
            CalculationSettingMvc calcSetting,
            StateCalculation stateCalculation)
        {
            CalcPoints = calcPoints;
            CalcRectangles = calcRectangles;
            CalcSetting = calcSetting;
            StateCalculation = stateCalculation;
        }

        public List<CalculationPoint> CalcPoints { get; set; }
        public List<CalculationRectangle> CalcRectangles { get; set; }
        public CalculationSettingMvc CalcSetting { get; set; }
        public StateCalculation StateCalculation { get; set; }
    }
}

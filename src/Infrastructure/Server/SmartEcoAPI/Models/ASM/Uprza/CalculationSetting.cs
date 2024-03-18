using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Uprza
{
    public class CalculationSetting
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public string SettingsJson { get; set; }
    }

    public class CalculationPoint
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int Number { get; set; }

        public double AbscissaX { get; set; }
        public double OrdinateY { get; set; }
        public int ApplicateZ { get; set; }

        public double Abscissa3857 { get; set; }
        public double Ordinate3857 { get; set; }
    }

    public class CalculationRectangle
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int Number { get; set; }

        public double AbscissaX { get; set; }
        public double OrdinateY { get; set; }

        public int Width { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }

        public int StepByWidth { get; set; }
        public int StepByLength { get; set; }

        public double Abscissa3857 { get; set; }
        public double Ordinate3857 { get; set; }
    }

    public class CalculationSettingMvc
    {
        public int CalculationId { get; set; }

        public CalcWindSpeedSetting WindSpeedSetting { get; set; }
        public CalcWindDirectionSetting WindDirectionSetting { get; set; }

        public int ContributorCount { get; set; }
        public double ThresholdPdk { get; set; }
        public CalcSeasons Season { get; set; }

        public bool IsUseSummationGroups { get; set; }
        public bool IsUseBackground { get; set; }
        public bool IsUseBuilding { get; set; }

        public double? UnitBorderStep { get; set; }
        public double? SanitaryAreaBorderStep { get; set; }
        public double? LivingAreaBorderStep { get; set; }

        public bool IsUsePollutantsList { get; set; }
        public List<int> AirPollutantIds { get; set; }

        public class CalcWindSpeedSetting
        {
            public CalcWindModes Mode { get; set; }
            public float? Speed { get; set; }
            public float? StartSpeed { get; set; }
            public float? EndSpeed { get; set; }
            public float? StepSpeed { get; set; }
            public List<float?> Speeds { get; set; }
        }

        public class CalcWindDirectionSetting
        {
            public CalcWindModes Mode { get; set; }
            public float? Direction { get; set; }
            public float? StartDirection { get; set; }
            public float? EndDirection { get; set; }
            public float? StepDirection { get; set; }
            public List<float?> Directions { get; set; }
        }

        public enum CalcWindModes
        {
            Auto = 0,
            Fixed = 1,
            IteratingSetNumbers = 2,
            IteratingByStep = 3
        }

        public enum CalcSeasons
        {
            Summer = 1,
            Winter = 2
        }
    }
}

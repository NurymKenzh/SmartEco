using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.Uprza
{
    public class CalculationSetting
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
            [Display(Name = "Автомат")]
            Auto = 0,
            [Display(Name = "Фиксированное значение")]
            Fixed = 1,
            [Display(Name = "Перебор из заданного набора значений")]
            IteratingSetNumbers = 2,
            [Display(Name = "Перебор в заданном диапазоне с заданным шагом")]
            IteratingByStep = 3
        }

        public enum CalcSeasons
        {
            [Display(Name = "Лето")]
            Summer = 1,
            [Display(Name = "Зима")]
            Winter = 2
        }
    }

    public class CalculationPoint
    {
        [Display(Name = "№ п/п")]
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Display(Name = "X (м)")]
        public double AbscissaX { get; set; }
        [Display(Name = "Y (м)")]
        public double OrdinateY { get; set; }
        [Display(Name = "Z (м)")]
        public int ApplicateZ { get; set; }

        public double Abscissa3857 { get; set; }
        public double Ordinate3857 { get; set; }
    }

    public class CalculationRectangle
    {
        [Display(Name = "№ п/п")]
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Display(Name = "X (м)")]
        public double AbscissaX { get; set; }
        [Display(Name = "Y (м)")]
        public double OrdinateY { get; set; }

        [Display(Name = "Ширина (м)")]
        public int Width { get; set; }
        [Display(Name = "Длина (м)")]
        public int Length { get; set; }
        [Display(Name = "Высота (м)")]
        public int Height { get; set; }

        [Display(Name = "Шаг по X (м)")]
        public int StepByWidth { get; set; }
        [Display(Name = "Шаг по Y (м)")]
        public int StepByLength { get; set; }

        public double Abscissa3857 { get; set; }
        public double Ordinate3857 { get; set; }
    }

    public class CalculationSettingsViewModel
    {
        public List<CalculationRectangle> CalculationRectangles { get; set; } = new List<CalculationRectangle>();
        public List<CalculationPoint> CalculationPoints { get; set; } = new List<CalculationPoint>();
        public CalculationSetting CalculationSetting { get; set; } = new CalculationSetting();
        public StateCalculation StateCalculation { get; set; } = new StateCalculation();

        public MultiSelectList AirPollutionsSelectList { get; set; }
    }
}

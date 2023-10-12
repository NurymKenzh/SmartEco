using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class AirEmission
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Display(Name = "Загрязняющее вещество")]
        public int PollutantId { get; set; }
        [Display(Name = "Загрязняющее вещество")]
        public AirPollutant Pollutant { get; set; }

        public int OperationModeId { get; set; }
        public OperationMode OperationMode { get; set; }

        [Display(Name = "Максимальный (г/с)")]
        public decimal MaxGramSec { get; set; }

        [Display(Name = "Максимальный (мг/м3)")]
        public decimal? MaxMilligramMeter { get; set; }

        [Display(Name = "Валовый (т/год)")]
        public decimal GrossTonYear { get; set; }

        [Display(Name = "Коэффициент оседания")]
        public int SettlingCoef { get; set; }

        [Display(Name = "Дата ввода")]
        public DateTime EnteredDate { get; set; }
    }
}

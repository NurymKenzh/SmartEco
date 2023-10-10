using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class GasAirMixture
    {

        public int OperationModeId { get; set; }
        public OperationMode OperationMode { get; set; }

        [Display(Name = "Температура")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Temperature { get; set; }

        [Display(Name = "Давление")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Pressure { get; set; }

        [Display(Name = "Скорость")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Speed { get; set; }

        [Display(Name = "Объём")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Volume { get; set; }

        [Display(Name = "Влажность")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Humidity { get; set; }

        [Display(Name = "Плотность")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double Density { get; set; }

        [Display(Name = "Тепловая мощность")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double ThermalPower { get; set; }

        [Display(Name = "Часть тепловой мощности, затрачиваемая на излучение")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public double PartRadiation { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class OperationMode
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }

        [Display(Name = "Наработка, Ч")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(0, 8784, ErrorMessage = "Значение должно быть больше {1} и меньше {2}")]
        public int WorkedTime { get; set; }

        [Display(Name = "Параметры ГВС")]
        public GasAirMixture GasAirMixture { get; set; }

        public List<AirEmission> Emissions { get; set; }

        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }
    }
}

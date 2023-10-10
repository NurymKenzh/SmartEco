using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

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
        public int WorkedTime { get; set; }

        [Display(Name = "Параметры ГВС")]
        public GasAirMixture GasAirMixture { get; set; }

        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }
    }
}

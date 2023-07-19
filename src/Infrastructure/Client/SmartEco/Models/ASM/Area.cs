using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEco.Models.ASM
{
    public class Area
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Цех")]
        public int WorkshopId { get; set; }
        [Display(Name = "Цех")]
        public Workshop Workshop { get; set; }
    }
}

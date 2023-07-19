using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEco.Models.ASM
{
    public class Workshop
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Промплощадка предприятия")]
        public int IndSiteEnterpriseId { get; set; }
        [Display(Name = "Промплощадка предприятия")]
        public IndSiteEnterprise IndSiteEnterprise { get; set; }


    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM
{
    public class SanZoneEnterpriseBorder
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Допустимое значение концентрации, доли ПДК")]
        public decimal PermissibleConcentration { get; set; }

        [Required]
        [Display(Name = "Промплощадка предприятия")]
        public int IndSiteEnterpriseId { get; set; }
        [Display(Name = "Промплощадка предприятия")]
        public IndSiteEnterprise IndSiteEnterprise { get; set; }

        [Required]
        [Display(Name = "Координаты")]
        public List<string> Coordinates { get; set; }
    }

    public class SanZoneEnterpriseBorderListViewModel
    {
        public List<SanZoneEnterpriseBorder> Items { get; set; } = new List<SanZoneEnterpriseBorder>();
        public int IndSiteEnterpriseId { get; set; }
        public int EnterpriseId { get; set; }
    }
}
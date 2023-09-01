using System.ComponentModel.DataAnnotations;
namespace SmartEco.Models.ASM
{
    public class IndSiteEnterprise
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Предприятие")]
        public int EnterpriseId { get; set; }
        [Display(Name = "Предприятие")]
        public Enterprise Enterprise { get; set; }

        [Display(Name = "Размер минимальной санитарной зоны, м")]
        public int MinSizeSanitaryZone { get; set; }

        [Display(Name = "Местоположение")]
        public string Location => Enterprise?.Kato?.ComplexName;

        public IndSiteEnterpriseBorder IndSiteBorder { get; set; }
        public SanZoneEnterpriseBorder SanZoneBorder { get; set; }
    }
}
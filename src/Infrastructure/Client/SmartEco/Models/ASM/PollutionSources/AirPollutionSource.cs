using SmartEco.Models.ASM.Filsters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class AirPollutionSource
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Display(Name = "Номер")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Номер должен содержать 4 цифры")]
        public string Number { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }

        [Display(Name = "Используется")]
        public bool IsActive { get; set; }

        [Display(Name = "Тип источника")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int? TypeId { get; set; }
        [Display(Name = "Тип источника")]
        public AirPollutionSourceType Type { get; set; }

        //one-to-one relation
        public AirPollutionSourceIndSite SourceIndSite { get; set; }
        public AirPollutionSourceWorkshop SourceWorkshop { get; set; }
        public AirPollutionSourceArea SourceArea { get; set; }

        [Display(Name = "Параметры")]
        public AirPollutionSourceInfo SourceInfo { get; set; }

        [Display(Name = "Подразделение")]
        public SourceRelations Relation { get { return GetSourceRelation(); } }

        private SourceRelations GetSourceRelation()
        {
            if (SourceIndSite != null) return SourceRelations.IndSite;
            else if (SourceWorkshop != null) return SourceRelations.Workshop;
            else if (SourceArea != null) return SourceRelations.Area;
            else return SourceRelations.Undefined;

        }
    }

    public enum SourceRelations
    {
        Undefined,
        IndSite,
        Workshop,
        Area
    }

    public class AirPollutionSourceListViewModel
    {
        public AirPollutionSourceFilter Filter { get; set; }
        public List<AirPollutionSource> Items { get; set; }
        public Pager Pager { get; set; }

        public List<AirPollutionSourceType> DropdownTypes { get; set; }
        public List<IndSiteEnterprise> DropdownIndSite { get; set; }
        public List<Workshop> DropdownWorkShop { get; set; }
        public List<Area> DropdownArea{ get; set; }
    }
}

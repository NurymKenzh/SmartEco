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
        public string Number { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Используется")]
        public bool IsActive { get; set; }

        [Display(Name = "Тип источника")]
        public int TypeId { get; set; }
        [Display(Name = "Тип источника")]
        public AirPollutionSourceType Type { get; set; }

        //one-to-one relation
        public AirPollutionSourceIndSite SourceIndSite { get; set; }
        public AirPollutionSourceWorkshop SourceWorkshop { get; set; }
        public AirPollutionSourceArea SourceArea { get; set; }

        [Display(Name = "Параметры")]
        public AirPollutionSourceInfo SourceInfo { get; set; }

        public SourceRelations Relation { get { return GetSourceRelation(); } }

        [Display(Name = "Подразделение")]
        public string RelationCombine { get { return GetSourceRelationCombine(); } }

        private SourceRelations GetSourceRelation()
        {
            if (SourceIndSite != null) return SourceRelations.IndSite;
            else if (SourceWorkshop != null) return SourceRelations.Workshop;
            else if (SourceArea != null) return SourceRelations.Area;
            else return SourceRelations.Undefined;

        }

        private string GetSourceRelationCombine()
        {
            switch (Relation)
            {
                case SourceRelations.IndSite: return $"{SourceIndSite.IndSiteEnterprise.Name}";
                case SourceRelations.Workshop: return $"{SourceWorkshop.Workshop.IndSiteEnterprise.Name}, {SourceWorkshop.Workshop.Name}";
                case SourceRelations.Area: return $"{SourceArea.Area.Workshop.IndSiteEnterprise.Name}, {SourceArea.Area.Workshop.Name}, {SourceArea.Area.Name}";
                default: return string.Empty;
            }
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
        public int EnterpriseId { get; set; }
    }
}

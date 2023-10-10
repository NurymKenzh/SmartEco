using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class AirPollutionSource
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public int TypeId { get; set; }
        public AirPollutionSourceType Type { get; set; }

        //one-to-one relation
        public AirPollutionSourceIndSite SourceIndSite { get; set; }
        public AirPollutionSourceWorkshop SourceWorkshop { get; set; }
        public AirPollutionSourceArea SourceArea { get; set; }

        public AirPollutionSourceInfo SourceInfo { get; set; }

        public List<OperationMode> OperationModes { get; set; }

        [NotMapped]
        internal SourceRelations Relation { get { return GetSourceRelation(); } }

        [NotMapped]
        internal string RelationCombine { get { return GetSourceRelationCombine(); } }

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
}

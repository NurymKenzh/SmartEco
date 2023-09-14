namespace SmartEco.Models.ASM.Filsters
{
    public class AirPollutionSourceFilter : BaseFilter
    {
        public string NameFilter { get; set; }
        public string NumberFilter { get; set; }
        public string RelationFilter { get; set; }
        public int? EnterpriseId { get; set; }

        public string NameSort { get; set; }
        public string NumberSort { get; set; }
        public string RelationSort { get; set; }
    }

    public class AirPollutionSourceFilterId : AirPollutionSourceFilter
    {
        public int? Id { get; set; }
    }
}
namespace SmartEco.Models.ASM.Filsters
{
    public class AirPollutionSourceTypeFilter : BaseFilter
    {
        public string NameFilter { get; set; }
    }

    public class AirPollutionSourceTypeFilterId : AirPollutionSourceTypeFilter
    {
        public int? Id { get; set; }
    }
}
namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class AirPollutionSourceIndSite
    {
        public int AirPollutionSourceId { get; set; }
        public AirPollutionSource AirPollutionSource { get; set; }

        public int IndSiteEnterpriseId { get; set; }
        public IndSiteEnterprise IndSiteEnterprise { get; set; }
    }

    public class AirPollutionSourceWorkshop
    {
        public int AirPollutionSourceId { get; set; }
        public AirPollutionSource AirPollutionSource { get; set; }

        public int WorkshopId { get; set; }
        public Workshop Workshop { get; set; }
    }

    public class AirPollutionSourceArea
    {
        public int AirPollutionSourceId { get; set; }
        public AirPollutionSource AirPollutionSource { get; set; }

        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}

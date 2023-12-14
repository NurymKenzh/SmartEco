namespace SmartEco.Common.Data.Entities.SmartEcoApi
{
    public class MonitoringPost
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string? Name { get; set; }
        public bool TurnOnOff { get; set; }
        public bool Automatic { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? MN { get; set; }
        public string? PhoneNumber { get; set; }
        public int? KazhydrometID { get; set; }

        public int DataProviderId { get; set; }
        public required DataProvider DataProvider { get; set; }

        public int PollutionEnvironmentId { get; set; }
        public required PollutionEnvironment PollutionEnvironment { get; set; }

        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}

namespace SmartEco.Common.Data.Entities.SmartEcoApi
{
    public class PollutionSource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
    }
}

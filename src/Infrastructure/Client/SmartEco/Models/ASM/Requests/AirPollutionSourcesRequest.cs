namespace SmartEco.Models.ASM.Requests
{
    public class AirPollutionSourcesRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Relation { get; set; }
        public int? EnterpriseId { get; set; }
    }
}

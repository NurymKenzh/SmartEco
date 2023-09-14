using SmartEcoAPI.Models.ASM.Requests;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class AirPollutionSourcesRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Relation { get; set; }
        public int? EnterpriseId { get; set; }
    }
}

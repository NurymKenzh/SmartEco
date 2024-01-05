using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Requests
{
    public class CalculationToSourcesRequest : BaseRequest
    {
        public int? CalculationId { get; set; }
        public List<int> EnterpriseIds { get; set; }
    }
}

using System.Collections.Generic;

namespace SmartEco.Models.ASM.Requests
{
    public class CalculationToSourcesRequest : BaseRequest
    {
        public int? CalculationId { get; set; }
        public List<int> EnterpriseIds { get; set; }
    }
}

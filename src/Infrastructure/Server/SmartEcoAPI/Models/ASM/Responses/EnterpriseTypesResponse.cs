using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class EnterpriseTypesResponse
    {
        public EnterpriseTypesResponse(List<EnterpriseType> enterpriseTypes, int count) 
        {
            EnterpriseTypes = enterpriseTypes;
            Count = count;
        }

        public List<EnterpriseType> EnterpriseTypes { get; set; }
        public int Count { get; set; }
    }
}

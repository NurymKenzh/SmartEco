using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses
{
    public class EnterprisesResponse
    {
        public EnterprisesResponse(List<Enterprise> enterprises, int count) 
        {
            Enterprises = enterprises;
            Count = count;
        }

        public List<Enterprise> Enterprises { get; set; }
        public int Count { get; set; }
    }
}

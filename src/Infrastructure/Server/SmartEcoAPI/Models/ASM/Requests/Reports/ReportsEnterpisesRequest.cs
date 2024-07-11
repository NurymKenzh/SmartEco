using System;

namespace SmartEcoAPI.Models.ASM.Requests
{
    public class ReportsEnterpisesRequest : BaseRequest
    {
        public DateTime? CreatedDate { get; set; }
        public string Name { get; set; }
        public string KatoComplex { get; set; }
    }
}

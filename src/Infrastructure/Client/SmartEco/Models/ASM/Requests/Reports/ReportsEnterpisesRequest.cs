using System;

namespace SmartEco.Models.ASM.Requests.Reports
{
    public class ReportsEnterpisesRequest : BaseRequest
    {
        public DateTime? CreatedDate { get; set; }
        public string Name { get; set; }
        public string KatoComplex { get; set; }
    }
}

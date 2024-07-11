using SmartEco.Models.ASM.Reports;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses.Reports
{
    public class ReportsEnterprisesResponse
    {
        public List<ReportEnterprise> ReportsEnterprises { get; set; }
        public int Count { get; set; }
    }
}

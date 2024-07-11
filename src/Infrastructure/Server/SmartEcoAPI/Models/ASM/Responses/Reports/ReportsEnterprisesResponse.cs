using SmartEcoAPI.Models.ASM.Reports;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Responses.Reports
{
    public class ReportsEnterprisesResponse
    {
        public ReportsEnterprisesResponse(List<ReportEnterprise> reports, int count)
        {
            ReportsEnterprises = reports;
            Count = count;
        }

        public List<ReportEnterprise> ReportsEnterprises { get; set; }
        public int Count { get; set; }
    }
}

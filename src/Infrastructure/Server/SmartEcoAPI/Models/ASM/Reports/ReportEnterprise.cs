using System;

namespace SmartEcoAPI.Models.ASM.Reports
{
    public class ReportEnterprise
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }

        public string KatoCode { get; set; }
        public string KatoName { get; set; }
    }
}

using System;

namespace SmartEco.Models.ASM.Filsters
{
    public class ReportFilter : BaseFilter
    {
        public DateTime? CreatedDateFilter { get; set; }
        public string KatoComplexFilter { get; set; }
        public string NameFilter { get; set; }
    }

    public class ReportFilterId : ReportFilter
    {
        public int? Id { get; set; }
    }
}

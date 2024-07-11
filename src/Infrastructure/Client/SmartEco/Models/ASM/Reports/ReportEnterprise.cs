using SmartEco.Models.ASM.Filsters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.Reports
{
    public class ReportEnterprise
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Город или регион (КАТО)")]
        public string KatoComplexName => $"{KatoCode} {KatoName}";

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        public string KatoCode { get; set; }
        [Required]
        public string KatoName { get; set; }
    }

    #region ViewModels
    public class ReportEnterpriseListViewModel
    {
        public ReportFilter Filter { get; set; }
        public List<ReportEnterprise> Items { get; set; } = new List<ReportEnterprise>();
        public Pager Pager { get; set; }
    }

    public class ReportEnterpriseViewModel
    {
        public ReportFilter Filter { get; set; }
        public ReportEnterprise Item { get; set; }
    }
    #endregion
}

using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.Uprza
{
    public class Calculation
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование расчёта")]
        public string Name { get; set; }

        [Display(Name = "Вид расчёта")]
        public int TypeId { get; set; }
        [Display(Name = "Вид расчёта")]
        public CalculationType Type { get; set; }

        [Display(Name = "Статус готовности расчёта")]
        public int StatusId { get; set; }
        [Display(Name = "Статус готовности расчёта")]
        public CalculationStatus Status { get; set; }

        public string KatoCode { get; set; }
        public string KatoName { get; set; }

        [Display(Name = "Город или регион (КАТО)")]
        public string KatoComplexName => $"{KatoCode} {KatoName}";
    }

    #region ViewModels
    public class CalculationListViewModel
    {
        public CalculationFilter Filter { get; set; }
        public List<Calculation> Items { get; set; }
        public Pager Pager { get; set; }
        public SelectList CalculationTypesSelectList { get; set; }
        public SelectList CalculationStatusesSelectList { get; set; }
    }

    public class CalculationViewModel
    {
        public CalculationFilter Filter { get; set; }
        public Calculation Item { get; set; }
        public SelectList CalculationTypesSelectList { get; set; }
    }

    public class CalculationDetailViewModel : CalculationViewModel
    {
    }
    #endregion

    public enum CalculationStatuses
    {
        New = 1,
        Configuration = 2,
        Initiated = 3,
        Error = 4,
        Done = 5
    }
}

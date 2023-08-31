using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models.ASM.Filsters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SmartEco.Models.ASM
{
    public class Enterprise
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{12}", ErrorMessage = "БИН должен содержать 12 цифр")]
        [Display(Name = "БИН (ИНН)")]
        public string Bin { get; set; }

        [Required]
        [Display(Name = "Наименование предприятия")]
        public string Name { get; set; }

        [Display(Name = "Город или регион (КАТО)")]
        public KatoEnterprise Kato { get; set; }

        [Display(Name = "Тип предприятия")]
        public int? EnterpriseTypeId { get; set; }
        [Display(Name = "Тип предприятия")]
        public EnterpriseType EnterpriseType { get; set; }

        [Display(Name = "Контакты")]
        public string Contacts { get; set; }
    }

    public class KatoEnterprise
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }

        public int EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }

        public string ComplexName => $"{Code} {Address}";
    }

    #region ViewModels
    public class EnterpriseListViewModel
    {
        public EnterpriseFilter Filter { get; set; }
        public List<Enterprise> Items { get; set; }
        public Pager Pager { get; set; }
        public SelectList EnterpriseTypesSelectList { get; set; }
    }

    public class EnterpriseViewModel
    {
        public EnterpriseFilter Filter { get; set; }
        public Enterprise Item { get; set; }
        public SelectList EnterpriseTypesSelectList { get; set; }
    }

    public class EnterpriseDetailViewModel : EnterpriseViewModel
    {
        public TreeNodes TreeNodes { get; set; }
    }
    #endregion
}
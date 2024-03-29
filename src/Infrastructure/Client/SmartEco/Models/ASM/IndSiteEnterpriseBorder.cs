﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM
{
    public class IndSiteEnterpriseBorder
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Промплощадка предприятия")]
        public int IndSiteEnterpriseId { get; set; }
        [Display(Name = "Промплощадка предприятия")]
        public IndSiteEnterprise IndSiteEnterprise { get; set; }

        [Required]
        [Display(Name = "Координаты")]
        public List<string> Coordinates { get; set; } = new List<string>();
    }

    public class IndSiteEnterpriseBorderListViewModel
    {
        public List<IndSiteEnterpriseBorder> Items { get; set; } = new List<IndSiteEnterpriseBorder>();
        public int IndSiteEnterpriseId { get; set; }
        public int EnterpriseId { get; set; }
    }
}
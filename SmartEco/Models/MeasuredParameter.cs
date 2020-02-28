using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class MeasuredParameter
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameterUnit")]
        public int? MeasuredParameterUnitId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasuredParameterUnit")]
        public MeasuredParameterUnit MeasuredParameterUnit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = NameRU + (MeasuredParameterUnit != null ? ", " + MeasuredParameterUnit.Name : "");
                if (language == "kk")
                {
                    name = NameKK + (MeasuredParameterUnit != null ? ", " + MeasuredParameterUnit.Name : "");
                }
                if (language == "en")
                {
                    name = NameEN + (MeasuredParameterUnit != null ? ", " + MeasuredParameterUnit.Name : "");
                }
                return name;
            }
        }

        [Display(Name = "EcomonCode")]
        public int? EcomonCode { get; set; }

        [Display(Name = "OceanusCode")]
        public string OceanusCode { get; set; }

        [Display(Name = "KazhydrometCode")]
        public string KazhydrometCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPC")]
        public decimal? MPC { get; set; } // maximum permissible concentration

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCMaxSingle")]
        public decimal? MPCMaxSingle { get; set; }
    }
}

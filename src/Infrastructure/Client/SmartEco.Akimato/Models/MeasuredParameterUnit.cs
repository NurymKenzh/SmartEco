﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Akimato.Models
{
    public class MeasuredParameterUnit
    {
        public int Id { get; set; }

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
                    name = NameRU;
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "en")
                {
                    name = NameEN;
                }
                return name;
            }
        }
    }
}

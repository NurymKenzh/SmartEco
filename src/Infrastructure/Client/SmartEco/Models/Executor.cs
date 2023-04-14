using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class Executor
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Position")]
        public string Position { get; set; }
    }
}

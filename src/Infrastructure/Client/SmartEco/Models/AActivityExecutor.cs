using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class AActivityExecutor
    {
        public int Id { get; set; }

        public int AActivityId { get; set; }
        public AActivity AActivity { get; set; }

        public int ExecutorId { get; set; }
        public Executor Executor { get; set; }
        
        [Range(0, 100, ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "ErrorNumberRangeMustBe")]
        public decimal? Contribution { get; set; }
    }
}

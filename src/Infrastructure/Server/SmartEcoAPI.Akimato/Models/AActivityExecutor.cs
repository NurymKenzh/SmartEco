using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class AActivityExecutor
    {
        public int Id { get; set; }

        public int AActivityId { get; set; }
        public AActivity AActivity { get; set; }

        public int ExecutorId { get; set; }
        public Executor Executor { get; set; }

        public decimal? Contribution { get; set; }
    }
}

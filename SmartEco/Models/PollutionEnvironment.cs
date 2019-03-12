using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class PollutionEnvironment
    {
        public int Id { get; set; }
        public string NameKK { get; set; }
        public string NameRU { get; set; }
        public string NameEN { get; set; }
        public string Name
        {
            get
            {
                // переписать (пример в class MeasuredParameter)
                return NameRU;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class Target
    {
        public int Id { get; set; }
        
        public PollutionEnvironment PollutionEnvironment { get; set; }
        public int PollutionEnvironmentId { get; set; }
        
        public string NameKK { get; set; }
        
        public string NameRU { get; set; }
        
        public string NameEN { get; set; }
        
        public bool TypeOfAchievement { get; set; }
        
        public MeasuredParameterUnit MeasuredParameterUnit { get; set; }
        public int MeasuredParameterUnitId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class Layer
    {
        public int Id { get; set; }

        public string GeoServerWorkspace { get; set; }
        public string GeoServerName { get; set; }

        public string NameKK { get; set; }
        public string NameRU { get; set; }
        public string NameEN { get; set; }

        public PollutionEnvironment PollutionEnvironment { get; set; }
        public int? PollutionEnvironmentId { get; set; }

        public MeasuredParameter MeasuredParameter { get; set; }
        public int? MeasuredParameterId { get; set; }

        public KATO KATO { get; set; }
        public int? KATOId { get; set; }

        public Season? Season { get; set; }

        public int? Hour { get; set; }
    }
}

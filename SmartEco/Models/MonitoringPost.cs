using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class MonitoringPost
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public string AdditionalInformation { get; set; }

        public DataProvider DataProvider { get; set; }
        public int DataProviderId { get; set; }

        public PollutionEnvironment PollutionEnvironment { get; set; }
        public int PollutionEnvironmentId { get; set; }
    }
}

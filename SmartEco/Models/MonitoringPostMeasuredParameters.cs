using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class MonitoringPostMeasuredParameters
    {
        public int Id { get; set; }
        public int MonitoringPostId { get; set; }
        public MonitoringPost MonitoringPost { get; set; }
        public bool Sensor { get; set; }
        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }
        public string MeasuredParameterNameRU { get; set; }
        public string MeasuredParameterNameKK { get; set; }
        public string MeasuredParameterNameEN { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string MeasuredParameterName
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = MeasuredParameterNameRU;
                if (language == "kk")
                {
                    name = MeasuredParameterNameKK;
                }
                if (language == "en")
                {
                    name = MeasuredParameterNameEN;
                }
                return name;
            }
        }
    }
}

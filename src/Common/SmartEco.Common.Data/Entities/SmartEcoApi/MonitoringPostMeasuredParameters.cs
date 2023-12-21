using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEco.Common.Data.Entities.SmartEcoApi
{
    public class MonitoringPostMeasuredParameters
    {
        public int Id { get; set; }
        public int MonitoringPostId { get; set; }
        public required MonitoringPost MonitoringPost { get; set; }
        [NotMapped]
        public bool Sensor { get; set; }
        public int MeasuredParameterId { get; set; }
        public required MeasuredParameter MeasuredParameter { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? MinMeasuredValue { get; set; }
        public decimal? MaxMeasuredValue { get; set; }
        public decimal? Coefficient { get; set; }
    }
}

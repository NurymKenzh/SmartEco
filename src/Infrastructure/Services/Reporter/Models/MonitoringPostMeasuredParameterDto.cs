namespace Reporter.Models
{
    internal class MonitoringPostMeasuredParameterDto
    {
        public int MonitoringPostId { get; set; }
        public int MeasuredParameterId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}

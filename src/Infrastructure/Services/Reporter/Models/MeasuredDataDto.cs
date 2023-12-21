namespace Reporter.Models
{
    internal class MeasuredDataDto
    {
        public long Id { get; set; }
        public int MeasuredParameterId { get; set; }
        public DateTime? DateTime { get; set; }
        public decimal? Value { get; set; }
        public int? MonitoringPostId { get; set; }
    }
}

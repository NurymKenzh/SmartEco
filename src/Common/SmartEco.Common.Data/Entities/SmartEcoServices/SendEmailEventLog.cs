namespace SmartEco.Common.Data.Entities.SmartEcoServices
{
    public class SendEmailEventLog
    {
        public DateTime SendedOn { get; set; }
        public int? MonitoringPostId { get; set; }
        public int? MeasuredParameterId { get; set; }
        public string? Information { get; set; }
    }
}

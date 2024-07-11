namespace SmartEco.Models.ASM.Responses.Reports
{
    public class ReportDownloadResponse
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
    }
}

namespace SmartEcoAPI.Models.ASM.Responses.Reports
{
    public class ReportDownloadResponse
    {
        public ReportDownloadResponse(byte[] fileData, string fileName, string contentType)
        {
            FileData = fileData;
            FileName = fileName;
            ContentType = contentType;
        }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
    }
}

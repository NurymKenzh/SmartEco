namespace SmartEcoAPI.Models.ASM.Requests
{
    public class BaseRequest
    {
        public string SortOrder { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}

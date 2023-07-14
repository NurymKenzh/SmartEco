namespace SmartEco.Models.ASM.Requests
{
    public class EnterprisesRequest : BaseRequest
    {
        public long? Bin { get; set; }
        public string Name { get; set; }
        public string KatoComplex { get; set; }
        public int? EnterpriseTypeId { get; set; }
    }
}

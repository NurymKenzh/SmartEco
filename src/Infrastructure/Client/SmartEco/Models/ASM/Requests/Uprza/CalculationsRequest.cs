namespace SmartEco.Models.ASM.Requests
{
    public class CalculationsRequest : BaseRequest
    {
        public string Name { get; set; }
        public int? CalculationTypeId { get; set; }
        public int? CalculationStatusId { get; set; }
        public string KatoComplex { get; set; }
    }
}

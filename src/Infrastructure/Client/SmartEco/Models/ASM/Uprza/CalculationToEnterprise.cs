namespace SmartEco.Models.ASM.Uprza
{
    public class CalculationToEnterprise
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }
    }
}

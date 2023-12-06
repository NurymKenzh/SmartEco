namespace SmartEco.Models.ASM.Filsters
{
    public class CalculationFilter : BaseFilter
    {
        public string NameFilter { get; set; }
        public int? CalculationTypeIdFilter { get; set; }
        public int? CalculationStatusIdFilter { get; set; }
        public string KatoComplexFilter { get; set; }
    }

    public class CalculationFilterId : CalculationFilter
    {
        public int? Id { get; set; }
    }
}
namespace SmartEco.Models.ASM.Filsters
{
    public class EnterpriseFilter : BaseFilter
    {
        public long? BinFilter { get; set; }
        public string NameFilter { get; set; }
        public string KatoComplexFilter { get; set; }
        public int? EnterpriseTypeIdFilter { get; set; }
    }

    public class EnterpriseFilterId : EnterpriseFilter
    {
        public int? Id { get; set; }
    }
}
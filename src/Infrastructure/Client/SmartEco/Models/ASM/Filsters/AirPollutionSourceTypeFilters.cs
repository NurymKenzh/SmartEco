namespace SmartEco.Models.ASM.Filsters
{
    public class EnterpriseTypeFilter : BaseFilter
    {
        public string NameFilter { get; set; }
    }

    public class EnterpriseTypeFilterId : EnterpriseTypeFilter
    {
        public int? Id { get; set; }
    }
}
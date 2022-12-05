using SmartEco.Common.Models.Filters.Directories;

namespace SmartEco.API.Models.Filters.Directories
{
    public class PersonFilter : BaseFilter, IPersonFilter
    {
        public string Email { get; set; }
        public int? RoleId { get; set; }
        public string SortOrder { get; set; }
    }
}

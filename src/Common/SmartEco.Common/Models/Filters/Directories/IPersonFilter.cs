
namespace SmartEco.Common.Models.Filters.Directories
{
    public interface IPersonFilter : ISorter
    {
        public string? Email { get; set; }
        public int? RoleId { get; set; }
    }
}

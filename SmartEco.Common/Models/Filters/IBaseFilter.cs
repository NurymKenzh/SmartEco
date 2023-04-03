
namespace SmartEco.Common.Models.Filters
{
    public interface IBaseFilter
    {
        public int? PageNumber { get; }
        public int? PageSize { get; }
    }
}

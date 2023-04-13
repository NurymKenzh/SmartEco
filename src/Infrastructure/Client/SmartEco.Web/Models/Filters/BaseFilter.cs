using SmartEco.Common.Models.Filters;

namespace SmartEco.Web.Models.Filters
{
    public record BaseFilter : IBaseFilter
    {
        public BaseFilter(int? page = null, int? pageSize = null)
        {
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);

            PageNumber = page is null or 0 ? 1 : (int)page;
            PageSize = pageSize is null or 0 ? Convert.ToInt32(pageSizeList.Min(p => p.Value)) : (int)pageSize;
        }

        public int? PageNumber { get; init; }
        public int? PageSize { get; init; }
    }
}

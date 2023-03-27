using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartEco.Web.Models
{
    public class Pager
    {
        public Pager(int totalItems = 0, int? page = null, int? pageSize = null)
        {
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);

            pageSize = pageSize is null or 0 ? Convert.ToInt32(pageSizeList.Min(p => p.Value)) : (int)pageSize;
            var currentPage = page is null or 0 ? 1 : (int)page;
            var totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            PageNumber = currentPage;
            PageSize = (int)pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            PageSizeList = new SelectList(pageSizeList
                .OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), 
                "Key", "Value", pageSize);
        }

        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public SelectList PageSizeList { get; set; }
    }

    public record ViewPager(object BaseFilter, Pager Pager);
}

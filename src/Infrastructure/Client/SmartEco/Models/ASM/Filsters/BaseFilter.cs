using System.Data.SqlClient;

namespace SmartEco.Models.ASM.Filsters
{
    public class BaseFilter
    {
        public string SortOrder { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public static class FilterProcess
    {
        public static string FilterSorting(this string filterName, string sortOrder)
        {
            return sortOrder == filterName.TrimFilter() ? filterName.TrimFilterDesc() : filterName.TrimFilter();
        }

        private static string TrimFilter(this string filterName)
        {
            return filterName.Replace("Filter", "");
        }

        private static string TrimFilterDesc(this string filterName)
        {
            return filterName.Replace("Filter", "Desc");
        }
    }
}

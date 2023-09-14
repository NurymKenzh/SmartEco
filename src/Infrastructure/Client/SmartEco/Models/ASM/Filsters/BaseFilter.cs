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
        public static string Sorting(this string sortName, string sortOrder)
            => sortOrder == sortName.TrimSort() ? sortName.TrimSortDesc() : sortName.TrimSort();
        public static string FilterSorting(this string filterName, string sortOrder)
            => sortOrder == filterName.TrimFilter() ? filterName.TrimFilterDesc() : filterName.TrimFilter();

        private static string TrimFilter(this string filterName)
            => filterName.Replace("Filter", "");

        private static string TrimFilterDesc(this string filterName)
            => filterName.Replace("Filter", "Desc");

        private static string TrimSort(this string filterName)
            => filterName.Replace("Sort", "");

        private static string TrimSortDesc(this string filterName)
            => filterName.Replace("Sort", "Desc");
    }
}

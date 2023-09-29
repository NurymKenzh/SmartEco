using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartEco.Models.ASM.Filsters
{
    public class Pager
    {
        private static int _rowNumber;

        public Pager(int? pageNumber, int? pageSize = null)
        {
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);
            var pageSizeSelectList = new SelectList(pageSizeList.OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), "Key", "Value");

            var currentPage = pageNumber is null ? 1 : pageNumber.Value;
            pageSize = pageSize is null ? Convert.ToInt32(pageSizeList.Min(p => p.Value)) : pageSize.Value;
            _rowNumber = (int)(currentPage * pageSize - pageSize);

            PageNumber = currentPage;
            PageSize = (int)pageSize;
            PageSizeList = pageSizeSelectList;
        }

        public Pager(int totalItems, int? pageNumber, int? pageSize = null)
        {
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);
            var pageSizeSelectList = new SelectList(pageSizeList.OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), "Key", "Value");

            var currentPage = pageNumber.GetValueOrDefault() is 0 ? 1 : pageNumber.Value;
            pageSize = pageSize is null ? Convert.ToInt32(pageSizeList.Min(p => p.Value)) : pageSize.Value;
            var totalPages = pageSize is null ? 1 : (int)Math.Ceiling(totalItems / (decimal)pageSize);
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

            _rowNumber = (int)(currentPage * pageSize - pageSize);

            PageNumber = currentPage;
            PageSize = (int)pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            PageSizeList = pageSizeSelectList;
        }

        public int TotalItems { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public SelectList PageSizeList { get; private set; }

        public int RowNumber { get { return GetCurrentRowNumber(); } }
        private int GetCurrentRowNumber()
            => ++_rowNumber;
    }
}

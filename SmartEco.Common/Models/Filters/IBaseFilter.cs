using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Models.Filters
{
    public interface IBaseFilter
    {
        public int? PageNumber { get; }
        public int? PageSize { get; }
    }
}

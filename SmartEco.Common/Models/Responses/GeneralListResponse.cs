using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Models.Responses
{
    public record GeneralListResponse<T>(IEnumerable<T> Objects, int CountTotal);
}

using SmartEco.Common.Models.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartEco.Gateway.Models.Filters
{
    public class BaseFilter : IBaseFilter
    {
        [SwaggerParameter("Номер возвращаемого блока")]
        public int? PageNumber { get; set; }

        [SwaggerParameter("Количество возвращаемых данных в блоке")]
        public int? PageSize { get; set; }
    }
}

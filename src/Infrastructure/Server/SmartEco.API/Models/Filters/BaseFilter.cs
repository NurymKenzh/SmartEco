using SmartEco.Common.Models.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartEco.API.Models.Filters
{
    public class BaseFilter : IBaseFilter
    {
        [SwaggerParameter("Номер возвращаемого блока")]
        public int? PageNumber { get; set; }

        [SwaggerParameter("Количество возвращаемых данных в блоке")]
        public int? PageSize { get; set; }
    }
}

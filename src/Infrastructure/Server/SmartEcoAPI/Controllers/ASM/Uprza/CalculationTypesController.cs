using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalculationTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CalculationTypes
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationTypesResponse>> GetCalculationTypes([FromBody] CalculationTypesRequest request)
        {
            var calculationTypes = _context.CalculationType
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                calculationTypes = calculationTypes.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    calculationTypes = calculationTypes.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    calculationTypes = calculationTypes.OrderByDescending(m => m.Name);
                    break;
                default:
                    calculationTypes = calculationTypes.OrderBy(m => m.Id);
                    break;
            }

            var count = await calculationTypes.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                calculationTypes = calculationTypes.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new CalculationTypesResponse(await calculationTypes.ToListAsync(), count);

            return response;
        }
    }
}
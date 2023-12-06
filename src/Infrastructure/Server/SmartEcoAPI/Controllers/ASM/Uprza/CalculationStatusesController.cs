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
    public class CalculationStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalculationStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CalculationStatuses
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationStatusesResponse>> GetCalculationStatuses([FromBody] CalculationStatusesRequest request)
        {
            var calculationStatuses = _context.CalculationStatus
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                calculationStatuses = calculationStatuses.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    calculationStatuses = calculationStatuses.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    calculationStatuses = calculationStatuses.OrderByDescending(m => m.Name);
                    break;
                default:
                    calculationStatuses = calculationStatuses.OrderBy(m => m.Id);
                    break;
            }

            var count = await calculationStatuses.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                calculationStatuses = calculationStatuses.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new CalculationStatusesResponse(await calculationStatuses.ToListAsync(), count);

            return response;
        }
    }
}
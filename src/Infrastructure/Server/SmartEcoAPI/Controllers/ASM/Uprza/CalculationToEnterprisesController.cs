using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using SmartEcoAPI.Models.ASM.Uprza;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationToEnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalculationToEnterprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CalculationToEnterprises
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToEnterprisesResponse>> GetCalculationToEnterprises([FromBody] CalculationToEnterprisesRequest request)
        {
            var calcToEnts = _context.CalculationToEnterprise
                .Include(c => c.Calculation)
                .Include(c => c.Enterprise)
                .Where(m => true);

            if (request.CalculationId != null)
            {
                calcToEnts = calcToEnts.Where(c => c.CalculationId == request.CalculationId);
            }

            var count = await calcToEnts.CountAsync();
            var response = new CalculationToEnterprisesResponse(await calcToEnts.ToListAsync(), count);

            return response;
        }

        // GET: api/CalculationToEnterprises/5
        [HttpGet("{calculationId}/{enterpriseId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToEnterprise>> GetCalculationToEnterprise(
            int calculationId,
            int enterpriseId)
        {
            var calcToEnt = await _context.CalculationToEnterprise
                .Include(c => c.Calculation)
                .Include(c => c.Enterprise)
                .FirstOrDefaultAsync(c => c.CalculationId == calculationId && c.EnterpriseId == enterpriseId);

            if (calcToEnt == null)
            {
                return NotFound();
            }

            return calcToEnt;
        }

        // POST: api/CalculationToEnterprises
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToEnterprise>> PostCalculationToEnterprise(CalculationToEnterprise calcToEnt)
        {
            _context.CalculationToEnterprise.Add(calcToEnt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCalculationToEnterprise", new 
            { 
                calculationId = calcToEnt.CalculationId, 
                enterpriseId = calcToEnt.EnterpriseId 
            }, calcToEnt);
        }

        // DELETE: api/CalculationToEnterprises/5
        [HttpDelete("{calculationId}/{enterpriseId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToEnterprise>> DeleteCalculationToEnterprise(
            int calculationId,
            int enterpriseId)
        {
            var calcToEnt = await _context.CalculationToEnterprise.FindAsync(calculationId, enterpriseId);
            if (calcToEnt == null)
            {
                return NotFound();
            }

            _context.CalculationToEnterprise.Remove(calcToEnt);
            await _context.SaveChangesAsync();

            return calcToEnt;
        }
    }
}
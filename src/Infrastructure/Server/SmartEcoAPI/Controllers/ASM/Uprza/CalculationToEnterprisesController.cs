using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using SmartEcoAPI.Models.ASM.Uprza;
using SmartEcoAPI.Services;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationToEnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public CalculationToEnterprisesController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
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
            await _calcService.UpdateStatus(calcToEnt.CalculationId, CalculationStatuses.Configuration);

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

            var calcToSrcs = GetCalcToSourcesByEnterprise(calculationId, enterpriseId); //get calcRoSrcs for relation cascade delete
            _context.CalculationToSource.RemoveRange(calcToSrcs);

            _context.CalculationToEnterprise.Remove(calcToEnt);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calculationId, CalculationStatuses.Configuration);

            return calcToEnt;
        }

        private IQueryable<CalculationToSource> GetCalcToSourcesByEnterprise(
            int calculationId,
            int enterpriseId)
        {
            var idAirPollutionSources = _context.AirPollutionSource
                .Include(a => a.SourceIndSite.IndSiteEnterprise.Enterprise)
                .Include(a => a.SourceWorkshop.Workshop.IndSiteEnterprise.Enterprise)
                .Include(a => a.SourceArea.Area.Workshop.IndSiteEnterprise.Enterprise)
                .Where(a => a.SourceIndSite.IndSiteEnterprise.EnterpriseId == enterpriseId ||
                    a.SourceWorkshop.Workshop.IndSiteEnterprise.EnterpriseId == enterpriseId ||
                    a.SourceArea.Area.Workshop.IndSiteEnterprise.EnterpriseId == enterpriseId)
                .Select(a => a.Id);

            var calcToSrcs = _context.CalculationToSource
                .Where(c => c.CalculationId == calculationId &&
                    idAirPollutionSources.Contains(c.SourceId));

            return calcToSrcs;
        }
    }
}
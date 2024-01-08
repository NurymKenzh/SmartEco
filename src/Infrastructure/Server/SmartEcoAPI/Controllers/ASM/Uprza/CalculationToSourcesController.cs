using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using SmartEcoAPI.Models.ASM.Uprza;
using SmartEcoAPI.Services;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationToSourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public CalculationToSourcesController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        // GET: api/CalculationToSources
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSourcesResponse>> GetCalculationToSources([FromBody] CalculationToSourcesRequest request)
        {
            var calcToSrcs = GetCalculationToSources(request.CalculationId);
            var pollutionSourcesResp = await GetAirPollutionSources(request); //get pollution sources by enterprises
            var involvedSources = MapToInvolvedSources(pollutionSourcesResp.AirPollutionSources, calcToSrcs);

            var isInvolvedAllSources = involvedSources.All(s => s.IsInvolved) && involvedSources.Count != 0;

            if (request.PageSize != null && request.PageNumber != null)
            {
                involvedSources = involvedSources.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize).ToList();
            }

            var response = new CalculationToSourcesResponse(involvedSources, isInvolvedAllSources, pollutionSourcesResp.Count);

            return response;
        }

        // GET: api/CalculationToSources/5
        [HttpGet("{calculationId}/{sourceId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSource>> GetCalculationToSource(
            int calculationId,
            int sourceId)
        {
            var calcToSrc = await _context.CalculationToSource
                .Include(c => c.Calculation)
                .Include(c => c.Source)
                .FirstOrDefaultAsync(c => c.CalculationId == calculationId && c.SourceId == sourceId);

            if (calcToSrc == null)
            {
                return NotFound();
            }

            return calcToSrc;
        }

        // POST: api/CalculationToSources
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSource>> PostCalculationToSource(CalculationToSource calcToSrc)
        {
            _context.CalculationToSource.Add(calcToSrc);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calcToSrc.CalculationId, CalculationStatuses.Configuration);

            return CreatedAtAction("GetCalculationToSource", new 
            { 
                calculationId = calcToSrc.CalculationId,
                sourceId = calcToSrc.SourceId 
            }, calcToSrc);
        }

        // DELETE: api/CalculationToSources/4/90
        [HttpDelete("{calculationId}/{sourceId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSource>> DeleteCalculationToSource(
            int calculationId,
            int sourceId)
        {
            var calcToSrc = await _context.CalculationToSource.FindAsync(calculationId, sourceId);
            if (calcToSrc == null)
            {
                return NotFound();
            }

            _context.CalculationToSource.Remove(calcToSrc);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calcToSrc.CalculationId, CalculationStatuses.Configuration);

            return calcToSrc;
        }

        // POST: api/CalculationToSources/SelectAll
        [HttpPost("SelectAll")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSource>> SelectAllSources(CalculationToSourcesRequest request)
        {
            var calcToSrcs = GetCalculationToSources(request.CalculationId);
            var pollutionSourcesResp = await GetAirPollutionSources(request); //get pollution sources by enterprises
            var uninvolvedSources = MapToInvolvedSources(pollutionSourcesResp.AirPollutionSources, calcToSrcs)
                .Where(s => !s.IsInvolved)
                .ToList();
            var calcToUninvolvedSrcs = uninvolvedSources
                .Select(s => new CalculationToSource
                {
                    CalculationId = (int)request.CalculationId,
                    SourceId = s.Id
                })
                .ToList();

            await _context.CalculationToSource.AddRangeAsync(calcToUninvolvedSrcs);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus((int)request.CalculationId, CalculationStatuses.Configuration);

            return Ok();
        }

        // DELETE: api/CalculationToSources/4
        [HttpDelete("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationToSource>> DeleteAllSources(int calculationId)
        {
            var calcToSrcs = GetCalculationToSources(calculationId);

            _context.CalculationToSource.RemoveRange(calcToSrcs);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calculationId, CalculationStatuses.Configuration);

            return Ok();
        }

        private IQueryable<CalculationToSource> GetCalculationToSources(int? calculationId)
        {
            var calcToSrcs = _context.CalculationToSource
                .Where(m => true);

            if (calculationId != null)
            {
                calcToSrcs = calcToSrcs.Where(c => c.CalculationId == calculationId);
            }
            return calcToSrcs;
        }

        private async Task<AirPollutionSourcesResponse> GetAirPollutionSources(CalculationToSourcesRequest request)
        {
            var enterpriseIds = request.EnterpriseIds;
            if (enterpriseIds is null)
                return new AirPollutionSourcesResponse(new List<AirPollutionSource>(), 0);

            var airPollutionSources = _context.AirPollutionSource
                .Include(a => a.Type)
                .Include(a => a.SourceInfo)
                    .ThenInclude(a => a.RelationBackground)
                .Include(a => a.SourceIndSite.IndSiteEnterprise.Enterprise)
                .Include(a => a.SourceWorkshop.Workshop.IndSiteEnterprise.Enterprise)
                .Include(a => a.SourceArea.Area.Workshop.IndSiteEnterprise.Enterprise)
                .Include(a => a.OperationModes)
                    .ThenInclude(mode => mode.GasAirMixture)
                .Include(a => a.OperationModes)
                    .ThenInclude(mode => mode.Emissions)
                        .ThenInclude(e => e.Pollutant)
                .Where(m => true);

            airPollutionSources = airPollutionSources
                .Where(a => enterpriseIds.Contains(a.SourceIndSite.IndSiteEnterprise.EnterpriseId) ||
                    enterpriseIds.Contains(a.SourceWorkshop.Workshop.IndSiteEnterprise.EnterpriseId) ||
                    enterpriseIds.Contains(a.SourceArea.Area.Workshop.IndSiteEnterprise.EnterpriseId));

            var count = await airPollutionSources.CountAsync();
            var response = new AirPollutionSourcesResponse(await airPollutionSources.ToListAsync(), count);

            return response;
        }

        private static List<AirPollutionSourceInvolved> MapToInvolvedSources(
            List<AirPollutionSource> airPollutionSources, 
            IQueryable<CalculationToSource> calcToSrcs)
        {
            var json = JsonConvert.SerializeObject(airPollutionSources, Formatting.Indented, //serialize list for downcasting
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var involvedSources = JsonConvert.DeserializeObject<List<AirPollutionSourceInvolved>>(json); //downcasting to involved objects
            var calcToSrcIds = calcToSrcs //select only id from calculation to sources
                .Select(c => c.SourceId)
                .ToList();
            involvedSources = involvedSources //updating property 'isInvolved' by ids
                .Select(s =>
                {
                    s.IsInvolved = calcToSrcIds.Contains(s.Id);
                    return s;
                })
                .OrderByDescending(s => s.IsInvolved)
                .ToList();
            return involvedSources;
        }
    }
}
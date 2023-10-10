using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using Org.BouncyCastle.Asn1.Ocsp;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM.PollutionSources
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,moderator,ASM")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AirPollutionSourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirPollutionSourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AirPollutionSources
        [HttpGet]
        public async Task<ActionResult<AirPollutionSourcesResponse>> GetAirPollutionSources([FromBody] AirPollutionSourcesRequest request)
        {
            var airPollutionSources = _context.AirPollutionSource
                .Include(a => a.Type)
                .Include(a => a.SourceInfo)
                .Include(a => a.SourceIndSite.IndSiteEnterprise.IndSiteBorder)
                .Include(a => a.SourceWorkshop.Workshop.IndSiteEnterprise.IndSiteBorder)
                .Include(a => a.SourceArea.Area.Workshop.IndSiteEnterprise.IndSiteBorder)
                .Include(a => a.OperationModes)
                    .ThenInclude(mode => mode.GasAirMixture)
                .Where(m => true);

            if (request?.EnterpriseId != null)
            {
                airPollutionSources = airPollutionSources
                    .Where(a => a.SourceIndSite.IndSiteEnterprise.EnterpriseId == request.EnterpriseId ||
                        a.SourceWorkshop.Workshop.IndSiteEnterprise.EnterpriseId == request.EnterpriseId ||
                        a.SourceArea.Area.Workshop.IndSiteEnterprise.EnterpriseId == request.EnterpriseId);
            }
            if (request?.Number != null)
            {
                airPollutionSources = airPollutionSources.Where(a => a.Number.StartsWith(request.Number));
            }
            if (request?.Name != null)
            {
                airPollutionSources = airPollutionSources.Where(a => a.Name.ToLower().StartsWith(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    airPollutionSources = airPollutionSources.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    airPollutionSources = airPollutionSources.OrderByDescending(m => m.Name);
                    break;
                case "Number":
                    airPollutionSources = airPollutionSources.OrderBy(m => m.Number);
                    break;
                case "NumberDesc":
                    airPollutionSources = airPollutionSources.OrderByDescending(m => m.Number);
                    break;
                case "Relation":
                    airPollutionSources = airPollutionSources.OrderBy(m => m.RelationCombine);
                    break;
                case "RelationDesc":
                    airPollutionSources = airPollutionSources.OrderByDescending(m => m.RelationCombine);
                    break;
                default:
                    airPollutionSources = airPollutionSources.OrderBy(m => m.Id);
                    break;
            }


            //Load data for use RelationCombine field
            await airPollutionSources.LoadAsync();
            if (request?.Relation != null)
            {
                airPollutionSources = airPollutionSources.Where(a => a.RelationCombine.ToLower().Contains(request.Relation.ToLower()));
            }
            if (request.SortOrder?.Contains("Relation") is true)
            {
                if (request.SortOrder.Contains("Desc"))
                    airPollutionSources = airPollutionSources.OrderByDescending(m => m.RelationCombine);
                else
                    airPollutionSources = airPollutionSources.OrderBy(m => m.RelationCombine);
            }

            var count = airPollutionSources.Count();
            if (request.PageSize != null && request.PageNumber != null)
            {
                airPollutionSources = airPollutionSources.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new AirPollutionSourcesResponse(await airPollutionSources.ToListAsync(), count);

            return response;
        }

        // GET: api/AirPollutionSources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AirPollutionSource>> GetAirPollutionSource(int id)
        {
            var airPollutionSource = await _context.AirPollutionSource
                .Include(a => a.Type)
                .Include(a => a.SourceInfo)
                .Include(a => a.SourceIndSite)
                .Include(a => a.SourceWorkshop)
                .Include(a => a.SourceArea)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (airPollutionSource == null)
            {
                return NotFound();
            }

            return airPollutionSource;
        }

        // PUT: api/AirPollutionSources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirPollutionSource(int id, AirPollutionSource airPollutionSource)
        {
            if (id != airPollutionSource.Id || airPollutionSource.Relation is SourceRelations.Undefined || airPollutionSource.SourceInfo is null)
                return BadRequest();

            var numberDb = _context.AirPollutionSource
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id)?.Number;
            if (numberDb == null)
                return NotFound();

            //If Number changed
            if (numberDb != airPollutionSource.Number)
            {
                var checkNumber = CheckSourceNumber(airPollutionSource);
                if (checkNumber.StatusCode == StatusCodes.Status409Conflict)
                    return Conflict();
            }

            //Update entries
            UpdateRelation(airPollutionSource);
            _context.Entry(airPollutionSource).State = EntityState.Modified;
            _context.Entry(airPollutionSource.SourceInfo).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/AirPollutionSources
        [HttpPost]
        public async Task<ActionResult<AirPollutionSource>> PostAirPollutionSource(AirPollutionSource airPollutionSource)
        {
            if (airPollutionSource.Relation is SourceRelations.Undefined || airPollutionSource.SourceInfo is null)
                return BadRequest();

            var checkNumber = CheckSourceNumber(airPollutionSource);
            if (checkNumber.StatusCode == StatusCodes.Status409Conflict)
                return Conflict();

            _context.AirPollutionSource.Add(airPollutionSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirPollutionSource", new { id = airPollutionSource.Id }, airPollutionSource);
        }

        // DELETE: api/AirPollutionSources/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AirPollutionSource>> DeleteAirPollutionSource(int id)
        {
            var airPollutionSource = await _context.AirPollutionSource.FindAsync(id);
            if (airPollutionSource == null)
            {
                return NotFound();
            }

            _context.AirPollutionSource.Remove(airPollutionSource);
            await _context.SaveChangesAsync();

            return airPollutionSource;
        }

        [HttpGet("[action]/{enterpriseId}")]
        public async Task<ActionResult<AirPollutinSourceLastNumberResponse>> GetLastNumber(int enterpriseId)
        {
            var airPollutionSources = GetSourcesByEnterprise(enterpriseId);
            var maxNumber = await airPollutionSources.DefaultIfEmpty().MaxAsync(a => Convert.ToInt32(a.Number));
            var count = await airPollutionSources.CountAsync();

            var response = new AirPollutinSourceLastNumberResponse(maxNumber, count);
            return response;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<RelationBackground>>> GetRelationBackgrounds()
            => await _context.RelationBackground
                .Where(r => true)
                .ToListAsync();

        private StatusCodeResult CheckSourceNumber(AirPollutionSource airPollutionSource)
        {
            int enterpriseId;
            if (airPollutionSource.Relation is SourceRelations.IndSite)
            {
                enterpriseId = _context.IndSiteEnterprise
                    .Include(i => i.Enterprise)
                    .Where(i => i.Id == airPollutionSource.SourceIndSite.IndSiteEnterpriseId)
                    .Single().EnterpriseId;
            }
            else if (airPollutionSource.Relation is SourceRelations.Workshop)
            {
                enterpriseId = _context.Workshop
                    .Include(i => i.IndSiteEnterprise.Enterprise)
                    .Where(i => i.Id == airPollutionSource.SourceWorkshop.WorkshopId)
                    .Single().IndSiteEnterprise.EnterpriseId;
            }
            else
            {
                enterpriseId = _context.Area
                    .Include(i => i.Workshop.IndSiteEnterprise.Enterprise)
                    .Where(i => i.Id == airPollutionSource.SourceArea.AreaId)
                    .Single().Workshop.IndSiteEnterprise.EnterpriseId;
            }

            var airPollutionSources = GetSourcesByEnterprise(enterpriseId);
            if (airPollutionSources.Any(a => a.Number == airPollutionSource.Number))
                return Conflict();

            return Ok();
        }

        private IQueryable<AirPollutionSource> GetSourcesByEnterprise(int enterpriseId)
            => _context.AirPollutionSource
                .Where(a => a.SourceIndSite.IndSiteEnterprise.EnterpriseId == enterpriseId
                || a.SourceWorkshop.Workshop.IndSiteEnterprise.EnterpriseId == enterpriseId
                || a.SourceArea.Area.Workshop.IndSiteEnterprise.EnterpriseId == enterpriseId);

        private void UpdateRelation(AirPollutionSource airPollutionSource)
        {
            try
            {
                var id = airPollutionSource.Id;
                var isSourceIndSite = _context.AirPollutionSourceIndSite.Any(a => a.AirPollutionSourceId == id);
                var isSourceWorkshop = _context.AirPollutionSourceWorkshop.Any(a => a.AirPollutionSourceId == id);
                var isSourceArea = _context.AirPollutionSourceArea.Any(a => a.AirPollutionSourceId == id);

                if (airPollutionSource.Relation is SourceRelations.IndSite)
                {
                    if (isSourceIndSite)
                    {
                        _context.Entry(airPollutionSource.SourceIndSite).State = EntityState.Modified;
                        return;
                    }
                    else
                    {
                        _context.Add(airPollutionSource.SourceIndSite);
                    }
                }

                if (airPollutionSource.Relation is SourceRelations.Workshop)
                {
                    if (isSourceWorkshop)
                    {
                        _context.Entry(airPollutionSource.SourceWorkshop).State = EntityState.Modified;
                        return;
                    }
                    else
                    {
                        _context.Add(airPollutionSource.SourceWorkshop);
                    }
                }

                if (airPollutionSource.Relation is SourceRelations.Area)
                {
                    if (isSourceArea)
                    {
                        _context.Entry(airPollutionSource.SourceArea).State = EntityState.Modified;
                        return;
                    }
                    else
                    {
                        _context.Add(airPollutionSource.SourceArea);
                    }
                }

                if (isSourceIndSite)
                    _context.AirPollutionSourceIndSite.Remove(_context.AirPollutionSourceIndSite.First(a => a.AirPollutionSourceId == id));
                if (isSourceWorkshop)
                    _context.AirPollutionSourceWorkshop.Remove(_context.AirPollutionSourceWorkshop.First(a => a.AirPollutionSourceId == id));
                if (isSourceArea)
                    _context.AirPollutionSourceArea.Remove(_context.AirPollutionSourceArea.First(a => a.AirPollutionSourceId == id));
            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }
    }
}
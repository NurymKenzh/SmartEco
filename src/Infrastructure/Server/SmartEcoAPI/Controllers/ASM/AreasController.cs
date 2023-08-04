using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AreasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Areas
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas(AreasRequest request)
        {
            var areas = _context.Area
                .Include(a => a.Workshop)
                .ThenInclude(a => a.IndSiteEnterprise)
                .ThenInclude(a => a.Enterprise)
                .Where(m => true);

            if (request?.EnterpriseId != null)
            {
                areas = areas.Where(a => a.Workshop.IndSiteEnterprise.EnterpriseId == request.EnterpriseId);
            }

            return await areas.ToListAsync();
        }

        // GET: api/Areas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            var area = await _context.Area
                .Include(a => a.Workshop)
                .ThenInclude(a => a.IndSiteEnterprise)
                .ThenInclude(a => a.Enterprise)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return area;
        }

        // PUT: api/Areas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> PutArea(int id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }

            _context.Entry(area).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetEnterpriseId(area.Id);
        }

        // POST: api/Areas
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> PostArea(Area area)
        {
            _context.Area.Add(area);
            await _context.SaveChangesAsync();

            return await GetEnterpriseId(area.Id);
        }

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> DeleteArea(int id)
        {
            var area = await _context.Area.FindAsync(id);
            var enerpriseResponse = await GetEnterpriseId(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Area.Remove(area);
            await _context.SaveChangesAsync();

            return enerpriseResponse;
        }

        private bool AreaExists(int id)
        {
            return _context.Area.Any(e => e.Id == id);
        }

        private async Task<EnterpriseResponse> GetEnterpriseId(int areaId)
        {
            var area = await _context.Area
                .Include(a => a.Workshop)
                .ThenInclude(a => a.IndSiteEnterprise)
                .ThenInclude(a => a.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == areaId);

            return new EnterpriseResponse()
            {
                Id = area.Workshop.IndSiteEnterprise.EnterpriseId
            };
        }
    }
}

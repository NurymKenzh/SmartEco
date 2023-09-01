using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
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
    public class SanZoneEnterpriseBordersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SanZoneEnterpriseBordersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SanZoneEnterpriseBorders
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<SanZoneEnterpriseBorder>>> GetSanZoneEnterpriseBorders(SanZoneEnterpriseBordersRequest request)
        {
            var sanZoneEnterpriseBorders = _context.SanZoneEnterpriseBorder
                .Include(border => border.IndSiteEnterprise)
                .ThenInclude(border => border.Enterprise)
                .Where(border => true);

            if (request?.IndSiteEnterpriseId != null)
            {
                sanZoneEnterpriseBorders = sanZoneEnterpriseBorders.Where(border => border.IndSiteEnterpriseId == request.IndSiteEnterpriseId);
            }

            return await sanZoneEnterpriseBorders
                .OrderBy(border => border.Id)
                .ToListAsync();
        }

        // GET: api/SanZoneEnterpriseBorders/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<SanZoneEnterpriseBorder>> GetSanZoneEnterpriseBorder(int id)
        {
            var sanZoneEnterpriseBorder = await GetFirstOrDefault(id);

            if (sanZoneEnterpriseBorder == null)
            {
                return NotFound();
            }

            return sanZoneEnterpriseBorder;
        }

        // PUT: api/SanZoneEnterpriseBorders/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<SanZoneEnterpriseBorder>> PutSanZoneEnterpriseBorder(int id, SanZoneEnterpriseBorder sanZoneEnterpriseBorder)
        {
            if (id != sanZoneEnterpriseBorder.Id)
            {
                return BadRequest();
            }

            _context.Entry(sanZoneEnterpriseBorder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanZoneEnterpriseBorderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetFirstOrDefault(sanZoneEnterpriseBorder.Id);
        }

        // POST: api/SanZoneEnterpriseBorders
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<SanZoneEnterpriseBorder>> PostSanZoneEnterpriseBorder(SanZoneEnterpriseBorder sanZoneEnterpriseBorder)
        {
            _context.SanZoneEnterpriseBorder.Add(sanZoneEnterpriseBorder);
            await _context.SaveChangesAsync();

            return await GetFirstOrDefault(sanZoneEnterpriseBorder.Id);
        }

        // DELETE: api/SanZoneEnterpriseBorders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> DeleteSanZoneEnterpriseBorder(int id)
        {
            var sanZoneEnterpriseBorder = await _context.SanZoneEnterpriseBorder.FindAsync(id);
            if (sanZoneEnterpriseBorder == null)
            {
                return NotFound();
            }

            _context.SanZoneEnterpriseBorder.Remove(sanZoneEnterpriseBorder);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool SanZoneEnterpriseBorderExists(int id)
        {
            return _context.SanZoneEnterpriseBorder.Any(e => e.Id == id);
        }

        private async Task<SanZoneEnterpriseBorder> GetFirstOrDefault(int id)
            => await _context.SanZoneEnterpriseBorder
                .Include(border => border.IndSiteEnterprise)
                .ThenInclude(border => border.Enterprise)
                .FirstOrDefaultAsync(border => border.Id == id);
    }
}

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
    public class IndSiteEnterpriseBordersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndSiteEnterpriseBordersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IndSiteEnterpriseBorders
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<IndSiteEnterpriseBorder>>> GetIndSiteEnterpriseBorders(IndSiteEnterpriseBordersRequest request)
        {
            var indSiteEnterpriseBorders = _context.IndSiteEnterpriseBorder
                .Include(border => border.IndSiteEnterprise)
                .ThenInclude(border => border.Enterprise)
                .Where(border => true);

            if (request?.IndSiteEnterpriseId != null)
            {
                indSiteEnterpriseBorders = indSiteEnterpriseBorders.Where(border => border.IndSiteEnterpriseId == request.IndSiteEnterpriseId);
            }

            return await indSiteEnterpriseBorders
                .OrderBy(border => border.Id)
                .ToListAsync();
        }

        // GET: api/IndSiteEnterpriseBorders/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterpriseBorder>> GetIndSiteEnterpriseBorder(int id)
        {
            var indSiteEnterpriseBorder = await GetFirstOrDefault(id);

            if (indSiteEnterpriseBorder == null)
            {
                return NotFound();
            }

            return indSiteEnterpriseBorder;
        }

        // PUT: api/IndSiteEnterpriseBorders/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterpriseBorder>> PutIndSiteEnterpriseBorder(int id, IndSiteEnterpriseBorder indSiteEnterpriseBorder)
        {
            if (id != indSiteEnterpriseBorder.Id)
            {
                return BadRequest();
            }

            _context.Entry(indSiteEnterpriseBorder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndSiteEnterpriseBorderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetFirstOrDefault(indSiteEnterpriseBorder.Id);
        }

        // POST: api/IndSiteEnterpriseBorders
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterpriseBorder>> PostIndSiteEnterpriseBorder(IndSiteEnterpriseBorder indSiteEnterpriseBorder)
        {
            _context.IndSiteEnterpriseBorder.Add(indSiteEnterpriseBorder);
            await _context.SaveChangesAsync();

            return await GetFirstOrDefault(indSiteEnterpriseBorder.Id);
        }

        // DELETE: api/IndSiteEnterpriseBorders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> DeleteIndSiteEnterpriseBorder(int id)
        {
            var indSiteEnterpriseBorder = await _context.IndSiteEnterpriseBorder.FindAsync(id);
            if (indSiteEnterpriseBorder == null)
            {
                return NotFound();
            }

            _context.IndSiteEnterpriseBorder.Remove(indSiteEnterpriseBorder);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool IndSiteEnterpriseBorderExists(int id)
        {
            return _context.IndSiteEnterpriseBorder.Any(e => e.Id == id);
        }

        private async Task<IndSiteEnterpriseBorder> GetFirstOrDefault(int id)
            => await _context.IndSiteEnterpriseBorder
                .Include(border => border.IndSiteEnterprise)
                .ThenInclude(border => border.Enterprise)
                .FirstOrDefaultAsync(border => border.Id == id);
    }
}

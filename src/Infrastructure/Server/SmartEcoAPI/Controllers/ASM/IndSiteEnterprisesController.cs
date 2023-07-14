using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IndSiteEnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndSiteEnterprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IndSiteEnterprises
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<IndSiteEnterprise>>> GetIndSiteEnterprises(IndSiteEnterprisesRequest request)
        {
            var indSiteEnterprises = _context.IndSiteEnterprise
                .Include(ind => ind.Enterprise)
                .ThenInclude(ind => ind.Kato)
                .Where(m => true);

            if (request?.EnterpriseId != null)
            {
                indSiteEnterprises = indSiteEnterprises.Where(m => m.EnterpriseId == request.EnterpriseId);
            }

            return await indSiteEnterprises.ToListAsync();
        }

        // GET: api/IndSiteEnterprises/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterprise>> GetIndSiteEnterprise(int id)
        {
            var indSiteEnterprise = await _context.IndSiteEnterprise
                .Include(e => e.Enterprise)
                .ThenInclude(ind => ind.Kato)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (indSiteEnterprise == null)
            {
                return NotFound();
            }

            return indSiteEnterprise;
        }

        // PUT: api/IndSiteEnterprises/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutIndSiteEnterprise(int id, IndSiteEnterprise indSiteEnterprise)
        {
            if (id != indSiteEnterprise.Id)
            {
                return BadRequest();
            }

            _context.Entry(indSiteEnterprise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndSiteEnterpriseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IndSiteEnterprises
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterprise>> PostIndSiteEnterprise(IndSiteEnterprise indSiteEnterprise)
        {
            _context.IndSiteEnterprise.Add(indSiteEnterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIndSiteEnterprise", new { id = indSiteEnterprise.Id }, indSiteEnterprise);
        }

        // DELETE: api/IndSiteEnterprises/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IndSiteEnterprise>> DeleteIndSiteEnterprise(int id)
        {
            var indSiteEnterprise = await _context.IndSiteEnterprise.FindAsync(id);
            if (indSiteEnterprise == null)
            {
                return NotFound();
            }

            _context.IndSiteEnterprise.Remove(indSiteEnterprise);
            await _context.SaveChangesAsync();

            return indSiteEnterprise;
        }

        private bool IndSiteEnterpriseExists(int id)
        {
            return _context.IndSiteEnterprise.Any(e => e.Id == id);
        }
    }
}
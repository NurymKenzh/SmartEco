using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WorkshopsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkshopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Workshops
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<Workshop>>> GetWorkshops(WorkshopsRequest request)
        {
            var workshops = _context.Workshop
                .Include(ind => ind.IndSiteEnterprise)
                .Where(m => true);

            if (request?.IndSiteEnterpriseId != null)
            {
                workshops = workshops.Where(m => m.IndSiteEnterpriseId == request.IndSiteEnterpriseId);
            }

            return await workshops.ToListAsync();
        }

        // GET: api/Workshops/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Workshop>> GetWorkshop(int id)
        {
            var workshop = await _context.Workshop
                .Include(e => e.IndSiteEnterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workshop == null)
            {
                return NotFound();
            }

            return workshop;
        }

        // PUT: api/Workshops/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutWorkshop(int id, Workshop workshop)
        {
            if (id != workshop.Id)
            {
                return BadRequest();
            }

            _context.Entry(workshop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkshopExists(id))
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

        // POST: api/Workshops
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Workshop>> PostWorkshop(Workshop workshop)
        {
            _context.Workshop.Add(workshop);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkshop", new { id = workshop.Id }, workshop);
        }

        // DELETE: api/Workshops/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Workshop>> DeleteWorkshop(int id)
        {
            var workshop = await _context.Workshop.FindAsync(id);
            if (workshop == null)
            {
                return NotFound();
            }

            _context.Workshop.Remove(workshop);
            await _context.SaveChangesAsync();

            return workshop;
        }

        private bool WorkshopExists(int id)
        {
            return _context.Workshop.Any(e => e.Id == id);
        }
    }
}

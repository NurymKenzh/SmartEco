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
                .Include(w => w.Workshop)
                .Where(m => true);

            if (request?.WorkshopId != null)
            {
                areas = areas.Where(m => m.WorkshopId == request.WorkshopId);
            }

            return await areas.ToListAsync();
        }

        // GET: api/Areas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            var area = await _context.Area
                .Include(e => e.Workshop)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (area == null)
            {
                return NotFound();
            }

            return area;
        }

        // PUT: api/Areas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutArea(int id, Area area)
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

            return NoContent();
        }

        // POST: api/Areas
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Area>> PostArea(Area area)
        {
            _context.Area.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, area);
        }

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Area>> DeleteArea(int id)
        {
            var area = await _context.Area.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Area.Remove(area);
            await _context.SaveChangesAsync();

            return area;
        }

        private bool AreaExists(int id)
        {
            return _context.Area.Any(e => e.Id == id);
        }
    }
}

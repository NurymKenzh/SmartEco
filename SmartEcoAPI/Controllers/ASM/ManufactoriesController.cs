using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "admin,moderator,ASM")]
    public class ManufactoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ManufactoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Manufactories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manufactory>>> GetManufactory(string SortOrder,
            string Name,
            int? EnterpriseId,
            int? PageSize,
            int? PageNumber)
        {
            var manufactories = _context.Manufactory
                .Include(m => m.Enterprise)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                manufactories = manufactories.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (EnterpriseId != null)
            {
                manufactories = manufactories.Where(m => m.EnterpriseId == EnterpriseId);
            }

            switch (SortOrder)
            {
                case "Name":
                    manufactories = manufactories.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    manufactories = manufactories.OrderByDescending(k => k.Name);
                    break;
                case "Enterprise":
                    manufactories = manufactories.OrderBy(k => k.Enterprise);
                    break;
                case "EnterpriseDesc":
                    manufactories = manufactories.OrderByDescending(k => k.Enterprise);
                    break;
                default:
                    manufactories = manufactories.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                manufactories = manufactories.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await manufactories.ToListAsync();
        }

        // GET: api/Manufactories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manufactory>> GetManufactory(int id)
        {
            var manufactory = await _context.Manufactory
                .Include(m => m.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manufactory == null)
            {
                return NotFound();
            }

            return manufactory;
        }

        // PUT: api/Manufactories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManufactory(int id, Manufactory manufactory)
        {
            if (id != manufactory.Id)
            {
                return BadRequest();
            }

            _context.Entry(manufactory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManufactoryExists(id))
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

        // POST: api/Manufactories
        [HttpPost]
        public async Task<ActionResult<Manufactory>> PostManufactory(Manufactory manufactory)
        {
            _context.Manufactory.Add(manufactory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManufactory", new { id = manufactory.Id }, manufactory);
        }

        // DELETE: api/Manufactories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Manufactory>> DeleteManufactory(int id)
        {
            var manufactory = await _context.Manufactory
                .Include(m => m.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manufactory == null)
            {
                return NotFound();
            }

            _context.Manufactory.Remove(manufactory);
            await _context.SaveChangesAsync();

            return manufactory;
        }

        private bool ManufactoryExists(int id)
        {
            return _context.Manufactory.Any(e => e.Id == id);
        }

        // GET: api/Manufactories/Count
        [HttpGet("count")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Manufactory>>> GetManufactoryCount(string Name,
            int? EnterpriseId)
        {
            var manufactories = _context.Manufactory
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                manufactories = manufactories.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (EnterpriseId != null)
            {
                manufactories = manufactories.Where(m => m.EnterpriseId == EnterpriseId);
            }

            int count = await manufactories.CountAsync();

            return Ok(count);
        }
    }
}

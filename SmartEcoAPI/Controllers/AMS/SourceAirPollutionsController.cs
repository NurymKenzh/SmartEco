using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.AMS
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "admin,moderator,AMS")]
    public class SourceAirPollutionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SourceAirPollutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SourceAirPollutions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceAirPollution>>> GetSourceAirPollution(string SortOrder,
            string Name,
            int? ManufactoryId,
            int? PageSize,
            int? PageNumber)
        {
            var sourceAirPollutions = _context.SourceAirPollution
                .Include(m => m.Manufactory)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                sourceAirPollutions = sourceAirPollutions.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (ManufactoryId != null)
            {
                sourceAirPollutions = sourceAirPollutions.Where(m => m.ManufactoryId == ManufactoryId);
            }

            switch (SortOrder)
            {
                case "Name":
                    sourceAirPollutions = sourceAirPollutions.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    sourceAirPollutions = sourceAirPollutions.OrderByDescending(k => k.Name);
                    break;
                case "Manufactory":
                    sourceAirPollutions = sourceAirPollutions.OrderBy(k => k.Manufactory);
                    break;
                case "ManufactoryDesc":
                    sourceAirPollutions = sourceAirPollutions.OrderByDescending(k => k.Manufactory);
                    break;
                default:
                    sourceAirPollutions = sourceAirPollutions.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                sourceAirPollutions = sourceAirPollutions.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await sourceAirPollutions.ToListAsync();
        }

        // GET: api/SourceAirPollutions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SourceAirPollution>> GetSourceAirPollution(int id)
        {
            var sourceAirPollution = await _context.SourceAirPollution
                .Include(m => m.Manufactory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sourceAirPollution == null)
            {
                return NotFound();
            }

            return sourceAirPollution;
        }

        // PUT: api/SourceAirPollutions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSourceAirPollution(int id, SourceAirPollution sourceAirPollution)
        {
            if (id != sourceAirPollution.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceAirPollution).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceAirPollutionExists(id))
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

        // POST: api/SourceAirPollutions
        [HttpPost]
        public async Task<ActionResult<SourceAirPollution>> PostSourceAirPollution(SourceAirPollution sourceAirPollution)
        {
            _context.SourceAirPollution.Add(sourceAirPollution);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceAirPollution", new { id = sourceAirPollution.Id }, sourceAirPollution);
        }

        // DELETE: api/SourceAirPollutions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SourceAirPollution>> DeleteSourceAirPollution(int id)
        {
            var sourceAirPollution = await _context.SourceAirPollution
                .Include(m => m.Manufactory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sourceAirPollution == null)
            {
                return NotFound();
            }

            _context.SourceAirPollution.Remove(sourceAirPollution);
            await _context.SaveChangesAsync();

            return sourceAirPollution;
        }

        private bool SourceAirPollutionExists(int id)
        {
            return _context.SourceAirPollution.Any(e => e.Id == id);
        }

        // GET: api/SourceAirPollutions/Count
        [HttpGet("count")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<SourceAirPollution>>> GetSourceAirPollutionCount(string Name,
            int? ManufactoryId)
        {
            var sourceAirPollutions = _context.SourceAirPollution
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                sourceAirPollutions = sourceAirPollutions.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (ManufactoryId != null)
            {
                sourceAirPollutions = sourceAirPollutions.Where(m => m.ManufactoryId == ManufactoryId);
            }

            int count = await sourceAirPollutions.CountAsync();

            return Ok(count);
        }
    }
}

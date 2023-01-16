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
    public class SourceEmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SourceEmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SourceEmissions
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<SourceEmission>>> GetSourceEmission(string SortOrder,
            string Name,
            int? SourceAirPollutionId,
            int? PageSize,
            int? PageNumber)
        {
            var sourceEmissions = _context.SourceEmission
                .Include(m => m.SourceAirPollution)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                sourceEmissions = sourceEmissions.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (SourceAirPollutionId != null)
            {
                sourceEmissions = sourceEmissions.Where(m => m.SourceAirPollutionId == SourceAirPollutionId);
            }

            switch (SortOrder)
            {
                case "Name":
                    sourceEmissions = sourceEmissions.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    sourceEmissions = sourceEmissions.OrderByDescending(k => k.Name);
                    break;
                case "SourceAirPollution":
                    sourceEmissions = sourceEmissions.OrderBy(k => k.SourceAirPollution);
                    break;
                case "SourceAirPollutionDesc":
                    sourceEmissions = sourceEmissions.OrderByDescending(k => k.SourceAirPollution);
                    break;
                default:
                    sourceEmissions = sourceEmissions.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                sourceEmissions = sourceEmissions.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await sourceEmissions.ToListAsync();
        }

        // GET: api/SourceEmissions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<SourceEmission>> GetSourceEmission(int id)
        {
            var sourceEmission = await _context.SourceEmission
                .Include(m => m.SourceAirPollution)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sourceEmission == null)
            {
                return NotFound();
            }

            return sourceEmission;
        }

        // PUT: api/SourceEmissions/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutSourceEmission(int id, SourceEmission sourceEmission)
        {
            if (id != sourceEmission.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceEmission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceEmissionExists(id))
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

        // POST: api/SourceEmissions
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<SourceEmission>> PostSourceEmission(SourceEmission sourceEmission)
        {
            _context.SourceEmission.Add(sourceEmission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceEmission", new { id = sourceEmission.Id }, sourceEmission);
        }

        // DELETE: api/SourceEmissions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<SourceEmission>> DeleteSourceEmission(int id)
        {
            var sourceEmission = await _context.SourceEmission
                .Include(m => m.SourceAirPollution)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sourceEmission == null)
            {
                return NotFound();
            }

            _context.SourceEmission.Remove(sourceEmission);
            await _context.SaveChangesAsync();

            return sourceEmission;
        }

        private bool SourceEmissionExists(int id)
        {
            return _context.SourceEmission.Any(e => e.Id == id);
        }

        // GET: api/SourceEmissions/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,AMS")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<SourceEmission>>> GetSourceEmissionCount(string Name,
            int? SourceAirPollutionId)
        {
            var sourceEmissions = _context.SourceEmission
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                sourceEmissions = sourceEmissions.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (SourceAirPollutionId != null)
            {
                sourceEmissions = sourceEmissions.Where(m => m.SourceAirPollutionId == SourceAirPollutionId);
            }

            int count = await sourceEmissions.CountAsync();

            return Ok(count);
        }
    }
}

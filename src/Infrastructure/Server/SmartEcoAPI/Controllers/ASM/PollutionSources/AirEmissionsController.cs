using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM.PollutionSources
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,moderator,ASM")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AirEmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirEmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AirEmissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirEmission>>> GetAirEmissions([FromBody] AirEmissionsRequest request)
        {
            var airEmissions = _context.AirEmission
                .Include(mode => mode.Pollutant)
                .Where(m => true);

            if (request?.OperationModeId != null)
            {
                airEmissions = airEmissions.Where(e => e.OperationModeId == request.OperationModeId);
            }

            return await airEmissions
                .OrderBy(mode => mode.Id)
                .ToListAsync();
        }

        // GET: api/AirEmissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AirEmission>> GetAirEmission(int id)
        {
            var airEmission = await _context.AirEmission.FindAsync(id);

            if (airEmission == null)
            {
                return NotFound();
            }

            return airEmission;
        }

        // PUT: api/AirEmissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirEmission(int id, AirEmission airEmission)
        {
            if (id != airEmission.Id)
            {
                return BadRequest();
            }

            _context.Entry(airEmission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirEmissionExists(id))
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

        // POST: api/AirEmissions
        [HttpPost]
        public async Task<ActionResult<AirEmission>> PostAirEmission(AirEmission airEmission)
        {
            _context.AirEmission.Add(airEmission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirEmission", new { id = airEmission.Id }, airEmission);
        }

        // DELETE: api/AirEmissions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AirEmission>> DeleteAirEmission(int id)
        {
            var airEmission = await _context.AirEmission.FindAsync(id);
            if (airEmission == null)
            {
                return NotFound();
            }

            _context.AirEmission.Remove(airEmission);
            await _context.SaveChangesAsync();

            return airEmission;
        }

        private bool AirEmissionExists(int id)
        {
            return _context.AirEmission.Any(e => e.Id == id);
        }
    }
}
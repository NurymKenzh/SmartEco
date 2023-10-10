using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM.PollutionSources
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,moderator,ASM")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OperationModesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationModesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OperationModes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationMode>>> GetOperationModes(OperationModesRequest request)
        {
            var operationModes = _context.OperationMode
                .Include(mode => mode.GasAirMixture)
                .Where(m => true);

            if (request?.SourceId != null)
            {
                operationModes = operationModes.Where(mode => mode.SourceId == request.SourceId);
            }

            return await operationModes
                .OrderBy(mode => mode.Id)
                .ToListAsync();
        }

        // GET: api/OperationModes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationMode>> GetOperationMode(int id)
        {
            var operationMode = await _context.OperationMode.FindAsync(id);
            if (operationMode == null)
            {
                return NotFound();
            }

            return operationMode;
        }

        // PUT: api/OperationModes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperationMode(int id, OperationMode operationMode)
        {
            if (id != operationMode.Id)
            {
                return BadRequest();
            }

            _context.Entry(operationMode).State = EntityState.Modified;
            _context.Entry(operationMode.GasAirMixture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationModeExists(id))
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

        // POST: api/OperationModes
        [HttpPost]
        public async Task<ActionResult<OperationMode>> PostOperationMode(OperationMode operationMode)
        {
            if (operationMode.GasAirMixture is null)
                return BadRequest();

            _context.OperationMode.Add(operationMode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOperationMode", new { id = operationMode.Id }, operationMode);
        }

        // DELETE: api/OperationModes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationMode>> DeleteOperationMode(int id)
        {
            var operationMode = await _context.OperationMode.FindAsync(id);
            if (operationMode == null)
            {
                return NotFound();
            }

            _context.OperationMode.Remove(operationMode);
            await _context.SaveChangesAsync();

            return operationMode;
        }

        private bool OperationModeExists(int id)
        {
            return _context.OperationMode.Any(e => e.Id == id);
        }
    }
}

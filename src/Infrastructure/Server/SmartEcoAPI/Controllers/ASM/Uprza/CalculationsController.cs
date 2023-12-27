using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using SmartEcoAPI.Models.ASM.Uprza;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalculationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Calculations
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationsResponse>> GetCalculations([FromBody] CalculationsRequest request)
        {
            var calculations = _context.Calculation
                .Include(c => c.Type)
                .Include(c => c.Status)
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                calculations = calculations.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.KatoComplex))
            {
                calculations = calculations.Where(m => $"{m.KatoCode} {m.KatoName}".ToLower().Contains(request.KatoComplex.ToLower()));
            }
            if (request.CalculationTypeId != null)
            {
                calculations = calculations.Where(m => m.TypeId == request.CalculationTypeId);
            }
            if (request.CalculationStatusId != null)
            {
                calculations = calculations.Where(m => m.StatusId == request.CalculationStatusId);
            }

            switch (request.SortOrder)
            {
                case "Name":
                    calculations = calculations.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    calculations = calculations.OrderByDescending(m => m.Name);
                    break;
                default:
                    calculations = calculations.OrderBy(m => m.Id);
                    break;
            }

            var count = await calculations.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                calculations = calculations.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new CalculationsResponse(await calculations.ToListAsync(), count);

            return response;
        }

        // GET: api/Calculations/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Calculation>> GetCalculation(int id)
        {
            var calculation = await _context.Calculation
                .Include(c => c.Type)
                .Include(c => c.Status)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calculation == null)
            {
                return NotFound();
            }

            return calculation;
        }

        // PUT: api/Calculations/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutCalculation(int id, Calculation calculation)
        {
            if (id != calculation.Id)
            {
                return BadRequest();
            }

            _context.Entry(calculation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalculationExists(id))
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

        // POST: api/Calculations
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Calculation>> PostCalculation(Calculation calculation)
        {
            _context.Calculation.Add(calculation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCalculation", new { id = calculation.Id }, calculation);
        }

        // DELETE: api/Calculations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Calculation>> DeleteCalculation(int id)
        {
            var calculation = await _context.Calculation.FindAsync(id);
            if (calculation == null)
            {
                return NotFound();
            }

            _context.Calculation.Remove(calculation);
            await _context.SaveChangesAsync();

            return calculation;
        }

        private bool CalculationExists(int id)
        {
            return _context.Calculation.Any(e => e.Id == id);
        }
    }
}
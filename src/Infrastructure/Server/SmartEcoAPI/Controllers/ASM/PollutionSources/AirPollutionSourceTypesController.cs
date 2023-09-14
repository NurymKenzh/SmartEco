using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM.PollutionSources
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AirPollutionSourceTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirPollutionSourceTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AirPollutionSourceTypes
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<AirPollutionSourceTypesResponse>> GetAirPollutionSourceTypes([FromBody] AirPollutionSourceTypesRequest request)
        {
            var airPollutionSourceTypes = _context.AirPollutionSourceType
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                airPollutionSourceTypes = airPollutionSourceTypes.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    airPollutionSourceTypes = airPollutionSourceTypes.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    airPollutionSourceTypes = airPollutionSourceTypes.OrderByDescending(m => m.Name);
                    break;
                default:
                    airPollutionSourceTypes = airPollutionSourceTypes.OrderBy(m => m.Id);
                    break;
            }

            var count = await airPollutionSourceTypes.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                airPollutionSourceTypes = airPollutionSourceTypes.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new AirPollutionSourceTypesResponse(await airPollutionSourceTypes.ToListAsync(), count);

            return response;
        }

        // GET: api/AirPollutionSourceTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<AirPollutionSourceType>> GetAirPollutionSourceType(int id)
        {
            var airPollutionSourceType = await _context.AirPollutionSourceType.FindAsync(id);

            if (airPollutionSourceType == null)
            {
                return NotFound();
            }

            return airPollutionSourceType;
        }

        // PUT: api/AirPollutionSourceTypes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutAirPollutionSourceType(int id, AirPollutionSourceType airPollutionSourceType)
        {
            if (id != airPollutionSourceType.Id)
            {
                return BadRequest();
            }

            _context.Entry(airPollutionSourceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirPollutionSourceTypeExists(id))
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

        // POST: api/AirPollutionSourceTypes
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<AirPollutionSourceType>> PostAirPollutionSourceType(AirPollutionSourceType airPollutionSourceType)
        {
            _context.AirPollutionSourceType.Add(airPollutionSourceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirPollutionSourceType", new { id = airPollutionSourceType.Id }, airPollutionSourceType);
        }

        // DELETE: api/AirPollutionSourceTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<AirPollutionSourceType>> DeleteAirPollutionSourceType(int id)
        {
            var airPollutionSourceType = await _context.AirPollutionSourceType.FindAsync(id);
            if (airPollutionSourceType == null)
            {
                return NotFound();
            }

            _context.AirPollutionSourceType.Remove(airPollutionSourceType);
            await _context.SaveChangesAsync();

            return airPollutionSourceType;
        }

        private bool AirPollutionSourceTypeExists(int id)
        {
            return _context.AirPollutionSourceType.Any(e => e.Id == id);
        }
    }
}
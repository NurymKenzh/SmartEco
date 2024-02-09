using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Uprza;
using SmartEcoAPI.Services;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ResultEmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultEmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ResultEmissions
        [HttpGet("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<List<AirPollutant>>> GetResultEmissionPollutants(int calculationId)
            => await _context.ResultEmission
                .Include(r => r.Calculation.Status)
                .Include(r => r.AirPollutant)
                .Where(r => r.CalculationId == calculationId)
                .Select(r => r.AirPollutant)
                .ToListAsync();

        // GET: api/ResultEmissions
        [HttpGet("{calculationId}/{pollutantCode}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<ResultEmission>> GetResultEmission(int calculationId, int pollutantCode)
            => await _context.ResultEmission
                .Include(r => r.Calculation.Status)
                .Include(r => r.AirPollutant)
                .Where(r => r.CalculationId == calculationId && r.AirPollutant.Code == pollutantCode)
                .FirstOrDefaultAsync();

        // POST: api/ResultEmissions
        [HttpPost("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> CreateResultEmissions(int calculationId, List<ResultEmission> resultEmissions)
        {
            var calculationIds = resultEmissions.Select(r => r.CalculationId).ToList();
            if(!calculationIds.All(c => c.Equals(calculationId)))
                return BadRequest();

            await _context.ResultEmission.AddRangeAsync(resultEmissions);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using SmartEcoAPI.Models.ASM.Uprza;
using SmartEcoAPI.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CalculationSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public CalculationSettingsController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationSettingsResponse>> GetCalculationSettings([FromBody] CalculationSettingsRequest request)
        {
            var calcPoint = await _context.CalculationPoint
                .Where(p => p.CalculationId == request.CalculationId)
                .ToListAsync();

            var calcRectangle = await _context.CalculationRectangle
                .Where(p => p.CalculationId == request.CalculationId)
                .ToListAsync();

            CalculationSettingMvc calcSettingMvc = null;
            var calcSetting = await _context.CalculationSetting
                .FirstOrDefaultAsync(c => c.CalculationId == request.CalculationId);
            if (calcSetting != null)
            {
                calcSettingMvc = JsonConvert.DeserializeObject<CalculationSettingMvc>(calcSetting.SettingsJson);
            }

            var stateCalc = await _context.StateCalculation
                .Include(s => s.Calculation.Status)
                .Where(s => s.CalculationId == request.CalculationId)
                .FirstOrDefaultAsync();

            return new CalculationSettingsResponse(calcPoint, calcRectangle, calcSettingMvc, stateCalc);
        }

        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationPoint>> SetCalculationSetting(CalculationSettingMvc calcSettingMvc)
        {
            var calcSetting = await _context.CalculationSetting
                .FirstOrDefaultAsync(s => s.CalculationId == calcSettingMvc.CalculationId);
            var settingJson = JsonConvert.SerializeObject(calcSettingMvc);

            if (calcSetting != null)
            {
                calcSetting.SettingsJson = settingJson;
                _context.Entry(calcSetting).State = EntityState.Modified;
            }
            else
            {
                _context.CalculationSetting.Add(new CalculationSetting()
                {
                    CalculationId = calcSettingMvc.CalculationId,
                    SettingsJson = settingJson
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region Points
        [HttpGet("Points")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationSettingsResponse>> GetCalculationPoints([FromBody] CalculationSettingsRequest request)
        {
            var calcPoint = await _context.CalculationPoint
                .Where(p => p.CalculationId == request.CalculationId)
                .ToListAsync();

            return new CalculationSettingsResponse(calcPoint);
        }

        [HttpPost("Point")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> CreateCalculationPoint(CalculationPoint calcPoint)
        {
            _context.CalculationPoint.Add(calcPoint);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calcPoint.CalculationId, CalculationStatuses.Configuration);

            return NoContent();
        }

        [HttpDelete("Point/{calculationId}/{number}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> DeleteCalculationPoint(
            int calculationId,
            int number)
        {
            var calcPoint = await _context.CalculationPoint.FindAsync(calculationId, number);
            if (calcPoint == null)
                return NotFound();

            _context.CalculationPoint.Remove(calcPoint);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calculationId, CalculationStatuses.Configuration);

            return NoContent();
        }
        #endregion Points

        #region Rectangle
        [HttpGet("Rectangles")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<CalculationSettingsResponse>> GetCalculationRectangles([FromBody] CalculationSettingsRequest request)
        {
            var calcRectangle = await _context.CalculationRectangle
                .Where(p => p.CalculationId == request.CalculationId)
                .ToListAsync();

            return new CalculationSettingsResponse(calcRectangle);
        }

        [HttpPost("Rectangle")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> CreateCalculationRectangle(CalculationRectangle calcRectangle)
        {
            _context.CalculationRectangle.Add(calcRectangle);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calcRectangle.CalculationId, CalculationStatuses.Configuration);

            return NoContent();
        }

        [HttpDelete("Rectangle/{calculationId}/{number}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> DeleteCalculationRectangle(
            int calculationId,
            int number)
        {
            var calcRectangle = await _context.CalculationRectangle.FindAsync(calculationId, number);
            if (calcRectangle == null)
                return NotFound();

            _context.CalculationRectangle.Remove(calcRectangle);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calculationId, CalculationStatuses.Configuration);

            return NoContent();
        }
        #endregion Rectangle
    }
}

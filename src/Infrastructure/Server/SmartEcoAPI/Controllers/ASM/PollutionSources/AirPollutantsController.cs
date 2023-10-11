using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
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
    public class AirPollutantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirPollutantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AirPollutants
        [HttpGet]
        public async Task<ActionResult<AirPollutantsResponse>> GetAirPollutants([FromBody] AirPollutantsRequest request)
        {
            var airPollutants = _context.AirPollutant
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                airPollutants = airPollutants.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    airPollutants = airPollutants.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    airPollutants = airPollutants.OrderByDescending(m => m.Name);
                    break;
                default:
                    airPollutants = airPollutants.OrderBy(m => m.Id);
                    break;
            }

            var count = await airPollutants.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                airPollutants = airPollutants.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new AirPollutantsResponse(await airPollutants.ToListAsync(), count);

            return response;
        }

        // GET: api/AirPollutants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AirPollutant>> GetAirPollutant(int id)
        {
            var airPollutant = await _context.AirPollutant.FindAsync(id);

            if (airPollutant == null)
            {
                return NotFound();
            }

            return airPollutant;
        }

        // PUT: api/AirPollutants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirPollutant(int id, AirPollutant airPollutant)
        {
            if (id != airPollutant.Id)
            {
                return BadRequest();
            }

            _context.Entry(airPollutant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirPollutantExists(id))
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

        // POST: api/AirPollutants
        [HttpPost]
        public async Task<ActionResult<AirPollutant>> PostAirPollutant(AirPollutant airPollutant)
        {
            _context.AirPollutant.Add(airPollutant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirPollutant", new { id = airPollutant.Id }, airPollutant);
        }

        // DELETE: api/AirPollutants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AirPollutant>> DeleteAirPollutant(int id)
        {
            var airPollutant = await _context.AirPollutant.FindAsync(id);
            if (airPollutant == null)
            {
                return NotFound();
            }

            _context.AirPollutant.Remove(airPollutant);
            await _context.SaveChangesAsync();

            return airPollutant;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public async Task<IActionResult> ImportPollutants(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("File is empty");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File extension is not supported");

            var isReadSuccess = await ReadExcel(file, cancellationToken);
            if (isReadSuccess)
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private bool AirPollutantExists(int id)
        {
            return _context.AirPollutant.Any(e => e.Id == id);
        }

        private async Task<bool> ReadExcel(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var airPollutants = new List<AirPollutant>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream, cancellationToken);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var airPollutant = new AirPollutant();
                            airPollutant.Code = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString());
                            airPollutant.Name = worksheet.Cells[row, 3].Value?.ToString();
                            airPollutant.Formula = worksheet.Cells[row, 4].Value?.ToString();
                            airPollutant.MpcMaxSingle = worksheet.Cells[row, 5].Value is null
                                ? null
                                : ConvertDecimal(worksheet.Cells[row, 5].Value.ToString());
                            airPollutant.MpcAvgDaily = worksheet.Cells[row, 6].Value is null
                                ? null
                                : ConvertDecimal(worksheet.Cells[row, 6].Value.ToString());
                            airPollutant.Asel = worksheet.Cells[row, 7].Value is null
                                ? null
                                : ConvertDecimal(worksheet.Cells[row, 7].Value.ToString());

                            airPollutant.MeasuredUnit = worksheet.Cells[row, 8].Value?.ToString();
                            airPollutant.HazardLevelId = worksheet.Cells[row, 9].Value is null
                                    ? null
                                    : (int?)Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString());

                            airPollutant.Cas = worksheet.Cells[row, 10].Value?.ToString();
                            airPollutant.MpcMaxSingle2 = worksheet.Cells[row, 12].Value is null
                                ? null
                                : ConvertDecimal(worksheet.Cells[row, 12].Value.ToString());
                            airPollutant.SummationGroup = worksheet.Cells[row, 13].Value?.ToString();
                            airPollutant.AggregationState = worksheet.Cells[row, 15].Value?.ToString();
                            airPollutants.Add(airPollutant);
                        }
                    }
                }

                await _context.AirPollutant.AddRangeAsync(airPollutants);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        private decimal? ConvertDecimal(string value)
            => decimal.Parse(value, NumberStyles.Float);
    }
}
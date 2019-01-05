using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasuredParametersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredParameters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameter(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? EcomonCode,
            int? PageSize,
            int? PageNumber)
        {
            var measuredParameters = _context.MeasuredParameter
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameters = measuredParameters.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameters = measuredParameters.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameters = measuredParameters.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (EcomonCode!=null)
            {
                measuredParameters = measuredParameters.Where(m => m.EcomonCode == EcomonCode);
            }

            switch (SortOrder)
            {
                case "NameKK":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameEN);
                    break;
                case "EcomonCode":
                    measuredParameters = measuredParameters.OrderBy(m => m.EcomonCode);
                    break;
                case "EcomonCodeDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.EcomonCode);
                    break;
                default:
                    measuredParameters = measuredParameters.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                measuredParameters = measuredParameters.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await measuredParameters.ToListAsync();
        }

        // GET: api/MeasuredParameters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeasuredParameter>> GetMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter.FindAsync(id);

            if (measuredParameter == null)
            {
                return NotFound();
            }

            return measuredParameter;
        }

        // PUT: api/MeasuredParameters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasuredParameter(int id, MeasuredParameter measuredParameter)
        {
            if (id != measuredParameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredParameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredParameterExists(id))
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

        // POST: api/MeasuredParameters
        [HttpPost]
        public async Task<ActionResult<MeasuredParameter>> PostMeasuredParameter(MeasuredParameter measuredParameter)
        {
            _context.MeasuredParameter.Add(measuredParameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredParameter", new { id = measuredParameter.Id }, measuredParameter);
        }

        // DELETE: api/MeasuredParameters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MeasuredParameter>> DeleteMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter.FindAsync(id);
            if (measuredParameter == null)
            {
                return NotFound();
            }

            _context.MeasuredParameter.Remove(measuredParameter);
            await _context.SaveChangesAsync();

            return measuredParameter;
        }

        private bool MeasuredParameterExists(int id)
        {
            return _context.MeasuredParameter.Any(e => e.Id == id);
        }

        // GET: api/MeasuredParameters/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameterCount(string NameKK,
            string NameRU,
            string NameEN,
            int? EcomonCode)
        {
            var measuredParameters = _context.MeasuredParameter
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameters = measuredParameters.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameters = measuredParameters.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameters = measuredParameters.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (EcomonCode != null)
            {
                measuredParameters = measuredParameters.Where(m => m.EcomonCode == EcomonCode);
            }

            int count = await measuredParameters.CountAsync();

            return Ok(count);
        }
    }
}

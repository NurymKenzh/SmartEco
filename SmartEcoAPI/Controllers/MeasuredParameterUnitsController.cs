using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MeasuredParameterUnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredParameterUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredParameterUnits
        [HttpGet]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<IEnumerable<MeasuredParameterUnit>>> GetMeasuredParameterUnit(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var measuredParameterUnits = _context.MeasuredParameterUnit
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    measuredParameterUnits = measuredParameterUnits.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    measuredParameterUnits = measuredParameterUnits.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    measuredParameterUnits = measuredParameterUnits.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    measuredParameterUnits = measuredParameterUnits.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    measuredParameterUnits = measuredParameterUnits.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    measuredParameterUnits = measuredParameterUnits.OrderByDescending(m => m.NameEN);
                    break;
                default:
                    measuredParameterUnits = measuredParameterUnits.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                measuredParameterUnits = measuredParameterUnits.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await measuredParameterUnits.ToListAsync();
        }

        // GET: api/MeasuredParameterUnits/5
        [HttpGet("{id}")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<MeasuredParameterUnit>> GetMeasuredParameterUnit(int id)
        {
            var measuredParameterUnit = await _context.MeasuredParameterUnit.FindAsync(id);

            if (measuredParameterUnit == null)
            {
                return NotFound();
            }

            return measuredParameterUnit;
        }

        // PUT: api/MeasuredParameterUnits/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutMeasuredParameterUnit(int id, MeasuredParameterUnit measuredParameterUnit)
        {
            if (id != measuredParameterUnit.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredParameterUnit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredParameterUnitExists(id))
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

        // POST: api/MeasuredParameterUnits
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MeasuredParameterUnit>> PostMeasuredParameterUnit(MeasuredParameterUnit measuredParameterUnit)
        {
            _context.MeasuredParameterUnit.Add(measuredParameterUnit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredParameterUnit", new { id = measuredParameterUnit.Id }, measuredParameterUnit);
        }

        // DELETE: api/MeasuredParameterUnits/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MeasuredParameterUnit>> DeleteMeasuredParameterUnit(int id)
        {
            var measuredParameterUnit = await _context.MeasuredParameterUnit.FindAsync(id);
            if (measuredParameterUnit == null)
            {
                return NotFound();
            }

            _context.MeasuredParameterUnit.Remove(measuredParameterUnit);
            await _context.SaveChangesAsync();

            return measuredParameterUnit;
        }

        private bool MeasuredParameterUnitExists(int id)
        {
            return _context.MeasuredParameterUnit.Any(e => e.Id == id);
        }

        // GET: api/MeasuredParameterUnits/Count
        [HttpGet("count")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameterUnitCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var measuredParameterUnits = _context.MeasuredParameterUnit
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameterUnits = measuredParameterUnits.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await measuredParameterUnits.CountAsync();

            return Ok(count);
        }
    }
}

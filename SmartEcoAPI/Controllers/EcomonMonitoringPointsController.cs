using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcomonMonitoringPointsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EcomonMonitoringPointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EcomonMonitoringPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EcomonMonitoringPoint>>> GetEcomonMonitoringPoint(string SortOrder,
            int? Number,
            int? PageSize,
            int? PageNumber)
        {
            var ecomonMonitoringPoints = _context.EcomonMonitoringPoint
                .Where(e => true);

            if (Number != null)
            {
                ecomonMonitoringPoints = ecomonMonitoringPoints.Where(e => e.Number == Number);
            }

            switch (SortOrder)
            {
                case "Number":
                    ecomonMonitoringPoints = ecomonMonitoringPoints.OrderBy(e => e.Number);
                    break;
                case "NumberDesc":
                    ecomonMonitoringPoints = ecomonMonitoringPoints.OrderByDescending(e => e.Number);
                    break;
                default:
                    ecomonMonitoringPoints = ecomonMonitoringPoints.OrderBy(e => e.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                ecomonMonitoringPoints = ecomonMonitoringPoints.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await ecomonMonitoringPoints.ToListAsync();
        }

        // GET: api/EcomonMonitoringPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EcomonMonitoringPoint>> GetEcomonMonitoringPoint(int id)
        {
            var ecomonMonitoringPoint = await _context.EcomonMonitoringPoint.FindAsync(id);

            if (ecomonMonitoringPoint == null)
            {
                return NotFound();
            }

            return ecomonMonitoringPoint;
        }

        // PUT: api/EcomonMonitoringPoints/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEcomonMonitoringPoint(int id, EcomonMonitoringPoint ecomonMonitoringPoint)
        {
            if (id != ecomonMonitoringPoint.Id)
            {
                return BadRequest();
            }

            _context.Entry(ecomonMonitoringPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcomonMonitoringPointExists(id))
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

        // POST: api/EcomonMonitoringPoints
        [HttpPost]
        public async Task<ActionResult<EcomonMonitoringPoint>> PostEcomonMonitoringPoint(EcomonMonitoringPoint ecomonMonitoringPoint)
        {
            _context.EcomonMonitoringPoint.Add(ecomonMonitoringPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEcomonMonitoringPoint", new { id = ecomonMonitoringPoint.Id }, ecomonMonitoringPoint);
        }

        // DELETE: api/EcomonMonitoringPoints/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EcomonMonitoringPoint>> DeleteEcomonMonitoringPoint(int id)
        {
            var ecomonMonitoringPoint = await _context.EcomonMonitoringPoint.FindAsync(id);
            if (ecomonMonitoringPoint == null)
            {
                return NotFound();
            }

            _context.EcomonMonitoringPoint.Remove(ecomonMonitoringPoint);
            await _context.SaveChangesAsync();

            return ecomonMonitoringPoint;
        }

        private bool EcomonMonitoringPointExists(int id)
        {
            return _context.EcomonMonitoringPoint.Any(e => e.Id == id);
        }

        // GET: api/EcomonMonitoringPoints/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<EcomonMonitoringPoint>>> GetMeasuredParameterCount(int? Number)
        {
            var ecomonMonitoringPoints = _context.EcomonMonitoringPoint
                .Where(e => true);

            if (Number != null)
            {
                ecomonMonitoringPoints = ecomonMonitoringPoints.Where(e => e.Number == Number);
            }

            int count = await ecomonMonitoringPoints.CountAsync();

            return Ok(count);
        }
    }
}

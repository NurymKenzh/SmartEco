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
    public class MeasuredDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetMeasuredData()
        {
            return await _context.MeasuredData.ToListAsync();
        }

        // GET: api/MeasuredDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeasuredData>> GetMeasuredData(int id)
        {
            var measuredData = await _context.MeasuredData.FindAsync(id);

            if (measuredData == null)
            {
                return NotFound();
            }

            return measuredData;
        }

        // PUT: api/MeasuredDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasuredData(int id, MeasuredData measuredData)
        {
            if (id != measuredData.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredDataExists(id))
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

        // POST: api/MeasuredDatas
        [HttpPost]
        public async Task<ActionResult<MeasuredData>> PostMeasuredData(MeasuredData measuredData)
        {
            _context.MeasuredData.Add(measuredData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredData", new { id = measuredData.Id }, measuredData);
        }

        // DELETE: api/MeasuredDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MeasuredData>> DeleteMeasuredData(int id)
        {
            var measuredData = await _context.MeasuredData.FindAsync(id);
            if (measuredData == null)
            {
                return NotFound();
            }

            _context.MeasuredData.Remove(measuredData);
            await _context.SaveChangesAsync();

            return measuredData;
        }

        private bool MeasuredDataExists(int id)
        {
            return _context.MeasuredData.Any(e => e.Id == id);
        }
    }
}

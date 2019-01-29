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
    public class PollutantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pollutants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pollutant>>> GetPollutant(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var pollutants = _context.Pollutant
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                pollutants = pollutants.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                pollutants = pollutants.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                pollutants = pollutants.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    pollutants = pollutants.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    pollutants = pollutants.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    pollutants = pollutants.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    pollutants = pollutants.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    pollutants = pollutants.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    pollutants = pollutants.OrderByDescending(m => m.NameEN);
                    break;
                default:
                    pollutants = pollutants.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                pollutants = pollutants.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await pollutants.ToListAsync();
        }

        // GET: api/Pollutants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pollutant>> GetPollutant(int id)
        {
            var pollutant = await _context.Pollutant.FindAsync(id);

            if (pollutant == null)
            {
                return NotFound();
            }

            return pollutant;
        }

        // PUT: api/Pollutants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPollutant(int id, Pollutant pollutant)
        {
            if (id != pollutant.Id)
            {
                return BadRequest();
            }

            _context.Entry(pollutant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollutantExists(id))
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

        // POST: api/Pollutants
        [HttpPost]
        public async Task<ActionResult<Pollutant>> PostPollutant(Pollutant pollutant)
        {
            _context.Pollutant.Add(pollutant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutant", new { id = pollutant.Id }, pollutant);
        }

        // DELETE: api/Pollutants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pollutant>> DeletePollutant(int id)
        {
            var pollutant = await _context.Pollutant.FindAsync(id);
            if (pollutant == null)
            {
                return NotFound();
            }

            _context.Pollutant.Remove(pollutant);
            await _context.SaveChangesAsync();

            return pollutant;
        }

        private bool PollutantExists(int id)
        {
            return _context.Pollutant.Any(e => e.Id == id);
        }

        // GET: api/Pollutants/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<Pollutant>>> GetPollutantCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var pollutants = _context.Pollutant
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                pollutants = pollutants.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                pollutants = pollutants.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                pollutants = pollutants.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await pollutants.CountAsync();

            return Ok(count);
        }
    }
}

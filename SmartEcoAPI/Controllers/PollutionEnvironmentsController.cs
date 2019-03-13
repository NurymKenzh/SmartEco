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
    public class PollutionEnvironmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutionEnvironmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PollutionEnvironments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PollutionEnvironment>>> GetPollutionEnvironment(string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var pollutionEnvironments = _context.PollutionEnvironment
                .Where(k => true);

            if (NameKK != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameKK == NameKK);
            }
            if (NameRU != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameRU == NameRU);
            }
            if (NameEN != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameEN == NameEN);
            }

            switch (NameKKSortOrder)
            {
                case "NameKK":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.NameKK);
                    break;
                case "NameKKDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(k => k.NameKK);
                    break;
                default:
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.Id);
                    break;
            }
            switch (NameRUSortOrder)
            {
                case "NameRU":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.NameRU);
                    break;
                case "NameRUDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(k => k.NameRU);
                    break;
                default:
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.Id);
                    break;
            }
            switch (NameENSortOrder)
            {
                case "NameEN":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.NameEN);
                    break;
                case "NameENDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(k => k.NameEN);
                    break;
                default:
                    pollutionEnvironments = pollutionEnvironments.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                pollutionEnvironments = pollutionEnvironments.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await pollutionEnvironments.ToListAsync();
        }

        // GET: api/PollutionEnvironments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PollutionEnvironment>> GetPollutionEnvironment(int id)
        {
            var pollutionEnvironment = await _context.PollutionEnvironment.FindAsync(id);

            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            return pollutionEnvironment;
        }

        // PUT: api/PollutionEnvironments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPollutionEnvironment(int id, PollutionEnvironment pollutionEnvironment)
        {
            if (id != pollutionEnvironment.Id)
            {
                return BadRequest();
            }

            _context.Entry(pollutionEnvironment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollutionEnvironmentExists(id))
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

        // POST: api/PollutionEnvironments
        [HttpPost]
        public async Task<ActionResult<PollutionEnvironment>> PostPollutionEnvironment(PollutionEnvironment pollutionEnvironment)
        {
            _context.PollutionEnvironment.Add(pollutionEnvironment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutionEnvironment", new { id = pollutionEnvironment.Id }, pollutionEnvironment);
        }

        // DELETE: api/PollutionEnvironments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PollutionEnvironment>> DeletePollutionEnvironment(int id)
        {
            var pollutionEnvironment = await _context.PollutionEnvironment.FindAsync(id);
            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            _context.PollutionEnvironment.Remove(pollutionEnvironment);
            await _context.SaveChangesAsync();

            return pollutionEnvironment;
        }

        private bool PollutionEnvironmentExists(int id)
        {
            return _context.PollutionEnvironment.Any(e => e.Id == id);
        }

        // GET: api/PollutionEnvironments/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<PollutionEnvironment>>> GetPollutionEnvironmentCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var pollutionEnvironments = _context.PollutionEnvironment
                .Where(k => true);

            if (NameKK != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameKK == NameKK);
            }
            if (NameRU != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameRU == NameRU);
            }
            if (NameEN != null)
            {
                pollutionEnvironments = pollutionEnvironments.Where(k => k.NameEN == NameEN);
            }

            int count = await pollutionEnvironments.CountAsync();

            return Ok(count);
        }
    }
}
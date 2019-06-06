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
    public class PollutionSourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutionSourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PollutionSources
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<PollutionSource>>> GetPollutionSource(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var pollutionSources = _context.PollutionSource
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                pollutionSources = pollutionSources.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    pollutionSources = pollutionSources.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    pollutionSources = pollutionSources.OrderByDescending(m => m.Name);
                    break;
                default:
                    pollutionSources = pollutionSources.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                pollutionSources = pollutionSources.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await pollutionSources.ToListAsync();
        }

        // GET: api/PollutionSources/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PollutionSource>> GetPollutionSource(int id)
        {
            var pollutionSource = await _context.PollutionSource.FindAsync(id);

            if (pollutionSource == null)
            {
                return NotFound();
            }

            return pollutionSource;
        }

        // PUT: api/PollutionSources/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutPollutionSource(int id, PollutionSource pollutionSource)
        {
            if (id != pollutionSource.Id)
            {
                return BadRequest();
            }

            _context.Entry(pollutionSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollutionSourceExists(id))
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

        // POST: api/PollutionSources
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PollutionSource>> PostPollutionSource(PollutionSource pollutionSource)
        {
            _context.PollutionSource.Add(pollutionSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutionSource", new { id = pollutionSource.Id }, pollutionSource);
        }

        // DELETE: api/PollutionSources/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PollutionSource>> DeletePollutionSource(int id)
        {
            var pollutionSource = await _context.PollutionSource.FindAsync(id);
            if (pollutionSource == null)
            {
                return NotFound();
            }

            _context.PollutionSource.Remove(pollutionSource);
            await _context.SaveChangesAsync();

            return pollutionSource;
        }

        private bool PollutionSourceExists(int id)
        {
            return _context.PollutionSource.Any(e => e.Id == id);
        }

        // GET: api/PollutionSources/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<PollutionSource>>> GetPollutionSourceCount(string Name)
        {
            var pollutionSources = _context.PollutionSource
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                pollutionSources = pollutionSources.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await pollutionSources.CountAsync();

            return Ok(count);
        }
    }
}

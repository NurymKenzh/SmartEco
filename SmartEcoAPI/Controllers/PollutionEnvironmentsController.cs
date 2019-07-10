using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollutionEnvironmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutionEnvironmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PollutionEnvironments
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<IEnumerable<PollutionEnvironment>>> GetPollutionEnvironment(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var pollutionEnvironments = _context.PollutionEnvironment
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    pollutionEnvironments = pollutionEnvironments.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    pollutionEnvironments = pollutionEnvironments.OrderByDescending(m => m.NameEN);
                    break;
                default:
                    pollutionEnvironments = pollutionEnvironments.OrderBy(m => m.Id);
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
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
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
        [Authorize(Roles = "admin,moderator")]
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
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PollutionEnvironment>> PostPollutionEnvironment(PollutionEnvironment pollutionEnvironment)
        {
            _context.PollutionEnvironment.Add(pollutionEnvironment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutionEnvironment", new { id = pollutionEnvironment.Id }, pollutionEnvironment);
        }

        // DELETE: api/PollutionEnvironments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
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
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<IEnumerable<PollutionEnvironment>>> GetPollutionEnvironmentCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var pollutionEnvironments = _context.PollutionEnvironment
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                pollutionEnvironments = pollutionEnvironments.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await pollutionEnvironments.CountAsync();

            return Ok(count);
        }
    }
}
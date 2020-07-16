using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TargetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TargetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Targets
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<IEnumerable<Target>>> GetTarget(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PollutionEnvironmentId,
            int? PageSize,
            int? PageNumber)
        {
            var targets = _context.Target
                .Include(t => t.PollutionEnvironment)
                .Include(t => t.Project)
                .Where(m => true);

            var role = _context.Person
                .Where(p => p.Email == HttpContext.User.Identity.Name)
                .FirstOrDefault().Role;
            if (role == "Almaty")
            {
                targets = targets.Where(t => t.Project.Id == 3);
            }
            if (role == "Shymkent")
            {
                targets = targets.Where(t => t.Project.Id == 4);
            }

            if (!string.IsNullOrEmpty(NameKK))
            {
                targets = targets.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                targets = targets.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                targets = targets.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (PollutionEnvironmentId != null)
            {
                targets = targets.Where(m => m.PollutionEnvironmentId == PollutionEnvironmentId);
            }

            switch (SortOrder)
            {
                case "NameKK":
                    targets = targets.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    targets = targets.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    targets = targets.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    targets = targets.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    targets = targets.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    targets = targets.OrderByDescending(m => m.NameEN);
                    break;
                case "PollutionEnvironment":
                    targets = targets.OrderBy(k => k.PollutionEnvironment);
                    break;
                case "PollutionEnvironmentDesc":
                    targets = targets.OrderByDescending(k => k.PollutionEnvironment);
                    break;
                default:
                    targets = targets.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                targets = targets.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await targets.ToListAsync();
        }

        // GET: api/Targets/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<Target>> GetTarget(int id)
        {
            var target = await _context.Target
                .Include(t => t.PollutionEnvironment)
                .Include(t => t.MeasuredParameterUnit)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (target == null)
            {
                return NotFound();
            }

            return target;
        }

        // PUT: api/Targets/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<IActionResult> PutTarget(int id, Target target)
        {
            if (id != target.Id)
            {
                return BadRequest();
            }

            _context.Entry(target).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetExists(id))
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

        // POST: api/Targets
        [HttpPost]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<Target>> PostTarget(Target target)
        {
            _context.Target.Add(target);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarget", new { id = target.Id }, target);
        }

        // DELETE: api/Targets/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<Target>> DeleteTarget(int id)
        {
            var target = await _context.Target
                .Include(t => t.PollutionEnvironment)
                .Include(t => t.MeasuredParameterUnit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }

            _context.Target.Remove(target);
            await _context.SaveChangesAsync();

            return target;
        }

        private bool TargetExists(int id)
        {
            return _context.Target.Any(e => e.Id == id);
        }

        // GET: api/Targets/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<IEnumerable<Target>>> GetTargetCount(string NameKK,
            string NameRU,
            string NameEN,
            int? PollutionEnvironmentId)
        {
            var targets = _context.Target
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                targets = targets.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                targets = targets.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                targets = targets.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (PollutionEnvironmentId != null)
            {
                targets = targets.Where(m => m.PollutionEnvironmentId == PollutionEnvironmentId);
            }

            int count = await targets.CountAsync();

            return Ok(count);
        }
    }
}
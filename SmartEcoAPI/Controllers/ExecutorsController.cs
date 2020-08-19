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
    public class ExecutorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExecutorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Executors
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<Executor>>> GetExecutor(string SortOrder,
            string FullName,
            string Position,
            int? PageSize,
            int? PageNumber)
        {
            var executors = _context.Executor
                .Where(m => true);

            if (!string.IsNullOrEmpty(FullName))
            {
                executors = executors.Where(m => m.FullName.ToLower().Contains(FullName.ToLower()));
            }
            if (!string.IsNullOrEmpty(Position))
            {
                executors = executors.Where(m => m.Position.ToLower().Contains(Position.ToLower()));
            }

            switch (SortOrder)
            {
                case "FullName":
                    executors = executors.OrderBy(m => m.FullName);
                    break;
                case "FullNameDesc":
                    executors = executors.OrderByDescending(m => m.FullName);
                    break;
                case "Position":
                    executors = executors.OrderBy(m => m.Position);
                    break;
                case "PositionDesc":
                    executors = executors.OrderByDescending(m => m.Position);
                    break;
                default:
                    executors = executors.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                executors = executors.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await executors.ToListAsync();
        }

        // GET: api/Executors/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<Executor>> GetExecutor(int id)
        {
            var executor = await _context.Executor.FindAsync(id);

            if (executor == null)
            {
                return NotFound();
            }

            return executor;
        }

        // PUT: api/Executors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<IActionResult> PutExecutor(int id, Executor executor)
        {
            if (id != executor.Id)
            {
                return BadRequest();
            }

            _context.Entry(executor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExecutorExists(id))
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

        // POST: api/Executors
        [HttpPost]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<Executor>> PostExecutor(Executor executor)
        {
            _context.Executor.Add(executor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExecutor", new { id = executor.Id }, executor);
        }

        // DELETE: api/Executors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<Executor>> DeleteExecutor(int id)
        {
            var executor = await _context.Executor.FindAsync(id);
            if (executor == null)
            {
                return NotFound();
            }

            _context.Executor.Remove(executor);
            await _context.SaveChangesAsync();

            return executor;
        }

        private bool ExecutorExists(int id)
        {
            return _context.Executor.Any(e => e.Id == id);
        }

        // GET: api/Executors/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<Executor>>> GetExecutorCount(string FullName,
            string Position)
        {
            var executors = _context.Executor
                .Where(m => true);

            if (!string.IsNullOrEmpty(FullName))
            {
                executors = executors.Where(m => m.FullName.ToLower().Contains(FullName.ToLower()));
            }
            if (!string.IsNullOrEmpty(Position))
            {
                executors = executors.Where(m => m.Position.ToLower().Contains(Position.ToLower()));
            }

            int count = await executors.CountAsync();

            return Ok(count);
        }

        [HttpGet("CalcEfficiency")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<decimal>> CalcEfficiency(int ExecutorId) 
        {
            var aActivities = _context.AActivity
                .Join(_context.AActivityExecutor
                .Where(ae => ae.ExecutorId == ExecutorId), a => a.Id, ae => ae.AActivityId, (a, ae) => a)
                .ToList();
            List<decimal> multiplContrib = new List<decimal>();
            foreach (var aActivity in aActivities) 
            {
                var contribExecutor = _context.AActivityExecutor.Where(ae => ae.AActivityId == aActivity.Id && ae.ExecutorId == ExecutorId).FirstOrDefault().Contribution;
                multiplContrib.Add(Convert.ToDecimal((aActivity.Efficiency / 100) * (contribExecutor / 100)));
            }
            decimal efficiency = (multiplContrib.Sum() / aActivities.Count()) * 100;
            return efficiency;
        }
    }
}
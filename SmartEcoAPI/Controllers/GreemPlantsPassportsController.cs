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
    public class GreemPlantsPassportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GreemPlantsPassportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<GreemPlantsPassport>>> GetGreemPlantsPassport(string SortOrder,
            string GreenObject,
            int? KATOId,
            int? PageSize,
            int? PageNumber)
        {
            var greemPlantsPassports = _context.GreemPlantsPassport
                .Include(m => m.KATO)
                .Where(k => true);

            if (!string.IsNullOrEmpty(GreenObject))
            {
                greemPlantsPassports = greemPlantsPassports.Where(m => m.GreenObject.ToLower().Contains(GreenObject.ToLower()));
            }
            if (KATOId != null)
            {
                greemPlantsPassports = greemPlantsPassports.Where(m => m.KATOId == KATOId);
            }

            switch (SortOrder)
            {
                case "GreenObject":
                    greemPlantsPassports = greemPlantsPassports.OrderBy(k => k.GreenObject);
                    break;
                case "GreenObjectDesc":
                    greemPlantsPassports = greemPlantsPassports.OrderByDescending(k => k.GreenObject);
                    break;
                case "KATO":
                    greemPlantsPassports = greemPlantsPassports.OrderBy(k => k.KATO);
                    break;
                case "KATODesc":
                    greemPlantsPassports = greemPlantsPassports.OrderByDescending(k => k.KATO);
                    break;
                default:
                    greemPlantsPassports = greemPlantsPassports.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                greemPlantsPassports = greemPlantsPassports.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await greemPlantsPassports.ToListAsync();
        }

        // GET: api/GreemPlantsPassports/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GreemPlantsPassport>> GetGreemPlantsPassport(int id)
        {
            var greemPlantsPassport = await _context.GreemPlantsPassport
                .Include(m => m.KATO)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (greemPlantsPassport == null)
            {
                return NotFound();
            }

            return greemPlantsPassport;
        }

        // PUT: api/GreemPlantsPassports/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PutGreemPlantsPassport(int id, GreemPlantsPassport greemPlantsPassport)
        {
            if (id != greemPlantsPassport.Id)
            {
                return BadRequest();
            }

            _context.Entry(greemPlantsPassport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GreemPlantsPassportExists(id))
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

        // POST: api/GreemPlantsPassports
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GreemPlantsPassport>> PostGreemPlantsPassport(GreemPlantsPassport greemPlantsPassport)
        {
            _context.GreemPlantsPassport.Add(greemPlantsPassport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGreemPlantsPassport", new { id = greemPlantsPassport.Id }, greemPlantsPassport);
        }

        // DELETE: api/GreemPlantsPassports/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GreemPlantsPassport>> DeleteGreemPlantsPassport(int id)
        {
            var greemPlantsPassport = await _context.GreemPlantsPassport
                .Include(m => m.KATO)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (greemPlantsPassport == null)
            {
                return NotFound();
            }

            _context.GreemPlantsPassport.Remove(greemPlantsPassport);
            await _context.SaveChangesAsync();

            return greemPlantsPassport;
        }

        private bool GreemPlantsPassportExists(int id)
        {
            return _context.GreemPlantsPassport.Any(e => e.Id == id);
        }

        // GET: api/GreemPlantsPassports/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<GreemPlantsPassport>>> GetGreemPlantsPassportCount(string GreenObject,
            int? KATOId)
        {
            var greemPlantsPassports = _context.GreemPlantsPassport
                .Where(k => true);

            if (!string.IsNullOrEmpty(GreenObject))
            {
                greemPlantsPassports = greemPlantsPassports.Where(m => m.GreenObject.ToLower().Contains(GreenObject.ToLower()));
            }
            if (KATOId != null)
            {
                greemPlantsPassports = greemPlantsPassports.Where(m => m.KATOId == KATOId);
            }

            int count = await greemPlantsPassports.CountAsync();

            return Ok(count);
        }
    }
}
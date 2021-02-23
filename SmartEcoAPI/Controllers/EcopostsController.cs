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
    public class EcopostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EcopostsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        //[Authorize(Roles = "admin,moderator,Almaty,Kazhydromet")]
        public async Task<ActionResult<IEnumerable<Ecopost>>> GetEcopost(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var ecoposts = _context.Ecopost
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                ecoposts = ecoposts.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    ecoposts = ecoposts.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    ecoposts = ecoposts.OrderByDescending(k => k.Name);
                    break;
                default:
                    ecoposts = ecoposts.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                ecoposts = ecoposts.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await ecoposts.ToListAsync();
        }

        // GET: api/Ecoposts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Ecopost>> GetEcopost(int id)
        {
            var ecopost = await _context.Ecopost
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ecopost == null)
            {
                return NotFound();
            }

            return ecopost;
        }

        // PUT: api/Ecoposts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PutEcopost(int id, Ecopost ecopost)
        {
            if (id != ecopost.Id)
            {
                return BadRequest();
            }

            _context.Entry(ecopost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcopostExists(id))
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

        // POST: api/Ecoposts
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Ecopost>> PostEcopost(Ecopost ecopost)
        {
            _context.Ecopost.Add(ecopost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEcopost", new { id = ecopost.Id }, ecopost);
        }

        // DELETE: api/Ecoposts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Ecopost>> DeleteEcopost(int id)
        {
            var ecopost = await _context.Ecopost
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ecopost == null)
            {
                return NotFound();
            }

            _context.Ecopost.Remove(ecopost);
            await _context.SaveChangesAsync();

            return ecopost;
        }

        private bool EcopostExists(int id)
        {
            return _context.Ecopost.Any(e => e.Id == id);
        }

        // GET: api/Ecoposts/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Ecopost>>> GetEcopostCount(string Name)
        {
            var ecoposts = _context.Ecopost
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                ecoposts = ecoposts.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await ecoposts.CountAsync();

            return Ok(count);
        }
    }
}
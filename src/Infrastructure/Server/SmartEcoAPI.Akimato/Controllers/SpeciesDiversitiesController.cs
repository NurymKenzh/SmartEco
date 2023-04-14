using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Akimato.Data;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SpeciesDiversitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpeciesDiversitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SpeciesDiversities
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<SpeciesDiversity>>> GetSpeciesDiversity(string SortOrder,
            int? KATOId,
            int? PlantationsTypeId,
            int? PageSize,
            int? PageNumber)
        {
            var speciesDiversities = _context.SpeciesDiversity
                .Include(m => m.KATO)
                .Include(m => m.PlantationsType)
                .Where(k => true);

            if (KATOId != null)
            {
                speciesDiversities = speciesDiversities.Where(m => m.KATOId == KATOId);
            }
            if (PlantationsTypeId != null)
            {
                speciesDiversities = speciesDiversities.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            switch (SortOrder)
            {
                case "KATO":
                    speciesDiversities = speciesDiversities.OrderBy(k => k.KATO);
                    break;
                case "KATODesc":
                    speciesDiversities = speciesDiversities.OrderByDescending(k => k.KATO);
                    break;
                case "PlantationsType":
                    speciesDiversities = speciesDiversities.OrderBy(k => k.PlantationsType);
                    break;
                case "PlantationsTypeDesc":
                    speciesDiversities = speciesDiversities.OrderByDescending(k => k.PlantationsType);
                    break;
                default:
                    speciesDiversities = speciesDiversities.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                speciesDiversities = speciesDiversities.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await speciesDiversities.ToListAsync();
        }

        // GET: api/SpeciesDiversities/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<SpeciesDiversity>> GetSpeciesDiversity(int id)
        {
            var speciesDiversity = await _context.SpeciesDiversity
                .Include(m => m.KATO)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (speciesDiversity == null)
            {
                return NotFound();
            }

            return speciesDiversity;
        }

        // PUT: api/SpeciesDiversities/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutSpeciesDiversity(int id, SpeciesDiversity speciesDiversity)
        {
            if (id != speciesDiversity.Id)
            {
                return BadRequest();
            }

            _context.Entry(speciesDiversity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesDiversityExists(id))
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

        // POST: api/SpeciesDiversities
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<SpeciesDiversity>> PostSpeciesDiversity(SpeciesDiversity speciesDiversity)
        {
            _context.SpeciesDiversity.Add(speciesDiversity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpeciesDiversity", new { id = speciesDiversity.Id }, speciesDiversity);
        }

        // DELETE: api/SpeciesDiversities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<SpeciesDiversity>> DeleteSpeciesDiversity(int id)
        {
            var speciesDiversity = await _context.SpeciesDiversity
                .Include(m => m.KATO)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (speciesDiversity == null)
            {
                return NotFound();
            }

            _context.SpeciesDiversity.Remove(speciesDiversity);
            await _context.SaveChangesAsync();

            return speciesDiversity;
        }

        private bool SpeciesDiversityExists(int id)
        {
            return _context.SpeciesDiversity.Any(e => e.Id == id);
        }

        // GET: api/SpeciesDiversities/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<SpeciesDiversity>>> GetSpeciesDiversityCount(int? KATOId,
            int? PlantationsTypeId)
        {
            var speciesDiversities = _context.SpeciesDiversity
                .Where(k => true);

            if (KATOId != null)
            {
                speciesDiversities = speciesDiversities.Where(m => m.KATOId == KATOId);
            }
            if (PlantationsTypeId != null)
            {
                speciesDiversities = speciesDiversities.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            int count = await speciesDiversities.CountAsync();

            return Ok(count);
        }
    }
}
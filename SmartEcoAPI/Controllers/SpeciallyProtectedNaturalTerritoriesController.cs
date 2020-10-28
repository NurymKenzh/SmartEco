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
    public class SpeciallyProtectedNaturalTerritoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpeciallyProtectedNaturalTerritoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<SpeciallyProtectedNaturalTerritory>>> GetSpeciallyProtectedNaturalTerritory(string SortOrder,
            string NameEN,
            string NameRU,
            string NameKK,
            int? AuthorizedAuthorityId,
            int? PageSize,
            int? PageNumber)
        {
            var speciallyProtectedNaturalTerritories = _context.SpeciallyProtectedNaturalTerritory
                .Include(m => m.AuthorizedAuthority)
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameEN))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (AuthorizedAuthorityId != null)
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.AuthorizedAuthorityId == AuthorizedAuthorityId);
            }

            switch (SortOrder)
            {
                case "NameEN":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderBy(k => k.NameEN);
                    break;
                case "NameENDesc":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderByDescending(k => k.NameEN);
                    break;
                case "NameRU":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderBy(k => k.NameRU);
                    break;
                case "NameRUDesc":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderByDescending(k => k.NameRU);
                    break;
                case "NameKK":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderBy(k => k.NameKK);
                    break;
                case "NameKKDesc":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderByDescending(k => k.NameKK);
                    break;
                case "MonitoringPost":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderBy(k => k.AuthorizedAuthorityId);
                    break;
                case "MonitoringPostDesc":
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderByDescending(k => k.AuthorizedAuthorityId);
                    break;
                default:
                    speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await speciallyProtectedNaturalTerritories.ToListAsync();
        }

        // GET: api/SpeciallyProtectedNaturalTerritories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<SpeciallyProtectedNaturalTerritory>> GetSpeciallyProtectedNaturalTerritory(int id)
        {
            var speciallyProtectedNaturalTerritory = await _context.SpeciallyProtectedNaturalTerritory
                .Include(m => m.AuthorizedAuthority)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (speciallyProtectedNaturalTerritory == null)
            {
                return NotFound();
            }

            return speciallyProtectedNaturalTerritory;
        }

        // PUT: api/SpeciallyProtectedNaturalTerritories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<IActionResult> PutSpeciallyProtectedNaturalTerritory(int id, SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory)
        {
            if (id != speciallyProtectedNaturalTerritory.Id)
            {
                return BadRequest();
            }

            _context.Entry(speciallyProtectedNaturalTerritory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciallyProtectedNaturalTerritoryExists(id))
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

        // POST: api/SpeciallyProtectedNaturalTerritories
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<SpeciallyProtectedNaturalTerritory>> PostSpeciallyProtectedNaturalTerritory(SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory)
        {
            _context.SpeciallyProtectedNaturalTerritory.Add(speciallyProtectedNaturalTerritory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpeciallyProtectedNaturalTerritory", new { id = speciallyProtectedNaturalTerritory.Id }, speciallyProtectedNaturalTerritory);
        }

        // DELETE: api/SpeciallyProtectedNaturalTerritories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<SpeciallyProtectedNaturalTerritory>> DeleteSpeciallyProtectedNaturalTerritory(int id)
        {
            var speciallyProtectedNaturalTerritory = await _context.SpeciallyProtectedNaturalTerritory
                .Include(m => m.AuthorizedAuthority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (speciallyProtectedNaturalTerritory == null)
            {
                return NotFound();
            }

            _context.SpeciallyProtectedNaturalTerritory.Remove(speciallyProtectedNaturalTerritory);
            await _context.SaveChangesAsync();

            return speciallyProtectedNaturalTerritory;
        }

        private bool SpeciallyProtectedNaturalTerritoryExists(int id)
        {
            return _context.SpeciallyProtectedNaturalTerritory.Any(e => e.Id == id);
        }

        // GET: api/SpeciallyProtectedNaturalTerritories/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<SpeciallyProtectedNaturalTerritory>>> GetSpeciallyProtectedNaturalTerritoryCount(string NameEN,
            string NameRU,
            string NameKK,
            int? AuthorizedAuthorityId)
        {
            var speciallyProtectedNaturalTerritories = _context.SpeciallyProtectedNaturalTerritory
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameEN))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (AuthorizedAuthorityId != null)
            {
                speciallyProtectedNaturalTerritories = speciallyProtectedNaturalTerritories.Where(m => m.AuthorizedAuthorityId == AuthorizedAuthorityId);
            }

            int count = await speciallyProtectedNaturalTerritories.CountAsync();

            return Ok(count);
        }
    }
}
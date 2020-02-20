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
    public class TargetTerritoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TargetTerritoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TargetTerritories
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<IEnumerable<TargetTerritory>>> GetTargetTerritory(string SortOrder,
            string NameKK,
            string NameRU,
            string GISConnectionCode,
            int? TerritoryTypeId,
            int? PageSize,
            int? PageNumber)
        {
            var targetTerritories = _context.TargetTerritory
                .Include(t => t.KATO)
                .Include(t => t.TerritoryType)
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                targetTerritories = targetTerritories.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                targetTerritories = targetTerritories.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(GISConnectionCode))
            {
                targetTerritories = targetTerritories.Where(m => m.GISConnectionCode.ToLower().Contains(GISConnectionCode.ToLower()));
            }
            if (TerritoryTypeId != null)
            {
                targetTerritories = targetTerritories.Where(m => m.TerritoryTypeId == TerritoryTypeId);
            }

            switch (SortOrder)
            {
                case "NameKK":
                    targetTerritories = targetTerritories.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    targetTerritories = targetTerritories.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    targetTerritories = targetTerritories.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    targetTerritories = targetTerritories.OrderByDescending(m => m.NameRU);
                    break;
                case "GISConnectionCode":
                    targetTerritories = targetTerritories.OrderBy(m => m.GISConnectionCode);
                    break;
                case "GISConnectionCodeDesc":
                    targetTerritories = targetTerritories.OrderByDescending(m => m.GISConnectionCode);
                    break;
                case "TerritoryType":
                    targetTerritories = targetTerritories.OrderBy(m => m.TerritoryType);
                    break;
                case "TerritoryTypeDesc":
                    targetTerritories = targetTerritories.OrderByDescending(m => m.TerritoryType);
                    break;
                default:
                    targetTerritories = targetTerritories.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                targetTerritories = targetTerritories.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await targetTerritories.ToListAsync();
        }

        // GET: api/TargetTerritories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<TargetTerritory>> GetTargetTerritory(int id)
        {
            var targetTerritory = await _context.TargetTerritory
                .Include(t => t.KATO)
                .Include(t => t.TerritoryType)
                .Include(t => t.MonitoringPost)
                .Include(t => t.KazHydrometSoilPost)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (targetTerritory == null)
            {
                return NotFound();
            }

            return targetTerritory;
        }

        // PUT: api/TargetTerritories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutTargetTerritory(int id, TargetTerritory targetTerritory)
        {
            if (id != targetTerritory.Id)
            {
                return BadRequest();
            }

            _context.Entry(targetTerritory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetTerritoryExists(id))
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

        // POST: api/TargetTerritories
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<TargetTerritory>> PostTargetTerritory(TargetTerritory targetTerritory)
        {
            _context.TargetTerritory.Add(targetTerritory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTargetTerritory", new { id = targetTerritory.Id }, targetTerritory);
        }

        // DELETE: api/TargetTerritories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<TargetTerritory>> DeleteTargetTerritory(int id)
        {
            var targetTerritory = await _context.TargetTerritory
                .Include(t => t.KATO)
                .Include(t => t.TerritoryType)
                .Include(t => t.MonitoringPost)
                .Include(t => t.KazHydrometSoilPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (targetTerritory == null)
            {
                return NotFound();
            }

            _context.TargetTerritory.Remove(targetTerritory);
            await _context.SaveChangesAsync();

            return targetTerritory;
        }

        private bool TargetTerritoryExists(int id)
        {
            return _context.TargetTerritory.Any(e => e.Id == id);
        }

        // GET: api/TargetTerritories/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult<IEnumerable<TargetTerritory>>> GetTargetTerritoryCount(string NameKK,
            string NameRU,
            string GISConnectionCode,
            int? TerritoryTypeId)
        {
            var targetTerritories = _context.TargetTerritory
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                targetTerritories = targetTerritories.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                targetTerritories = targetTerritories.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(GISConnectionCode))
            {
                targetTerritories = targetTerritories.Where(m => m.GISConnectionCode.ToLower().Contains(GISConnectionCode.ToLower()));
            }
            if (TerritoryTypeId != null)
            {
                targetTerritories = targetTerritories.Where(m => m.TerritoryTypeId == TerritoryTypeId);
            }

            int count = await targetTerritories.CountAsync();

            return Ok(count);
        }
    }
}
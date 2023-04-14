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
    public class PlantationsStatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlantationsStatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlantationsStates
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsState>>> GetPlantationsState(string SortOrder,
            int? KATOId,
            int? PlantationsStateTypeId,
            int? PageSize,
            int? PageNumber)
        {
            var plantationsStates = _context.PlantationsState
                .Include(m => m.KATO)
                .Include(m => m.PlantationsStateType)
                .Where(k => true);

            if (KATOId != null)
            {
                plantationsStates = plantationsStates.Where(m => m.KATOId == KATOId);
            }
            if (PlantationsStateTypeId != null)
            {
                plantationsStates = plantationsStates.Where(m => m.PlantationsStateTypeId == PlantationsStateTypeId);
            }

            switch (SortOrder)
            {
                case "KATO":
                    plantationsStates = plantationsStates.OrderBy(k => k.KATO);
                    break;
                case "KATODesc":
                    plantationsStates = plantationsStates.OrderByDescending(k => k.KATO);
                    break;
                case "PlantationsStateType":
                    plantationsStates = plantationsStates.OrderBy(k => k.PlantationsStateType);
                    break;
                case "PlantationsStateTypeDesc":
                    plantationsStates = plantationsStates.OrderByDescending(k => k.PlantationsStateType);
                    break;
                default:
                    plantationsStates = plantationsStates.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                plantationsStates = plantationsStates.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await plantationsStates.ToListAsync();
        }

        // GET: api/PlantationsStates/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<PlantationsState>> GetPlantationsState(int id)
        {
            var plantationsState = await _context.PlantationsState
                .Include(m => m.KATO)
                .Include(m => m.PlantationsStateType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plantationsState == null)
            {
                return NotFound();
            }

            return plantationsState;
        }

        // PUT: api/PlantationsStates/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutPlantationsState(int id, PlantationsState plantationsState)
        {
            if (id != plantationsState.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantationsState).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantationsStateExists(id))
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

        // POST: api/PlantationsStates
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsState>> PostPlantationsState(PlantationsState plantationsState)
        {
            _context.PlantationsState.Add(plantationsState);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantationsState", new { id = plantationsState.Id }, plantationsState);
        }

        // DELETE: api/PlantationsStates/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsState>> DeletePlantationsState(int id)
        {
            var plantationsState = await _context.PlantationsState
                .Include(m => m.KATO)
                .Include(m => m.PlantationsStateType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantationsState == null)
            {
                return NotFound();
            }

            _context.PlantationsState.Remove(plantationsState);
            await _context.SaveChangesAsync();

            return plantationsState;
        }

        private bool PlantationsStateExists(int id)
        {
            return _context.PlantationsState.Any(e => e.Id == id);
        }

        // GET: api/PlantationsStates/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsState>>> GetPlantationsStateCount(int? KATOId,
            int? PlantationsStateTypeId)
        {
            var plantationsStates = _context.PlantationsState
                .Where(k => true);

            if (KATOId != null)
            {
                plantationsStates = plantationsStates.Where(m => m.KATOId == KATOId);
            }
            if (PlantationsStateTypeId != null)
            {
                plantationsStates = plantationsStates.Where(m => m.PlantationsStateTypeId == PlantationsStateTypeId);
            }

            int count = await plantationsStates.CountAsync();

            return Ok(count);
        }
    }
}
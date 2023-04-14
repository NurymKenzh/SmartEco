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
    public class PlantationsStateTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlantationsStateTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsStateType>>> GetPlantationsStateType(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var plantationsStateTypes = _context.PlantationsStateType
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    plantationsStateTypes = plantationsStateTypes.OrderBy(k => k.NameKK);
                    break;
                case "NameKKDesc":
                    plantationsStateTypes = plantationsStateTypes.OrderByDescending(k => k.NameKK);
                    break;
                case "NameRU":
                    plantationsStateTypes = plantationsStateTypes.OrderBy(k => k.NameRU);
                    break;
                case "NameRUDesc":
                    plantationsStateTypes = plantationsStateTypes.OrderByDescending(k => k.NameRU);
                    break;
                case "NameEN":
                    plantationsStateTypes = plantationsStateTypes.OrderBy(k => k.NameEN);
                    break;
                case "NameENDesc":
                    plantationsStateTypes = plantationsStateTypes.OrderByDescending(k => k.NameEN);
                    break;
                default:
                    plantationsStateTypes = plantationsStateTypes.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                plantationsStateTypes = plantationsStateTypes.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await plantationsStateTypes.ToListAsync();
        }

        // GET: api/PlantationsStateTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<PlantationsStateType>> GetPlantationsStateType(int id)
        {
            var plantationsStateType = await _context.PlantationsStateType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plantationsStateType == null)
            {
                return NotFound();
            }

            return plantationsStateType;
        }

        // PUT: api/PlantationsStateTypes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutPlantationsStateType(int id, PlantationsStateType plantationsStateType)
        {
            if (id != plantationsStateType.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantationsStateType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantationsStateTypeExists(id))
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

        // POST: api/PlantationsStateTypes
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsStateType>> PostPlantationsStateType(PlantationsStateType plantationsStateType)
        {
            _context.PlantationsStateType.Add(plantationsStateType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantationsStateType", new { id = plantationsStateType.Id }, plantationsStateType);
        }

        // DELETE: api/PlantationsStateTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsStateType>> DeletePlantationsStateType(int id)
        {
            var plantationsStateType = await _context.PlantationsStateType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantationsStateType == null)
            {
                return NotFound();
            }

            _context.PlantationsStateType.Remove(plantationsStateType);
            await _context.SaveChangesAsync();

            return plantationsStateType;
        }

        private bool PlantationsStateTypeExists(int id)
        {
            return _context.PlantationsStateType.Any(e => e.Id == id);
        }

        // GET: api/PlantationsStateTypes/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsStateType>>> GetPlantationsStateTypeCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var plantationsStateTypes = _context.PlantationsStateType
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                plantationsStateTypes = plantationsStateTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }


            int count = await plantationsStateTypes.CountAsync();

            return Ok(count);
        }

    }
}
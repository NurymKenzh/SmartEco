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
    public class PlantationsTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlantationsTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsType>>> GetPlantationsType(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var plantationsTypes = _context.PlantationsType
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    plantationsTypes = plantationsTypes.OrderBy(k => k.NameKK);
                    break;
                case "NameKKDesc":
                    plantationsTypes = plantationsTypes.OrderByDescending(k => k.NameKK);
                    break;
                case "NameRU":
                    plantationsTypes = plantationsTypes.OrderBy(k => k.NameRU);
                    break;
                case "NameRUDesc":
                    plantationsTypes = plantationsTypes.OrderByDescending(k => k.NameRU);
                    break;
                case "NameEN":
                    plantationsTypes = plantationsTypes.OrderBy(k => k.NameEN);
                    break;
                case "NameENDesc":
                    plantationsTypes = plantationsTypes.OrderByDescending(k => k.NameEN);
                    break;
                default:
                    plantationsTypes = plantationsTypes.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                plantationsTypes = plantationsTypes.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await plantationsTypes.ToListAsync();
        }

        // GET: api/PlantationsTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<PlantationsType>> GetPlantationsType(int id)
        {
            var plantationsType = await _context.PlantationsType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plantationsType == null)
            {
                return NotFound();
            }

            return plantationsType;
        }

        // PUT: api/PlantationsTypes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutPlantationsType(int id, PlantationsType plantationsType)
        {
            if (id != plantationsType.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantationsType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantationsTypeExists(id))
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

        // POST: api/PlantationsTypes
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsType>> PostPlantationsType(PlantationsType plantationsType)
        {
            _context.PlantationsType.Add(plantationsType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantationsType", new { id = plantationsType.Id }, plantationsType);
        }

        // DELETE: api/PlantationsTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<PlantationsType>> DeletePlantationsType(int id)
        {
            var plantationsType = await _context.PlantationsType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantationsType == null)
            {
                return NotFound();
            }

            _context.PlantationsType.Remove(plantationsType);
            await _context.SaveChangesAsync();

            return plantationsType;
        }

        private bool PlantationsTypeExists(int id)
        {
            return _context.PlantationsType.Any(e => e.Id == id);
        }

        // GET: api/PlantationsTypes/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<PlantationsType>>> GetPlantationsTypeCount(string NameKK,
            string NameRU, 
            string NameEN)
        {
            var plantationsTypes = _context.PlantationsType
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                plantationsTypes = plantationsTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }


            int count = await plantationsTypes.CountAsync();

            return Ok(count);
        }

    }    
}
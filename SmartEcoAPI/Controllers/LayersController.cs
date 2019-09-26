using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LayersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Layers
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<Layer>>> GetLayer(string SortOrder,
            string GeoServerName,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var layers = _context.Layer
                .Include(l => l.KATO)
                .Include(l => l.MeasuredParameter)
                .Include(l => l.PollutionEnvironment)
                .Where(l => true);

            if (!string.IsNullOrEmpty(GeoServerName))
            {
                layers = layers.Where(l => l.GeoServerName.ToLower().Contains(GeoServerName));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                layers = layers.Where(l => l.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                layers = layers.Where(l => l.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                layers = layers.Where(l => l.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "GeoServerName":
                    layers = layers.OrderBy(l => l.GeoServerName);
                    break;
                case "GeoServerNameDesc":
                    layers = layers.OrderByDescending(l => l.GeoServerName);
                    break;
                case "NameKK":
                    layers = layers.OrderBy(l => l.NameKK);
                    break;
                case "NameKKDesc":
                    layers = layers.OrderByDescending(l => l.NameKK);
                    break;
                case "NameRU":
                    layers = layers.OrderBy(l => l.NameRU);
                    break;
                case "Desc":
                    layers = layers.OrderByDescending(l => l.NameRU);
                    break;
                case "NameEN":
                    layers = layers.OrderBy(l => l.NameEN);
                    break;
                case "NameENDesc":
                    layers = layers.OrderByDescending(l => l.NameEN);
                    break;
                default:
                    layers = layers.OrderBy(l => l.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                layers = layers.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await layers.ToListAsync();
        }

        // GET: api/Layers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Layer>> GetLayer(int id)
        {
            var layer = await _context.Layer
                .Include(l => l.KATO)
                .Include(l => l.MeasuredParameter)
                .Include(l => l.PollutionEnvironment)
                .FirstOrDefaultAsync(m => m.Id == id);
            //.FindAsync(id);

            if (layer == null)
            {
                return NotFound();
            }

            return layer;
        }

        // PUT: api/Layers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutLayer(int id, Layer layer)
        {
            if (id != layer.Id)
            {
                return BadRequest();
            }

            _context.Entry(layer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayerExists(id))
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

        // POST: api/Layers
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Layer>> PostLayer(Layer layer)
        {
            _context.Layer.Add(layer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLayer", new { id = layer.Id }, layer);
        }

        // DELETE: api/Layers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Layer>> DeleteLayer(int id)
        {
            var layer = await _context.Layer
                .Include(l => l.KATO)
                .Include(l => l.MeasuredParameter)
                .Include(l => l.PollutionEnvironment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            _context.Layer.Remove(layer);
            await _context.SaveChangesAsync();

            return layer;
        }

        private bool LayerExists(int id)
        {
            return _context.Layer.Any(e => e.Id == id);
        }

        // GET: api/Layers/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<Layer>>> GetLayersCount(string GeoServerName,
            string NameKK,
            string NameRU,
            string NameEN)
        {
            var layers = _context.Layer
                 .Where(l => true);

            if (!string.IsNullOrEmpty(GeoServerName))
            {
                layers = layers.Where(l => l.GeoServerName.ToLower().Contains(GeoServerName));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                layers = layers.Where(l => l.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                layers = layers.Where(l => l.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                layers = layers.Where(l => l.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await layers.CountAsync();

            return Ok(count);
        }
    }
}

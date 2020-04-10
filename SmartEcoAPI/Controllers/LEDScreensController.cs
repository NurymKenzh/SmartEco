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
    public class LEDScreensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LEDScreensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LEDScreens
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        public async Task<ActionResult<IEnumerable<LEDScreen>>> GetLEDScreen(string SortOrder,
            string Name,
            int? MonitoringPostId,
            int? PageSize,
            int? PageNumber)
        {
            var ledScreens = _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .Where(k => true);
            
            if (!string.IsNullOrEmpty(Name))
            {
                ledScreens = ledScreens.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (MonitoringPostId != null)
            {
                ledScreens = ledScreens.Where(m => m.MonitoringPostId == MonitoringPostId);
            }

            switch (SortOrder)
            {
                case "Name":
                    ledScreens = ledScreens.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    ledScreens = ledScreens.OrderByDescending(k => k.Name);
                    break;
                case "MonitoringPost":
                    ledScreens = ledScreens.OrderBy(k => k.MonitoringPost);
                    break;
                case "MonitoringPostDesc":
                    ledScreens = ledScreens.OrderByDescending(k => k.MonitoringPost);
                    break;
                default:
                    ledScreens = ledScreens.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                ledScreens = ledScreens.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await ledScreens.ToListAsync();
        }

        // GET: api/LEDScreens/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        public async Task<ActionResult<LEDScreen>> GetLEDScreen(int id)
        {
            var ledScreen = await _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ledScreen == null)
            {
                return NotFound();
            }

            return ledScreen;
        }

        // PUT: api/LEDScreens/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutLEDScreen(int id, LEDScreen ledScreen)
        {
            if (id != ledScreen.Id)
            {
                return BadRequest();
            }

            _context.Entry(ledScreen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LEDScreenExists(id))
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

        // POST: api/LEDScreens
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<LEDScreen>> PostLEDScreen(LEDScreen ledScreen)
        {
            _context.LEDScreen.Add(ledScreen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLEDScreen", new { id = ledScreen.Id }, ledScreen);
        }

        // DELETE: api/LEDScreens/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<LEDScreen>> DeleteLEDScreen(int id)
        {
            var ledScreen = await _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ledScreen == null)
            {
                return NotFound();
            }

            _context.LEDScreen.Remove(ledScreen);
            await _context.SaveChangesAsync();

            return ledScreen;
        }

        private bool LEDScreenExists(int id)
        {
            return _context.LEDScreen.Any(e => e.Id == id);
        }

        // GET: api/LEDScreens/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        public async Task<ActionResult<IEnumerable<LEDScreen>>> GetLEDScreenCount(string Name,
            int? MonitoringPostId)
        {
            var ledScreens = _context.LEDScreen
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                ledScreens = ledScreens.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (MonitoringPostId != null)
            {
                ledScreens = ledScreens.Where(m => m.MonitoringPostId == MonitoringPostId);
            }

            int count = await ledScreens.CountAsync();

            return Ok(count);
        }
    }
}
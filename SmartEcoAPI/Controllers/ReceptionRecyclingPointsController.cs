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
    public class ReceptionRecyclingPointsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReceptionRecyclingPointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<ReceptionRecyclingPoint>>> GetReceptionRecyclingPoint(string SortOrder,
            string Organization,
            string TypesRaw,
            int? PageSize,
            int? PageNumber)
        {
            var receptionRecyclingPoints = _context.ReceptionRecyclingPoint
                .Include(m => m.Project)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Organization))
            {
                receptionRecyclingPoints = receptionRecyclingPoints.Where(m => m.Organization.ToLower().Contains(Organization.ToLower()));
            }
            if (!string.IsNullOrEmpty(TypesRaw))
            {
                receptionRecyclingPoints = receptionRecyclingPoints.Where(m => m.TypesRaw.ToLower().Contains(TypesRaw.ToLower()));
            }

            switch (SortOrder)
            {
                case "Organization":
                    receptionRecyclingPoints = receptionRecyclingPoints.OrderBy(a => a.Organization);
                    break;
                case "OrganizationDesc":
                    receptionRecyclingPoints = receptionRecyclingPoints.OrderByDescending(a => a.Organization);
                    break;
                case "TypesRaw":
                    receptionRecyclingPoints = receptionRecyclingPoints.OrderBy(a => a.TypesRaw);
                    break;
                case "TypesRawDesc":
                    receptionRecyclingPoints = receptionRecyclingPoints.OrderByDescending(a => a.TypesRaw);
                    break;
                default:
                    receptionRecyclingPoints = receptionRecyclingPoints.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                receptionRecyclingPoints = receptionRecyclingPoints.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await receptionRecyclingPoints.ToListAsync();
        }

        // GET: api/ReceptionRecyclingPoints/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<ReceptionRecyclingPoint>> GetReceptionRecyclingPoint(int id)
        {
            var receptionRecyclingPoint = await _context.ReceptionRecyclingPoint
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (receptionRecyclingPoint == null)
            {
                return NotFound();
            }

            return receptionRecyclingPoint;
        }

        // PUT: api/ReceptionRecyclingPoints/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<IActionResult> PutReceptionRecyclingPoint(int id, ReceptionRecyclingPoint receptionRecyclingPoint)
        {
            if (id != receptionRecyclingPoint.Id)
            {
                return BadRequest();
            }

            _context.Entry(receptionRecyclingPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceptionRecyclingPointExists(id))
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

        // POST: api/ReceptionRecyclingPoints
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<ReceptionRecyclingPoint>> PostReceptionRecyclingPoint(ReceptionRecyclingPoint receptionRecyclingPoint)
        {
            _context.ReceptionRecyclingPoint.Add(receptionRecyclingPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceptionRecyclingPoint", new { id = receptionRecyclingPoint.Id }, receptionRecyclingPoint);
        }

        // DELETE: api/ReceptionRecyclingPoints/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<ReceptionRecyclingPoint>> DeleteReceptionRecyclingPoint(int id)
        {
            var receptionRecyclingPoint = await _context.ReceptionRecyclingPoint
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receptionRecyclingPoint == null)
            {
                return NotFound();
            }

            _context.ReceptionRecyclingPoint.Remove(receptionRecyclingPoint);
            await _context.SaveChangesAsync();

            return receptionRecyclingPoint;
        }

        private bool ReceptionRecyclingPointExists(int id)
        {
            return _context.ReceptionRecyclingPoint.Any(e => e.Id == id);
        }

        // GET: api/ReceptionRecyclingPoints/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<ReceptionRecyclingPoint>>> GetReceptionRecyclingPointCount(string Organization,
            string TypesRaw)
        {
            var receptionRecyclingPoints = _context.ReceptionRecyclingPoint
                .Where(k => true);

            if (!string.IsNullOrEmpty(Organization))
            {
                receptionRecyclingPoints = receptionRecyclingPoints.Where(m => m.Organization.ToLower().Contains(Organization.ToLower()));
            }
            if (!string.IsNullOrEmpty(TypesRaw))
            {
                receptionRecyclingPoints = receptionRecyclingPoints.Where(m => m.TypesRaw.ToLower().Contains(TypesRaw.ToLower()));
            }

            int count = await receptionRecyclingPoints.CountAsync();

            return Ok(count);
        }
    }
}
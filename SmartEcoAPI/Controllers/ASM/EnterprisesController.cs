using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "admin,moderator,ASM")]
    public class EnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EnterprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Enterprises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterprise(string SortOrder,
            string Name,
            string City,
            int? CompanyId,
            int? PageSize,
            int? PageNumber)
        {
            var enterprises = _context.Enterprise
                .Include(m => m.Company)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                enterprises = enterprises.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(City))
            {
                enterprises = enterprises.Where(m => m.City.ToLower().Contains(City.ToLower()));
            }
            if (CompanyId != null)
            {
                enterprises = enterprises.Where(m => m.CompanyId == CompanyId);
            }

            switch (SortOrder)
            {
                case "Name":
                    enterprises = enterprises.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    enterprises = enterprises.OrderByDescending(k => k.Name);
                    break;
                case "City":
                    enterprises = enterprises.OrderBy(k => k.City);
                    break;
                case "CityDesc":
                    enterprises = enterprises.OrderByDescending(k => k.City);
                    break;
                case "Company":
                    enterprises = enterprises.OrderBy(k => k.Company);
                    break;
                case "CompanyDesc":
                    enterprises = enterprises.OrderByDescending(k => k.Company);
                    break;
                default:
                    enterprises = enterprises.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                enterprises = enterprises.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await enterprises.ToListAsync();
        }

        // GET: api/Enterprises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
        {
            var enterprise = await _context.Enterprise
                .Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
            {
                return NotFound();
            }

            return enterprise;
        }

        // PUT: api/Enterprises/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnterprise(int id, Enterprise enterprise)
        {
            if (id != enterprise.Id)
            {
                return BadRequest();
            }

            _context.Entry(enterprise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(id))
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

        // POST: api/Enterprises
        [HttpPost]
        public async Task<ActionResult<Enterprise>> PostEnterprise(Enterprise enterprise)
        {
            _context.Enterprise.Add(enterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnterprise", new { id = enterprise.Id }, enterprise);
        }

        // DELETE: api/Enterprises/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Enterprise>> DeleteEnterprise(int id)
        {
            var enterprise = await _context.Enterprise
                .Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
            {
                return NotFound();
            }

            _context.Enterprise.Remove(enterprise);
            await _context.SaveChangesAsync();

            return enterprise;
        }

        private bool EnterpriseExists(int id)
        {
            return _context.Enterprise.Any(e => e.Id == id);
        }

        // GET: api/Enterprises/Count
        [HttpGet("count")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterpriseCount(string Name,
            string City,
            int? CompanyId)
        {
            var enterprises = _context.Enterprise
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                enterprises = enterprises.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(City))
            {
                enterprises = enterprises.Where(m => m.City.ToLower().Contains(City.ToLower()));
            }
            if (CompanyId != null)
            {
                enterprises = enterprises.Where(m => m.CompanyId == CompanyId);
            }

            int count = await enterprises.CountAsync();

            return Ok(count);
        }
    }
}

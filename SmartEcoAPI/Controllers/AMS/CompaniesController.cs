using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartEcoAPI.Controllers.AMS
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var companies = _context.Company
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                companies = companies.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    companies = companies.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    companies = companies.OrderByDescending(m => m.Name);
                    break;
                default:
                    companies = companies.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                companies = companies.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Companies
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Company>> DeleteCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return company;
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }

        // GET: api/Companies/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,AMS")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanyCount(string Name)
        {
            var companies = _context.Company
            .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                companies = companies.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await companies.CountAsync();

            return Ok(count);
        }
    }
}

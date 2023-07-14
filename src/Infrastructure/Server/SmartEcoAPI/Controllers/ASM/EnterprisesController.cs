using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnterprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Enterprises
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterprisesResponse>> GetEnterprises([FromBody] EnterprisesRequest request)
        {
            var enterprises = _context.Enterprise
                .Include(e => e.Kato)
                .Include(e => e.EnterpriseType)
                .Where(m => true);

            if (request.Bin != null)
            {
                enterprises = enterprises.Where(m => m.Bin.ToString().StartsWith(request.Bin.Value.ToString()));
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                enterprises = enterprises.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.KatoComplex))
            {
                enterprises = enterprises.Where(m => $"{m.Kato.Code} {m.Kato.NameRU}".ToLower().Contains(request.KatoComplex.ToLower()));
            }
            if (request.EnterpriseTypeId != null)
            {
                enterprises = enterprises.Where(m => m.EnterpriseTypeId == request.EnterpriseTypeId);
            }

            switch (request.SortOrder)
            {
                case "Name":
                    enterprises = enterprises.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    enterprises = enterprises.OrderByDescending(m => m.Name);
                    break;
                default:
                    enterprises = enterprises.OrderBy(m => m.Id);
                    break;
            }

            var count = await enterprises.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                enterprises = enterprises.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new EnterprisesResponse(await enterprises.ToListAsync(), count);

            return response;
        }

        // GET: api/Enterprises/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
        {
            var enterprise = await _context.Enterprise
                .Include(e => e.Kato)
                .Include(e => e.EnterpriseType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
            {
                return NotFound();
            }

            return enterprise;
        }

        // PUT: api/Enterprises/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
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
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Enterprise>> PostEnterprise(Enterprise enterprise)
        {
            _context.Enterprise.Add(enterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnterprise", new { id = enterprise.Id }, enterprise);
        }

        // DELETE: api/Enterprises/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Enterprise>> DeleteEnterprise(int id)
        {
            var enterprise = await _context.Enterprise.FindAsync(id);
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
    }
}
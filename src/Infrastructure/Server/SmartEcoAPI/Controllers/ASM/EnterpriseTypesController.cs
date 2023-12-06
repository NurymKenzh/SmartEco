using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EnterpriseTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnterpriseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EnterpriseTypes
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseTypesResponse>> GetEnterpriseTypes([FromBody] EnterpriseTypesRequest request)
        {
            var enterpriseTypes = _context.EnterpriseType
                .Where(m => true);

            if (!string.IsNullOrEmpty(request.Name))
            {
                enterpriseTypes = enterpriseTypes.Where(m => m.Name.ToLower().Contains(request.Name.ToLower()));
            }

            switch (request.SortOrder)
            {
                case "Name":
                    enterpriseTypes = enterpriseTypes.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    enterpriseTypes = enterpriseTypes.OrderByDescending(m => m.Name);
                    break;
                default:
                    enterpriseTypes = enterpriseTypes.OrderBy(m => m.Id);
                    break;
            }

            var count = await enterpriseTypes.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                enterpriseTypes = enterpriseTypes.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new EnterpriseTypesResponse(await enterpriseTypes.ToListAsync(), count);

            return response;
        }

        // GET: api/EnterpriseTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseType>> GetEnterpriseType(int id)
        {
            var enterpriseType = await _context.EnterpriseType.FindAsync(id);

            if (enterpriseType == null)
            {
                return NotFound();
            }

            return enterpriseType;
        }

        // PUT: api/EnterpriseTypes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PutEnterpriseType(int id, EnterpriseType enterpriseType)
        {
            if (id != enterpriseType.Id)
            {
                return BadRequest();
            }

            _context.Entry(enterpriseType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseTypeExists(id))
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

        // POST: api/EnterpriseTypes
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseType>> PostEnterpriseType(EnterpriseType enterpriseType)
        {
            _context.EnterpriseType.Add(enterpriseType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnterpriseType", new { id = enterpriseType.Id }, enterpriseType);
        }

        // DELETE: api/EnterpriseTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseType>> DeleteEnterpriseType(int id)
        {
            var enterpriseType = await _context.EnterpriseType.FindAsync(id);
            if (enterpriseType == null)
            {
                return NotFound();
            }

            _context.EnterpriseType.Remove(enterpriseType);
            await _context.SaveChangesAsync();

            return enterpriseType;
        }

        private bool EnterpriseTypeExists(int id)
        {
            return _context.EnterpriseType.Any(e => e.Id == id);
        }
    }
}
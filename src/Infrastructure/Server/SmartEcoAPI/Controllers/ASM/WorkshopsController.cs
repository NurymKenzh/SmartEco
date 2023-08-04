using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WorkshopsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkshopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Workshops
        [HttpGet]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<IEnumerable<Workshop>>> GetWorkshops(WorkshopsRequest request)
        {
            var workshops = _context.Workshop
                .Include(w => w.IndSiteEnterprise)
                .ThenInclude(w => w.Enterprise)
                .Where(m => true);

            if (request?.EnterpriseId != null)
            {
                workshops = workshops.Where(m => m.IndSiteEnterprise.EnterpriseId == request.EnterpriseId);
            }

            return await workshops.ToListAsync();
        }

        // GET: api/Workshops/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<Workshop>> GetWorkshop(int id)
        {
            var workshop = await _context.Workshop
                .Include(w => w.IndSiteEnterprise)
                .ThenInclude(w => w.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workshop == null)
            {
                return NotFound();
            }

            return workshop;
        }

        // PUT: api/Workshops/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> PutWorkshop(int id, Workshop workshop)
        {
            if (id != workshop.Id)
            {
                return BadRequest();
            }

            _context.Entry(workshop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkshopExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetEnterpriseId(workshop.Id);
        }

        // POST: api/Workshops
        [HttpPost]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> PostWorkshop(Workshop workshop)
        {
            _context.Workshop.Add(workshop);
            await _context.SaveChangesAsync();

            return await GetEnterpriseId(workshop.Id);
        }

        // DELETE: api/Workshops/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<EnterpriseResponse>> DeleteWorkshop(int id)
        {
            var workshop = await _context.Workshop.FindAsync(id);
            var enerpriseResponse = await GetEnterpriseId(id);
            if (workshop == null)
            {
                return NotFound();
            }

            _context.Workshop.Remove(workshop);
            await _context.SaveChangesAsync();

            return enerpriseResponse;
        }

        private bool WorkshopExists(int id)
        {
            return _context.Workshop.Any(e => e.Id == id);
        }

        private async Task<EnterpriseResponse> GetEnterpriseId(int workshopId)
        {
            var workshop = await _context.Workshop
                .Include(a => a.IndSiteEnterprise)
                .ThenInclude(a => a.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == workshopId);

            return new EnterpriseResponse()
            {
                Id = workshop.IndSiteEnterprise.EnterpriseId
            };
        }
    }
}

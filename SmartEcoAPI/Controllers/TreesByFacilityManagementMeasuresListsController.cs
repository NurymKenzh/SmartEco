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
    public class TreesByFacilityManagementMeasuresListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreesByFacilityManagementMeasuresListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<TreesByFacilityManagementMeasuresList>>> GetTreesByFacilityManagementMeasuresList(string SortOrder,
            string Name,
            int? GreemPlantsPassportId,
            int? PlantationsTypeId,
            int? PageSize,
            int? PageNumber)
        {
            var treesByFacilityManagementMeasuresLists = _context.TreesByFacilityManagementMeasuresList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .Where(k => true);

            if (GreemPlantsPassportId != null)
            {
                treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.Where(m => m.GreemPlantsPassportId == GreemPlantsPassportId);
            }
            if (PlantationsTypeId != null)
            {
                treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            switch (SortOrder)
            {
                case "GreemPlantsPassportId":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderBy(a => a.GreemPlantsPassport.GreenObject);
                    break;
                case "GreemPlantsPassportIdDesc":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderByDescending(a => a.GreemPlantsPassport.GreenObject);
                    break;
                case "PlantationsTypeId":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderBy(a => a.PlantationsType);
                    break;
                case "PlantationsTypeIdDesc":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderByDescending(a => a.PlantationsType);
                    break;
                case "Quantity":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderBy(a => a.Quantity);
                    break;
                case "QuantityDesc":
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderByDescending(a => a.Quantity);
                    break;
                default:
                    treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await treesByFacilityManagementMeasuresLists.ToListAsync();
        }

        // GET: api/TreesByFacilityManagementMeasuresLists/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<TreesByFacilityManagementMeasuresList>> GetTreesByFacilityManagementMeasuresList(int id)
        {
            var treesByFacilityManagementMeasuresList = await _context.TreesByFacilityManagementMeasuresList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (treesByFacilityManagementMeasuresList == null)
            {
                return NotFound();
            }

            return treesByFacilityManagementMeasuresList;
        }

        // PUT: api/TreesByFacilityManagementMeasuresLists/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<IActionResult> PutTreesByFacilityManagementMeasuresList(int id, TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList)
        {
            if (id != treesByFacilityManagementMeasuresList.Id)
            {
                return BadRequest();
            }

            _context.Entry(treesByFacilityManagementMeasuresList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreesByFacilityManagementMeasuresListExists(id))
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

        // POST: api/TreesByFacilityManagementMeasuresLists
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<TreesByFacilityManagementMeasuresList>> PostTreesByFacilityManagementMeasuresList(TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList)
        {
            _context.TreesByFacilityManagementMeasuresList.Add(treesByFacilityManagementMeasuresList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTreesByFacilityManagementMeasuresList", new { id = treesByFacilityManagementMeasuresList.Id }, treesByFacilityManagementMeasuresList);
        }

        // DELETE: api/TreesByFacilityManagementMeasuresLists/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<TreesByFacilityManagementMeasuresList>> DeleteTreesByFacilityManagementMeasuresList(int id)
        {
            var treesByFacilityManagementMeasuresList = await _context.TreesByFacilityManagementMeasuresList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treesByFacilityManagementMeasuresList == null)
            {
                return NotFound();
            }

            _context.TreesByFacilityManagementMeasuresList.Remove(treesByFacilityManagementMeasuresList);
            await _context.SaveChangesAsync();

            return treesByFacilityManagementMeasuresList;
        }

        private bool TreesByFacilityManagementMeasuresListExists(int id)
        {
            return _context.TreesByFacilityManagementMeasuresList.Any(e => e.Id == id);
        }

        // GET: api/TreesByFacilityManagementMeasuresLists/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<TreesByFacilityManagementMeasuresList>>> GetTreesByFacilityManagementMeasuresListCount(int? GreemPlantsPassportId,
            int? PlantationsTypeId)
        {
            var treesByFacilityManagementMeasuresLists = _context.TreesByFacilityManagementMeasuresList
                .Where(k => true);

            if (GreemPlantsPassportId != null)
            {
                treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.Where(m => m.GreemPlantsPassportId == GreemPlantsPassportId);
            }
            if (PlantationsTypeId != null)
            {
                treesByFacilityManagementMeasuresLists = treesByFacilityManagementMeasuresLists.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            int count = await treesByFacilityManagementMeasuresLists.CountAsync();

            return Ok(count);
        }
    }
}
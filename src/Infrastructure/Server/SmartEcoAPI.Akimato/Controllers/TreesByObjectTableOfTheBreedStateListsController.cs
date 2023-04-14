using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Akimato.Data;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TreesByObjectTableOfTheBreedStateListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreesByObjectTableOfTheBreedStateListsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<TreesByObjectTableOfTheBreedStateList>>> GetTreesByObjectTableOfTheBreedStateList(string SortOrder,
            string Name,
            int? GreemPlantsPassportId,
            int? PlantationsTypeId,
            int? PageSize,
            int? PageNumber)
        {
            var treesByObjectTableOfTheBreedStateLists = _context.TreesByObjectTableOfTheBreedStateList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .Where(k => true);
            
            if (GreemPlantsPassportId != null)
            {
                treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.Where(m => m.GreemPlantsPassportId == GreemPlantsPassportId);
            }
            if (PlantationsTypeId != null)
            {
                treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            switch (SortOrder)
            {
                case "GreemPlantsPassportId":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderBy(a => a.GreemPlantsPassport.GreenObject);
                    break;
                case "GreemPlantsPassportIdDesc":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderByDescending(a => a.GreemPlantsPassport.GreenObject);
                    break;
                case "PlantationsTypeId":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderBy(a => a.PlantationsType);
                    break;
                case "PlantationsTypeIdDesc":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderByDescending(a => a.PlantationsType);
                    break;
                case "Quantity":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderBy(a => a.Quantity);
                    break;
                case "QuantityDesc":
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderByDescending(a => a.Quantity);
                    break;
                default:
                    treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await treesByObjectTableOfTheBreedStateLists.ToListAsync();
        }

        // GET: api/TreesByObjectTableOfTheBreedStateLists/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<TreesByObjectTableOfTheBreedStateList>> GetTreesByObjectTableOfTheBreedStateList(int id)
        {
            var treesByObjectTableOfTheBreedStateList = await _context.TreesByObjectTableOfTheBreedStateList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (treesByObjectTableOfTheBreedStateList == null)
            {
                return NotFound();
            }

            return treesByObjectTableOfTheBreedStateList;
        }

        // PUT: api/TreesByObjectTableOfTheBreedStateLists/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<IActionResult> PutTreesByObjectTableOfTheBreedStateList(int id, TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList)
        {
            if (id != treesByObjectTableOfTheBreedStateList.Id)
            {
                return BadRequest();
            }

            _context.Entry(treesByObjectTableOfTheBreedStateList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreesByObjectTableOfTheBreedStateListExists(id))
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

        // POST: api/TreesByObjectTableOfTheBreedStateLists
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<TreesByObjectTableOfTheBreedStateList>> PostTreesByObjectTableOfTheBreedStateList(TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList)
        {
            _context.TreesByObjectTableOfTheBreedStateList.Add(treesByObjectTableOfTheBreedStateList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTreesByObjectTableOfTheBreedStateList", new { id = treesByObjectTableOfTheBreedStateList.Id }, treesByObjectTableOfTheBreedStateList);
        }

        // DELETE: api/TreesByObjectTableOfTheBreedStateLists/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<TreesByObjectTableOfTheBreedStateList>> DeleteTreesByObjectTableOfTheBreedStateList(int id)
        {
            var treesByObjectTableOfTheBreedStateList = await _context.TreesByObjectTableOfTheBreedStateList
                .Include(m => m.GreemPlantsPassport)
                .Include(m => m.PlantationsType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treesByObjectTableOfTheBreedStateList == null)
            {
                return NotFound();
            }

            _context.TreesByObjectTableOfTheBreedStateList.Remove(treesByObjectTableOfTheBreedStateList);
            await _context.SaveChangesAsync();

            return treesByObjectTableOfTheBreedStateList;
        }

        private bool TreesByObjectTableOfTheBreedStateListExists(int id)
        {
            return _context.TreesByObjectTableOfTheBreedStateList.Any(e => e.Id == id);
        }

        // GET: api/TreesByObjectTableOfTheBreedStateLists/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<TreesByObjectTableOfTheBreedStateList>>> GetTreesByObjectTableOfTheBreedStateListCount(int? GreemPlantsPassportId,
            int? PlantationsTypeId)
        {
            var treesByObjectTableOfTheBreedStateLists = _context.TreesByObjectTableOfTheBreedStateList
                .Where(k => true);
            
            if (GreemPlantsPassportId != null)
            {
                treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.Where(m => m.GreemPlantsPassportId == GreemPlantsPassportId);
            }
            if (PlantationsTypeId != null)
            {
                treesByObjectTableOfTheBreedStateLists = treesByObjectTableOfTheBreedStateLists.Where(m => m.PlantationsTypeId == PlantationsTypeId);
            }

            int count = await treesByObjectTableOfTheBreedStateLists.CountAsync();

            return Ok(count);
        }
    }
}
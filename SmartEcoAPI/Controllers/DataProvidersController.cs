using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DataProvidersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataProvidersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DataProviders
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<DataProvider>>> GetDataProvider(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var dataProviders = _context.DataProvider
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                dataProviders = dataProviders.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    dataProviders = dataProviders.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    dataProviders = dataProviders.OrderByDescending(m => m.Name);
                    break;
                default:
                    dataProviders = dataProviders.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                dataProviders = dataProviders.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await dataProviders.ToListAsync();
        }

        // GET: api/DataProviders/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<DataProvider>> GetDataProvider(int id)
        {
            var dataProvider = await _context.DataProvider.FindAsync(id);

            if (dataProvider == null)
            {
                return NotFound();
            }

            return dataProvider;
        }

        // PUT: api/DataProviders/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutDataProvider(int id, DataProvider dataProvider)
        {
            if (id != dataProvider.Id)
            {
                return BadRequest();
            }

            _context.Entry(dataProvider).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataProviderExists(id))
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

        // POST: api/DataProviders
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<DataProvider>> PostDataProvider(DataProvider dataProvider)
        {
            _context.DataProvider.Add(dataProvider);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDataProvider", new { id = dataProvider.Id }, dataProvider);
        }

        // DELETE: api/DataProviders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<DataProvider>> DeleteDataProvider(int id)
        {
            var dataProvider = await _context.DataProvider.FindAsync(id);
            if (dataProvider == null)
            {
                return NotFound();
            }

            _context.DataProvider.Remove(dataProvider);
            await _context.SaveChangesAsync();

            return dataProvider;
        }

        private bool DataProviderExists(int id)
        {
            return _context.DataProvider.Any(e => e.Id == id);
        }

        // GET: api/DataProviders/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<DataProvider>>> GetDataProviderCount(string Name)
        {
            var dataProviders = _context.DataProvider
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                dataProviders = dataProviders.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await dataProviders.CountAsync();

            return Ok(count);
        }
    }
}

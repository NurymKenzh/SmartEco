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
    public class TerritoryTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerritoryTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TerritoryTypes
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<TerritoryType>>> GetTerritoryType(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? PageSize,
            int? PageNumber)
        {
            var territoryTypes = _context.TerritoryType
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                territoryTypes = territoryTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                territoryTypes = territoryTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                territoryTypes = territoryTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    territoryTypes = territoryTypes.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    territoryTypes = territoryTypes.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    territoryTypes = territoryTypes.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    territoryTypes = territoryTypes.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    territoryTypes = territoryTypes.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    territoryTypes = territoryTypes.OrderByDescending(m => m.NameEN);
                    break;
                default:
                    territoryTypes = territoryTypes.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                territoryTypes = territoryTypes.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await territoryTypes.ToListAsync();
        }

        // GET: api/TerritoryTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<TerritoryType>> GetTerritoryType(int id)
        {
            var territoryType = await _context.TerritoryType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (territoryType == null)
            {
                return NotFound();
            }

            return territoryType;
        }

        private bool TerritoryTypeExists(int id)
        {
            return _context.TerritoryType.Any(e => e.Id == id);
        }

        // GET: api/TerritoryTypes/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<TerritoryType>>> GetTerritoryTypeCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var territoryTypes = _context.TerritoryType
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                territoryTypes = territoryTypes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                territoryTypes = territoryTypes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                territoryTypes = territoryTypes.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await territoryTypes.CountAsync();

            return Ok(count);
        }
    }
}
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
    public class AuthorizedAuthoritiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorizedAuthoritiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<AuthorizedAuthority>>> GetAuthorizedAuthority(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var authorizedAuthorities = _context.AuthorizedAuthority
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                authorizedAuthorities = authorizedAuthorities.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    authorizedAuthorities = authorizedAuthorities.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    authorizedAuthorities = authorizedAuthorities.OrderByDescending(k => k.Name);
                    break;
                default:
                    authorizedAuthorities = authorizedAuthorities.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                authorizedAuthorities = authorizedAuthorities.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await authorizedAuthorities.ToListAsync();
        }

        // GET: api/AuthorizedAuthorities/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<AuthorizedAuthority>> GetAuthorizedAuthority(int id)
        {
            var authorizedAuthority = await _context.AuthorizedAuthority
                .FirstOrDefaultAsync(m => m.Id == id);

            if (authorizedAuthority == null)
            {
                return NotFound();
            }

            return authorizedAuthority;
        }

        // PUT: api/AuthorizedAuthorities/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<IActionResult> PutAuthorizedAuthority(int id, AuthorizedAuthority authorizedAuthority)
        {
            if (id != authorizedAuthority.Id)
            {
                return BadRequest();
            }

            _context.Entry(authorizedAuthority).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorizedAuthorityExists(id))
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

        // POST: api/AuthorizedAuthorities
        [HttpPost]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<AuthorizedAuthority>> PostAuthorizedAuthority(AuthorizedAuthority authorizedAuthority)
        {
            _context.AuthorizedAuthority.Add(authorizedAuthority);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthorizedAuthority", new { id = authorizedAuthority.Id }, authorizedAuthority);
        }

        // DELETE: api/AuthorizedAuthorities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Shymkent")]
        public async Task<ActionResult<AuthorizedAuthority>> DeleteAuthorizedAuthority(int id)
        {
            var authorizedAuthority = await _context.AuthorizedAuthority
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorizedAuthority == null)
            {
                return NotFound();
            }

            _context.AuthorizedAuthority.Remove(authorizedAuthority);
            await _context.SaveChangesAsync();

            return authorizedAuthority;
        }

        private bool AuthorizedAuthorityExists(int id)
        {
            return _context.AuthorizedAuthority.Any(e => e.Id == id);
        }

        // GET: api/AuthorizedAuthorities/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<AuthorizedAuthority>>> GetAuthorizedAuthorityCount(string Name,
            int? MonitoringPostId)
        {
            var authorizedAuthorities = _context.AuthorizedAuthority
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                authorizedAuthorities = authorizedAuthorities.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await authorizedAuthorities.CountAsync();

            return Ok(count);
        }
    }
}
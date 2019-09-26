using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class KazHydrometSoilPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KazHydrometSoilPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KazHydrometSoilPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KazHydrometSoilPost>>> GetKazHydrometSoilPost(string SortOrder,
            int? Number,
            int? PageSize,
            int? PageNumber)
        {
            var kazHydrometSoilPosts = _context.KazHydrometSoilPost
                .Where(k => true);

            if (Number != null)
            {
                kazHydrometSoilPosts = kazHydrometSoilPosts.Where(k => k.Number == Number);
            }

            switch (SortOrder)
            {
                case "Number":
                    kazHydrometSoilPosts = kazHydrometSoilPosts.OrderBy(k => k.Number);
                    break;
                case "NumberDesc":
                    kazHydrometSoilPosts = kazHydrometSoilPosts.OrderByDescending(k => k.Number);
                    break;
                default:
                    kazHydrometSoilPosts = kazHydrometSoilPosts.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                kazHydrometSoilPosts = kazHydrometSoilPosts.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await kazHydrometSoilPosts.ToListAsync();
        }

        // GET: api/KazHydrometSoilPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KazHydrometSoilPost>> GetKazHydrometSoilPost(int id)
        {
            var kazHydrometSoilPost = await _context.KazHydrometSoilPost.FindAsync(id);

            if (kazHydrometSoilPost == null)
            {
                return NotFound();
            }

            return kazHydrometSoilPost;
        }

        // PUT: api/KazHydrometSoilPosts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKazHydrometSoilPost(int id, KazHydrometSoilPost kazHydrometSoilPost)
        {
            if (id != kazHydrometSoilPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(kazHydrometSoilPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KazHydrometSoilPostExists(id))
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

        // POST: api/KazHydrometSoilPosts
        [HttpPost]
        public async Task<ActionResult<KazHydrometSoilPost>> PostKazHydrometSoilPost(KazHydrometSoilPost kazHydrometSoilPost)
        {
            _context.KazHydrometSoilPost.Add(kazHydrometSoilPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKazHydrometSoilPost", new { id = kazHydrometSoilPost.Id }, kazHydrometSoilPost);
        }

        // DELETE: api/KazHydrometSoilPosts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KazHydrometSoilPost>> DeleteKazHydrometSoilPost(int id)
        {
            var kazHydrometSoilPost = await _context.KazHydrometSoilPost.FindAsync(id);
            if (kazHydrometSoilPost == null)
            {
                return NotFound();
            }

            _context.KazHydrometSoilPost.Remove(kazHydrometSoilPost);
            await _context.SaveChangesAsync();

            return kazHydrometSoilPost;
        }

        private bool KazHydrometSoilPostExists(int id)
        {
            return _context.KazHydrometSoilPost.Any(e => e.Id == id);
        }

        // GET: api/KazHydrometSoilPosts/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<KazHydrometSoilPost>>> GetKazHydrometSoilPostCount(int? Number)
        {
            var kazHydrometSoilPosts = _context.KazHydrometSoilPost
                .Where(k => true);

            if (Number != null)
            {
                kazHydrometSoilPosts = kazHydrometSoilPosts.Where(k => k.Number == Number);
            }

            int count = await kazHydrometSoilPosts.CountAsync();

            return Ok(count);
        }
    }
}

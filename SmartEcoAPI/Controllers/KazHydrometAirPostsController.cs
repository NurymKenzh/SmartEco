using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KazHydrometAirPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KazHydrometAirPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KazHydrometAirPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KazHydrometAirPost>>> GetKazHydrometAirPost(string SortOrder,
            int? Number,
            int? PageSize,
            int? PageNumber)
        {
            var kazHydrometAirPosts = _context.KazHydrometAirPost
                .Where(k => true);

            if (Number != null)
            {
                kazHydrometAirPosts = kazHydrometAirPosts.Where(k => k.Number == Number);
            }

            switch (SortOrder)
            {
                case "Number":
                    kazHydrometAirPosts = kazHydrometAirPosts.OrderBy(k => k.Number);
                    break;
                case "NumberDesc":
                    kazHydrometAirPosts = kazHydrometAirPosts.OrderByDescending(k => k.Number);
                    break;
                default:
                    kazHydrometAirPosts = kazHydrometAirPosts.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                kazHydrometAirPosts = kazHydrometAirPosts.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await kazHydrometAirPosts.ToListAsync();
        }

        // GET: api/KazHydrometAirPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KazHydrometAirPost>> GetKazHydrometAirPost(int id)
        {
            var kazHydrometAirPost = await _context.KazHydrometAirPost.FindAsync(id);

            if (kazHydrometAirPost == null)
            {
                return NotFound();
            }

            return kazHydrometAirPost;
        }

        // PUT: api/KazHydrometAirPosts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKazHydrometAirPost(int id, KazHydrometAirPost kazHydrometAirPost)
        {
            if (id != kazHydrometAirPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(kazHydrometAirPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KazHydrometAirPostExists(id))
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

        // POST: api/KazHydrometAirPosts
        [HttpPost]
        public async Task<ActionResult<KazHydrometAirPost>> PostKazHydrometAirPost(KazHydrometAirPost kazHydrometAirPost)
        {
            _context.KazHydrometAirPost.Add(kazHydrometAirPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKazHydrometAirPost", new { id = kazHydrometAirPost.Id }, kazHydrometAirPost);
        }

        // DELETE: api/KazHydrometAirPosts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KazHydrometAirPost>> DeleteKazHydrometAirPost(int id)
        {
            var kazHydrometAirPost = await _context.KazHydrometAirPost.FindAsync(id);
            if (kazHydrometAirPost == null)
            {
                return NotFound();
            }

            _context.KazHydrometAirPost.Remove(kazHydrometAirPost);
            await _context.SaveChangesAsync();

            return kazHydrometAirPost;
        }

        private bool KazHydrometAirPostExists(int id)
        {
            return _context.KazHydrometAirPost.Any(e => e.Id == id);
        }

        // GET: api/KazHydrometAirPosts/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<KazHydrometAirPost>>> GetKazHydrometAirPostCount(int? Number)
        {
            var kazHydrometAirPosts = _context.KazHydrometAirPost
                .Where(k => true);

            if (Number != null)
            {
                kazHydrometAirPosts = kazHydrometAirPosts.Where(k => k.Number == Number);
            }

            int count = await kazHydrometAirPosts.CountAsync();

            return Ok(count);
        }
    }
}

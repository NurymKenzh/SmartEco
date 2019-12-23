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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject(string SortOrder,
            string Name,
            int? PageSize,
            int? PageNumber)
        {
            var projects = _context.Project
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                projects = projects.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    projects = projects.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    projects = projects.OrderByDescending(m => m.Name);
                    break;
                default:
                    projects = projects.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                projects = projects.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await projects.ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Project.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

        // GET: api/Projects/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectCount(string Name)
        {
            var projects = _context.Project
                .Where(m => true);

            if (!string.IsNullOrEmpty(Name))
            {
                projects = projects.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }

            int count = await projects.CountAsync();

            return Ok(count);
        }
    }
}
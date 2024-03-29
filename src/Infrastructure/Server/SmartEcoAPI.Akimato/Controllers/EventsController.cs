﻿using System;
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
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvent(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? ProjectId,
            int? PageSize,
            int? PageNumber)
        {
            var events = _context.Event
                .Include(e => e.Project)
                .Where(k => true);

            var role = _context.Person
                .Where(p => p.Email == HttpContext.User.Identity.Name)
                .FirstOrDefault().Role;
            if (role == "KaragandaRegion")
            {
                events = events.Where(e => e.Project.Id == 1);
            }
            if (role == "Almaty")
            {
                events = events.Where(e => e.Project.Id == 3);
            }
            if (role == "Shymkent")
            {
                events = events.Where(e => e.Project.Id == 4);
            }

            if (!string.IsNullOrEmpty(NameKK))
            {
                events = events.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                events = events.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                events = events.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (ProjectId != null)
            {
                events = events.Where(m => m.ProjectId == ProjectId);
            }

            switch (SortOrder)
            {
                case "NameKK":
                    events = events.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    events = events.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    events = events.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    events = events.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    events = events.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    events = events.OrderByDescending(m => m.NameEN);
                    break;
                case "Project":
                    events = events.OrderBy(m => m.Project);
                    break;
                case "ProjectDesc":
                    events = events.OrderByDescending(m => m.Project);
                    break;
                default:
                    events = events.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                events = events.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await events.ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventt = await _context.Event
                .Include(e => e.Project)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventt == null)
            {
                return NotFound();
            }

            return eventt;
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<IActionResult> PutEvent(int id, Event eventt)
        {
            if (id != eventt.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/Events
        [HttpPost]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<Event>> PostEvent(Event eventt)
        {
            _context.Event.Add(eventt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = eventt.Id }, eventt);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<Event>> DeleteEvent(int id)
        {
            var eventt = await _context.Event.FindAsync(id);
            if (eventt == null)
            {
                return NotFound();
            }

            _context.Event.Remove(eventt);
            await _context.SaveChangesAsync();

            return eventt;
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        // GET: api/Events/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventCount(string NameKK,
            string NameRU,
            string NameEN)
        {
            var events = _context.Event
                .Where(k => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                events = events.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                events = events.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                events = events.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            int count = await events.CountAsync();

            return Ok(count);
        }
    }
}
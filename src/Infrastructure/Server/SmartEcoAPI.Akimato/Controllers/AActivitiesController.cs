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
    public class AActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AActivities
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<AActivity>>> GetAActivity(string SortOrder,
            int? PollutionEnvironmentId,
            int? TargetId,
            int? MeasuredParameterUnitId,
            int? TerritoryTypeId,
            int? TargetTerritoryId,
            int? EventId,
            int? ProjectId,
            int? Year,
            bool? ActivityType,
            int? PageSize,
            int? PageNumber)
        {
            var aActivities = _context.AActivity
                .Include(a => a.Event)
                .Include(t => t.Target)
                .Include(t => t.TargetValue)
                .Include(t => t.Target.MeasuredParameterUnit)
                .Include(t => t.Target.PollutionEnvironment)
                .Include(t => t.TargetTerritory)
                .Include(t => t.TargetTerritory.TerritoryType)
                .Include(t => t.Project)
                .Where(k => true);

            var role = _context.Person
                .Where(p => p.Email == HttpContext.User.Identity.Name)
                .FirstOrDefault().Role;
            if (role == "KaragandaRegion")
            {
                aActivities = aActivities.Where(a => a.Project.Id == 1);
            }
            if (role == "Almaty")
            {
                aActivities = aActivities.Where(a => a.Project.Id == 3);
            }
            if (role == "Shymkent")
            {
                aActivities = aActivities.Where(a => a.Project.Id == 4);
            }

            if (PollutionEnvironmentId != null)
            {
                aActivities = aActivities.Where(t => t.Target.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (TargetId != null)
            {
                aActivities = aActivities.Where(t => t.TargetId == TargetId);
            }
            if (MeasuredParameterUnitId != null)
            {
                aActivities = aActivities.Where(t => t.Target.MeasuredParameterUnitId == MeasuredParameterUnitId);
            }
            if (TerritoryTypeId != null)
            {
                aActivities = aActivities.Where(t => t.TargetTerritory.TerritoryTypeId == TerritoryTypeId);
            }
            if (TargetTerritoryId != null)
            {
                aActivities = aActivities.Where(t => t.TargetTerritoryId == TargetTerritoryId);
            }
            if (EventId != null)
            {
                aActivities = aActivities.Where(t => t.EventId == EventId);
            }
            if (ProjectId != null)
            {
                aActivities = aActivities.Where(t => t.ProjectId == ProjectId);
            }
            if (Year != null)
            {
                aActivities = aActivities.Where(t => t.Year == Year);
            }
            if (ActivityType != null)
            {
                aActivities = aActivities.Where(t => t.ActivityType == ActivityType);
            }

            switch (SortOrder)
            {
                case "PollutionEnvironmentId":
                    aActivities = aActivities.OrderBy(t => t.Target.PollutionEnvironment);
                    break;
                case "PollutionEnvironmentIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Target.PollutionEnvironment);
                    break;
                case "TargetId":
                    aActivities = aActivities.OrderBy(t => t.Target);
                    break;
                case "TargetIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Target);
                    break;
                case "MeasuredParameterUnitId":
                    aActivities = aActivities.OrderBy(t => t.Target.MeasuredParameterUnit);
                    break;
                case "MeasuredParameterUnitIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Target.MeasuredParameterUnit);
                    break;
                case "TerritoryTypeId":
                    aActivities = aActivities.OrderBy(t => t.TargetTerritory.TerritoryType);
                    break;
                case "TerritoryTypeIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.TargetTerritory.TerritoryType);
                    break;
                case "TargetTerritoryId":
                    aActivities = aActivities.OrderBy(t => t.TargetTerritory);
                    break;
                case "TargetTerritoryIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.TargetTerritory);
                    break;
                case "EventId":
                    aActivities = aActivities.OrderBy(t => t.Event);
                    break;
                case "EventIdDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Event);
                    break;
                case "Project":
                    aActivities = aActivities.OrderBy(t => t.Project);
                    break;
                case "ProjectDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Project);
                    break;
                case "Year":
                    aActivities = aActivities.OrderBy(t => t.Year);
                    break;
                case "YearDesc":
                    aActivities = aActivities.OrderByDescending(t => t.Year);
                    break;
                case "AActivityType":
                    aActivities = aActivities.OrderBy(t => t.ActivityType);
                    break;
                case "AActivityTypeDesc":
                    aActivities = aActivities.OrderByDescending(t => t.ActivityType);
                    break;
                default:
                    aActivities = aActivities.OrderBy(t => t.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                aActivities = aActivities.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await aActivities.ToListAsync();
        }

        // GET: api/AActivities/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<AActivity>> GetAActivity(int id)
        {
            var aActivity = await _context.AActivity
                .Include(a => a.Event)
                .Include(t => t.Target)
                .Include(t => t.TargetValue)
                .Include(t => t.Target.MeasuredParameterUnit)
                .Include(t => t.Target.PollutionEnvironment)
                .Include(t => t.TargetTerritory)
                .Include(t => t.TargetTerritory.TerritoryType)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (aActivity == null)
            {
                return NotFound();
            }

            return aActivity;
        }

        // PUT: api/AActivities/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<IActionResult> PutAActivity(int id, AActivity aActivity)
        {
            if (id != aActivity.Id)
            {
                return BadRequest();
            }

            _context.Entry(aActivity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AActivityExists(id))
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

        // POST: api/AActivities
        [HttpPost]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<AActivity>> PostAActivity(AActivity aActivity)
        {
            _context.AActivity.Add(aActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAActivity", new { id = aActivity.Id }, aActivity);
        }

        // DELETE: api/AActivities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<AActivity>> DeleteAActivity(int id)
        {
            var aActivity = await _context.AActivity
                .Include(a => a.Event)
                .Include(t => t.Target)
                .Include(t => t.TargetValue)
                .Include(t => t.Target.MeasuredParameterUnit)
                .Include(t => t.Target.PollutionEnvironment)
                .Include(t => t.TargetTerritory)
                .Include(t => t.TargetTerritory.TerritoryType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aActivity == null)
            {
                return NotFound();
            }

            _context.AActivity.Remove(aActivity);
            await _context.SaveChangesAsync();

            return aActivity;
        }

        private bool AActivityExists(int id)
        {
            return _context.AActivity.Any(e => e.Id == id);
        }

        // GET: api/AActivities/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<AActivity>>> GetAActivityCount(int? PollutionEnvironmentId,
            int? TargetId,
            int? MeasuredParameterUnitId,
            int? TerritoryTypeId,
            int? TargetTerritoryId,
            int? EventId,
            int? Year,
            bool? ActivityType)
        {
            var aActivities = _context.AActivity
                .Where(k => true);

            if (PollutionEnvironmentId != null)
            {
                aActivities = aActivities.Where(t => t.Target.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (TargetId != null)
            {
                aActivities = aActivities.Where(t => t.TargetId == TargetId);
            }
            if (MeasuredParameterUnitId != null)
            {
                aActivities = aActivities.Where(t => t.Target.MeasuredParameterUnitId == MeasuredParameterUnitId);
            }
            if (TerritoryTypeId != null)
            {
                aActivities = aActivities.Where(t => t.TargetTerritory.TerritoryTypeId == TerritoryTypeId);
            }
            if (TargetTerritoryId != null)
            {
                aActivities = aActivities.Where(t => t.TargetTerritoryId == TargetTerritoryId);
            }
            if (Year != null)
            {
                aActivities = aActivities.Where(t => t.Year == Year);
            }
            if (ActivityType != null)
            {
                aActivities = aActivities.Where(t => t.ActivityType == ActivityType);
            }

            int count = await aActivities.CountAsync();

            return Ok(count);
        }

        // POST: api/AActivities/SetTargetValueId
        [HttpPost("SetTargetValueId")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task SetTargetValueId(
            int TargetValueId,
            [FromQuery(Name = "Ids")] List<int> Ids,
            [FromQuery(Name = "Efficiencies")] List<decimal> Efficiencies)
        {
            var aActivities = _context.AActivity
                .Where(a => a.TargetValueId == TargetValueId)
                .ToList();
            foreach (var aActivity in aActivities)
            {
                if (!Ids.Contains(aActivity.Id))
                {
                    aActivity.TargetValueId = null;
                }
            }

            for (int i = 0; i < Ids.Count; i++)
            {
                var aActivity = _context.AActivity
                    .Where(a => a.Id == Ids[i])
                    .FirstOrDefault();

                aActivity.TargetValueId = TargetValueId;
                aActivity.Efficiency = Efficiencies[i];
            }

            await _context.SaveChangesAsync();
        }
    }
}
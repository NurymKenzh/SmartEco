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
    public class TargetValuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TargetValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TargetValues
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<TargetValue>>> GetTargetValue(string SortOrder,
            int? PollutionEnvironmentId,
            int? TargetId,
            int? MeasuredParameterUnitId,
            int? TerritoryTypeId,
            int? TargetTerritoryId,
            int? ProjectId,
            int? Year,
            bool? TargetValueType,
            int? PageSize,
            int? PageNumber)
        {
            var targetValues = _context.TargetValue
                .Include(t => t.Target)
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
                targetValues = targetValues.Where(t => t.Project.Id == 1);
            }
            if (role == "Almaty")
            {
                targetValues = targetValues.Where(t => t.Project.Id == 3);
            }
            if (role == "Shymkent")
            {
                targetValues = targetValues.Where(t => t.Project.Id == 4);
            }

            if (PollutionEnvironmentId != null)
            {
                targetValues = targetValues.Where(t => t.Target.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (TargetId != null)
            {
                targetValues = targetValues.Where(t => t.TargetId == TargetId);
            }
            if (MeasuredParameterUnitId != null)
            {
                targetValues = targetValues.Where(t => t.Target.MeasuredParameterUnitId == MeasuredParameterUnitId);
            }
            if (TerritoryTypeId != null)
            {
                targetValues = targetValues.Where(t => t.TargetTerritory.TerritoryTypeId == TerritoryTypeId);
            }
            if (TargetTerritoryId != null)
            {
                targetValues = targetValues.Where(t => t.TargetTerritoryId == TargetTerritoryId);
            }
            if (ProjectId != null)
            {
                targetValues = targetValues.Where(t => t.ProjectId == ProjectId);
            }
            if (Year != null)
            {
                targetValues = targetValues.Where(t => t.Year == Year);
            }
            if (TargetValueType != null)
            {
                targetValues = targetValues.Where(t => t.TargetValueType == TargetValueType);
            }

            switch (SortOrder)
            {
                case "PollutionEnvironmentId":
                    targetValues = targetValues.OrderBy(t => t.Target.PollutionEnvironment);
                    break;
                case "PollutionEnvironmentIdDesc":
                    targetValues = targetValues.OrderByDescending(t => t.Target.PollutionEnvironment);
                    break;
                case "TargetId":
                    targetValues = targetValues.OrderBy(t => t.Target);
                    break;
                case "TargetIdDesc":
                    targetValues = targetValues.OrderByDescending(t => t.Target);
                    break;
                case "MeasuredParameterUnitId":
                    targetValues = targetValues.OrderBy(t => t.Target.MeasuredParameterUnit);
                    break;
                case "MeasuredParameterUnitIdDesc":
                    targetValues = targetValues.OrderByDescending(t => t.Target.MeasuredParameterUnit);
                    break;
                case "TerritoryTypeId":
                    targetValues = targetValues.OrderBy(t => t.TargetTerritory.TerritoryType);
                    break;
                case "TerritoryTypeIdDesc":
                    targetValues = targetValues.OrderByDescending(t => t.TargetTerritory.TerritoryType);
                    break;
                case "TargetTerritoryId":
                    targetValues = targetValues.OrderBy(t => t.TargetTerritory);
                    break;
                case "TargetTerritoryIdDesc":
                    targetValues = targetValues.OrderByDescending(t => t.TargetTerritory);
                    break;
                case "Project":
                    targetValues = targetValues.OrderBy(t => t.Project);
                    break;
                case "ProjectDesc":
                    targetValues = targetValues.OrderByDescending(t => t.Project);
                    break;
                case "Year":
                    targetValues = targetValues.OrderBy(t => t.Year);
                    break;
                case "YearDesc":
                    targetValues = targetValues.OrderByDescending(t => t.Year);
                    break;
                case "TargetValueType":
                    targetValues = targetValues.OrderBy(t => t.TargetValueType);
                    break;
                case "TargetValueTypeDesc":
                    targetValues = targetValues.OrderByDescending(t => t.TargetValueType);
                    break;
                default:
                    targetValues = targetValues.OrderBy(t => t.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                targetValues = targetValues.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await targetValues.ToListAsync();
        }

        // GET: api/TargetValues/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<TargetValue>> GetTargetValue(int id)
        {
            var targetValue = await _context.TargetValue
                .Include(t => t.Target)
                .Include(t => t.Target.MeasuredParameterUnit)
                .Include(t => t.Target.PollutionEnvironment)
                .Include(t => t.TargetTerritory)
                .Include(t => t.TargetTerritory.TerritoryType)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (targetValue == null)
            {
                return NotFound();
            }

            return targetValue;
        }

        // PUT: api/TargetValues/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<IActionResult> PutTargetValue(int id, TargetValue targetValue)
        {
            if (id != targetValue.Id)
            {
                return BadRequest();
            }

            _context.Entry(targetValue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetValueExists(id))
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

        // POST: api/TargetValues
        [HttpPost]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<TargetValue>> PostTargetValue(TargetValue targetValue)
        {
            _context.TargetValue.Add(targetValue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTargetValue", new { id = targetValue.Id }, targetValue);
        }

        // DELETE: api/TargetValues/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<TargetValue>> DeleteTargetValue(int id)
        {
            var targetValue = await _context.TargetValue
                .Include(t => t.Target)
                .Include(t => t.Target.MeasuredParameterUnit)
                .Include(t => t.Target.PollutionEnvironment)
                .Include(t => t.TargetTerritory)
                .Include(t => t.TargetTerritory.TerritoryType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (targetValue == null)
            {
                return NotFound();
            }

            _context.TargetValue.Remove(targetValue);
            await _context.SaveChangesAsync();

            return targetValue;
        }

        private bool TargetValueExists(int id)
        {
            return _context.TargetValue.Any(e => e.Id == id);
        }

        // GET: api/TargetValues/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<TargetValue>>> GetTargetValueCount(int? PollutionEnvironmentId,
            int? TargetId,
            int? MeasuredParameterUnitId,
            int? TerritoryTypeId,
            int? TargetTerritoryId,
            int? Year,
            bool? TargetValueType)
        {
            var targetValues = _context.TargetValue
                .Where(k => true);

            if (PollutionEnvironmentId != null)
            {
                targetValues = targetValues.Where(t => t.Target.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (TargetId != null)
            {
                targetValues = targetValues.Where(t => t.TargetId == TargetId);
            }
            if (MeasuredParameterUnitId != null)
            {
                targetValues = targetValues.Where(t => t.Target.MeasuredParameterUnitId == MeasuredParameterUnitId);
            }
            if (TerritoryTypeId != null)
            {
                targetValues = targetValues.Where(t => t.TargetTerritory.TerritoryTypeId == TerritoryTypeId);
            }
            if (TargetTerritoryId != null)
            {
                targetValues = targetValues.Where(t => t.TargetTerritoryId == TargetTerritoryId);
            }
            if (Year != null)
            {
                targetValues = targetValues.Where(t => t.Year == Year);
            }
            if (TargetValueType != null)
            {
                targetValues = targetValues.Where(t => t.TargetValueType == TargetValueType);
            }

            int count = await targetValues.CountAsync();

            return Ok(count);
        }
    }
}
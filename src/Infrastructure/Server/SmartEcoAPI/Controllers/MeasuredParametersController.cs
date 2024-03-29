﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MeasuredParametersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredParameters
        [HttpGet]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Kazhydromet")]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameter(string SortOrder,
            string NameKK,
            string NameRU,
            string NameEN,
            int? EcomonCode,
            string OceanusCode,
            string KazhydrometCode,
            int? PageSize,
            int? PageNumber)
        {
            var measuredParameters = _context.MeasuredParameter
                .Include(m => m.MeasuredParameterUnit)
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameters = measuredParameters.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameters = measuredParameters.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameters = measuredParameters.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (EcomonCode != null)
            {
                measuredParameters = measuredParameters.Where(m => m.EcomonCode == EcomonCode);
            }
            if (!string.IsNullOrEmpty(OceanusCode))
            {
                measuredParameters = measuredParameters.Where(m => m.OceanusCode.ToLower().Contains(OceanusCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(KazhydrometCode))
            {
                measuredParameters = measuredParameters.Where(m => m.KazhydrometCode.ToLower().Contains(KazhydrometCode.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameKK":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameRU);
                    break;
                case "NameEN":
                    measuredParameters = measuredParameters.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.NameEN);
                    break;
                case "EcomonCode":
                    measuredParameters = measuredParameters.OrderBy(m => m.EcomonCode);
                    break;
                case "EcomonCodeDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.EcomonCode);
                    break;
                case "OceanusCode":
                    measuredParameters = measuredParameters.OrderBy(m => m.OceanusCode);
                    break;
                case "OceanusCodeDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.OceanusCode);
                    break;
                case "KazhydrometCode":
                    measuredParameters = measuredParameters.OrderBy(m => m.KazhydrometCode);
                    break;
                case "KazhydrometCodeDesc":
                    measuredParameters = measuredParameters.OrderByDescending(m => m.KazhydrometCode);
                    break;
                default:
                    measuredParameters = measuredParameters.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                measuredParameters = measuredParameters.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await measuredParameters.ToListAsync();
        }

        // GET: api/MeasuredParameters/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<MeasuredParameter>> GetMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter
                .Include(m => m.MeasuredParameterUnit)
                //.FindAsync(id);
                .FirstOrDefaultAsync(m => m.Id == id);

            if (measuredParameter == null)
            {
                return NotFound();
            }

            return measuredParameter;
        }

        // PUT: api/MeasuredParameters/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutMeasuredParameter(int id, MeasuredParameter measuredParameter)
        {
            if (id != measuredParameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredParameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredParameterExists(id))
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

        // POST: api/MeasuredParameters
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MeasuredParameter>> PostMeasuredParameter(MeasuredParameter measuredParameter)
        {
            _context.MeasuredParameter.Add(measuredParameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredParameter", new { id = measuredParameter.Id }, measuredParameter);
        }

        // DELETE: api/MeasuredParameters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MeasuredParameter>> DeleteMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter.FindAsync(id);
            if (measuredParameter == null)
            {
                return NotFound();
            }

            _context.MeasuredParameter.Remove(measuredParameter);
            await _context.SaveChangesAsync();

            return measuredParameter;
        }

        private bool MeasuredParameterExists(int id)
        {
            return _context.MeasuredParameter.Any(e => e.Id == id);
        }

        // GET: api/MeasuredParameters/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameterCount(string NameKK,
            string NameRU,
            string NameEN,
            int? EcomonCode,
            string OceanusCode,
            string KazhydrometCode)
        {
            var measuredParameters = _context.MeasuredParameter
                .Where(m => true);

            if (!string.IsNullOrEmpty(NameKK))
            {
                measuredParameters = measuredParameters.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                measuredParameters = measuredParameters.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                measuredParameters = measuredParameters.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (EcomonCode != null)
            {
                measuredParameters = measuredParameters.Where(m => m.EcomonCode == EcomonCode);
            }
            if (!string.IsNullOrEmpty(OceanusCode))
            {
                measuredParameters = measuredParameters.Where(m => m.OceanusCode.ToLower().Contains(OceanusCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(KazhydrometCode))
            {
                measuredParameters = measuredParameters.Where(m => m.KazhydrometCode.ToLower().Contains(KazhydrometCode.ToLower()));
            }

            int count = await measuredParameters.CountAsync();

            return Ok(count);
        }

        [HttpPost("SetMaximumValue")]
        public async Task<IActionResult> SetMaximumValue()
        {
            var measuredParameters = _context.MeasuredParameter.Where(m => m.MPCMaxSingle != null).ToList();
            foreach (var measuredParameter in measuredParameters)
            {
                var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == measuredParameter.Id && m.Max == null).ToList();
                foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
                {
                    monitoringPostMeasuredParameter.Max = measuredParameter.MPCMaxSingle;
                    _context.Entry(monitoringPostMeasuredParameter).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("GetSameMeasuredParameters")]
        public async Task<List<MeasuredParameter>> GetSameMeasuredParameters()
        {
            var mpmp = await _context.MonitoringPostMeasuredParameters
                    .Include(m => m.MonitoringPost.Project)
                    .Include(m => m.MeasuredParameter)
                    .Where(m => m.MonitoringPost.Project.Name.Contains("Zhanatas"))
                    .ToListAsync();

            var measuredParameters = mpmp
                .GroupBy(m => m.MeasuredParameterId)
                .Select(m => m.First().MeasuredParameter)
                .OrderBy(m => m.NameRU)
                .ToList();

            return measuredParameters;
        }
    }
}

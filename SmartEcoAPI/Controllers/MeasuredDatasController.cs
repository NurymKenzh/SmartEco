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
    public class MeasuredDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetMeasuredData(string SortOrder,
            string Language,
            int? MeasuredParameterId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            int? MonitoringPostId,
            int? PollutionSourceId,
            int? PageSize,
            int? PageNumber)
        {
            var measuredDatas = _context.MeasuredData
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .Include(m => m.PollutionSource)
                .Where(m => true);

            if (MeasuredParameterId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MeasuredParameterId == MeasuredParameterId);
            }
            if (DateTimeFrom != null)
            {
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime >= DateTimeFrom) ||
                    (m.Year != null && m.Month == null && m.Year >= DateTimeFrom.Value.Year) ||
                    (m.Year != null && m.Month != null && m.Year >= DateTimeFrom.Value.Year && m.Month >= DateTimeFrom.Value.Month));
            }
            if (DateTimeTo != null)
            {
                //measuredDatas = measuredDatas.Where(m => m.DateTime <= DateTimeTo);
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime <= DateTimeTo) ||
                    (m.Year != null && m.Month == null && m.Year <= DateTimeTo.Value.Year) ||
                    (m.Year != null && m.Month != null &&  m.Year<= DateTimeTo.Value.Year && m.Month <= DateTimeTo.Value.Month));
            }
            if (MonitoringPostId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MonitoringPostId == MonitoringPostId);
            }
            if (PollutionSourceId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.PollutionSourceId == PollutionSourceId);
            }

            switch (SortOrder)
            {
                case "MeasuredParameter":
                    if (Language == "kk")
                    {
                        measuredDatas = measuredDatas.OrderBy(m => m.MeasuredParameter.NameKK);
                    }
                    else if (Language == "en")
                    {
                        measuredDatas = measuredDatas.OrderBy(m => m.MeasuredParameter.NameEN);
                    }
                    else
                    {
                        measuredDatas = measuredDatas.OrderBy(m => m.MeasuredParameter.NameRU);
                    }
                    break;
                case "MeasuredParameterDesc":
                    if (Language == "kk")
                    {
                        measuredDatas = measuredDatas.OrderByDescending(m => m.MeasuredParameter.NameKK);
                    }
                    else if (Language == "en")
                    {
                        measuredDatas = measuredDatas.OrderByDescending(m => m.MeasuredParameter.NameEN);
                    }
                    else
                    {
                        measuredDatas = measuredDatas.OrderByDescending(m => m.MeasuredParameter.NameRU);
                    }
                    break;
                case "DateTime":
                    measuredDatas = measuredDatas.OrderBy(m => m.DateTime != null ?
                        m.DateTime :
                        (m.Year != null && m.Month == null ? new DateTime((int)m.Year, 1, 1) : new DateTime((int)m.Year, (int)m.Month, 1, 0, 0, 1)));
                    break;
                case "DateTimeDesc":
                    //measuredDatas = measuredDatas.OrderByDescending(m => m.DateTime);
                    measuredDatas = measuredDatas.OrderByDescending(m => m.DateTime != null ?
                        m.DateTime :
                        (m.Year != null && m.Month == null ? new DateTime((int)m.Year, 1, 1) : new DateTime((int)m.Year, (int)m.Month, 1, 0, 0, 1)));
                    break;
                case "MonitoringPost":
                    measuredDatas = measuredDatas.OrderBy(k => k.MonitoringPost);
                    break;
                case "MonitoringPostDesc":
                    measuredDatas = measuredDatas.OrderByDescending(k => k.MonitoringPost);
                    break;
                case "PollutionSource":
                    measuredDatas = measuredDatas.OrderBy(k => k.PollutionSource);
                    break;
                case "PollutionSourceDesc":
                    measuredDatas = measuredDatas.OrderByDescending(k => k.PollutionSource);
                    break;
                default:
                    measuredDatas = measuredDatas.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                measuredDatas = measuredDatas.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await measuredDatas.ToListAsync();
        }

        // GET: api/MeasuredDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeasuredData>> GetMeasuredData(long id)
        {
            //var measuredData = await _context.MeasuredData.FindAsync(id);
            var measuredData = await _context.MeasuredData
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .Include(m => m.PollutionSource)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (measuredData == null)
            {
                return NotFound();
            }

            return measuredData;
        }

        // PUT: api/MeasuredDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasuredData(int id, MeasuredData measuredData)
        {
            if (id != measuredData.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredDataExists(id))
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

        // POST: api/MeasuredDatas
        [HttpPost]
        public async Task<ActionResult<MeasuredData>> PostMeasuredData(MeasuredData measuredData)
        {
            _context.MeasuredData.Add(measuredData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredData", new { id = measuredData.Id }, measuredData);
        }

        // DELETE: api/MeasuredDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MeasuredData>> DeleteMeasuredData(int id)
        {
            //var measuredData = await _context.MeasuredData.FindAsync(id);
            var measuredData = await _context.MeasuredData
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .Include(m => m.PollutionSource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measuredData == null)
            {
                return NotFound();
            }

            _context.MeasuredData.Remove(measuredData);
            await _context.SaveChangesAsync();

            return measuredData;
        }

        private bool MeasuredDataExists(int id)
        {
            return _context.MeasuredData.Any(e => e.Id == id);
        }

        // GET: api/MeasuredDatas/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetMeasuredDatasCount(int? MeasuredParameterId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            int? MonitoringPostId,
            int? PollutionSourceId)
        {
            var measuredDatas = _context.MeasuredData
                 .Where(m => true);

            if (MeasuredParameterId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MeasuredParameterId == MeasuredParameterId);
            }
            if (DateTimeFrom != null)
            {
                //measuredDatas = measuredDatas.Where(m => m.DateTime >= DateTimeFrom);
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime >= DateTimeFrom) ||
                     (m.Year != null && m.Month == null && m.Year >= DateTimeFrom.Value.Year) ||
                     (m.Year != null && m.Month != null && m.Year >= DateTimeFrom.Value.Year && m.Month >= DateTimeFrom.Value.Month));
            }
            if (DateTimeTo != null)
            {
                //measuredDatas = measuredDatas.Where(m => m.DateTime <= DateTimeFrom);
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime <= DateTimeTo) ||
                    (m.Year != null && m.Month == null && m.Year <= DateTimeTo.Value.Year) ||
                    (m.Year != null && m.Month != null && m.Year <= DateTimeTo.Value.Year && m.Month <= DateTimeTo.Value.Month));
            }
            if (MonitoringPostId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MonitoringPostId == MonitoringPostId);
            }
            if (PollutionSourceId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.PollutionSourceId == PollutionSourceId);
            }

            int count = await measuredDatas.CountAsync();

            return Ok(count);
        }
    }
}

using System;
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
    public class MeasuredDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeasuredDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredDatas
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
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
            PopulateEcoserviceData();

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
                    //measuredDatas = measuredDatas.OrderBy(m => m.DateTime != null ?
                    //    m.DateTime :
                    //    (m.Year != null && m.Month == null ? new DateTime((int)m.Year, 1, 1) : new DateTime((int)m.Year, (int)m.Month, 1, 0, 0, 1)));

                    //measuredDatas = measuredDatas.OrderBy(m => m.DateTime);

                    measuredDatas = measuredDatas.OrderBy(m => m.DateYear)
                        .ThenBy(m => m.DateMonth)
                        .ThenBy(m => m.DateDay)
                        .ThenBy(m => m.DateHour)
                        .ThenBy(m => m.DateMinute)
                        .ThenBy(m => m.DateSecond);
                    break;
                case "DateTimeDesc":
                    //measuredDatas = measuredDatas.OrderByDescending(m => m.DateTime);
                    //measuredDatas = measuredDatas.OrderByDescending(m => m.DateTime != null ?
                    //    m.DateTime :
                    //    (m.Year != null && m.Month == null ? new DateTime((int)m.Year, 1, 1) : new DateTime((int)m.Year, (int)m.Month, 1, 0, 0, 1)));

                    //measuredDatas = measuredDatas.OrderByDescending(m => m.DateTime);

                    measuredDatas = measuredDatas.OrderByDescending(m => m.DateYear)
                        .ThenByDescending(m => m.DateMonth)
                        .ThenByDescending(m => m.DateDay)
                        .ThenByDescending(m => m.DateHour)
                        .ThenByDescending(m => m.DateMinute)
                        .ThenByDescending(m => m.DateSecond);
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
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
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
        [Authorize(Roles = "admin,moderator")]
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
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MeasuredData>> PostMeasuredData(MeasuredData measuredData)
        {
            _context.MeasuredData.Add(measuredData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredData", new { id = measuredData.Id }, measuredData);
        }

        // DELETE: api/MeasuredDatas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
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
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
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

        public void PopulateEcoserviceData()
        {
            Random rnd = new Random();
            DateTime start = (DateTime) _context.MeasuredData
                .Where(m => m.MonitoringPostId == 39)
                .Max(m => m.DateTime);

            decimal? previousValue = _context.MeasuredData
                .Where(m => m.MonitoringPostId == 39 && m.MeasuredParameterId == 2)
                .OrderByDescending(m => m.DateTime)
                .FirstOrDefault()
                .Value;
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            for (DateTime dateTime = start; dateTime < DateTime.Now; dateTime = dateTime.AddMinutes(1))
            {
                if (measuredDatas.Count > 0)
                {
                    previousValue = measuredDatas.Last().Value;
                }
                decimal? newValue = -1;
                while (newValue < 0 || newValue > 400)
                {
                    newValue = previousValue + rnd.Next(-20, 20);
                }
                MeasuredData measuredData = new MeasuredData()
                {
                    DateTime = dateTime,
                    MeasuredParameterId = 2,
                    MonitoringPostId = 39,
                    Value = newValue
                };
                measuredDatas.Add(measuredData);
            }
            _context.MeasuredData.AddRange(measuredDatas);
            _context.SaveChanges();

            previousValue = _context.MeasuredData
                .Where(m => m.MonitoringPostId == 40 && m.MeasuredParameterId == 2)
                .OrderByDescending(m => m.DateTime)
                .FirstOrDefault()
                .Value;
            measuredDatas = new List<MeasuredData>();
            for (DateTime dateTime = start; dateTime < DateTime.Now; dateTime = dateTime.AddMinutes(1))
            {
                if (measuredDatas.Count > 0)
                {
                    previousValue = measuredDatas.Last().Value;
                }
                decimal? newValue = -1;
                while (newValue < 0 || newValue > 400)
                {
                    newValue = previousValue + rnd.Next(-20, 20);
                }
                MeasuredData measuredData = new MeasuredData()
                {
                    DateTime = dateTime,
                    MeasuredParameterId = 2,
                    MonitoringPostId = 40,
                    Value = newValue
                };
                measuredDatas.Add(measuredData);
            }
            _context.MeasuredData.AddRange(measuredDatas);
            _context.SaveChanges();

            previousValue = _context.MeasuredData
                .Where(m => m.MonitoringPostId == 39 && m.MeasuredParameterId == 3)
                .OrderByDescending(m => m.DateTime)
                .FirstOrDefault()
                .Value;
            measuredDatas = new List<MeasuredData>();
            for (DateTime dateTime = start; dateTime < DateTime.Now; dateTime = dateTime.AddMinutes(1))
            {
                if (measuredDatas.Count > 0)
                {
                    previousValue = measuredDatas.Last().Value;
                }
                decimal? newValue = -1;
                while (newValue < 0 || newValue > 200)
                {
                    newValue = previousValue + rnd.Next(-12, 12);
                }
                MeasuredData measuredData = new MeasuredData()
                {
                    DateTime = dateTime,
                    MeasuredParameterId = 3,
                    MonitoringPostId = 39,
                    Value = newValue
                };
                measuredDatas.Add(measuredData);
            }
            _context.MeasuredData.AddRange(measuredDatas);
            _context.SaveChanges();

            previousValue = _context.MeasuredData
                .Where(m => m.MonitoringPostId == 40 && m.MeasuredParameterId == 3)
                .OrderByDescending(m => m.DateTime)
                .FirstOrDefault()
                .Value;
            measuredDatas = new List<MeasuredData>();
            for (DateTime dateTime = start; dateTime < DateTime.Now; dateTime = dateTime.AddMinutes(1))
            {
                if (measuredDatas.Count > 0)
                {
                    previousValue = measuredDatas.Last().Value;
                }
                decimal? newValue = -1;
                while (newValue < 0 || newValue > 200)
                {
                    newValue = previousValue + rnd.Next(-12, 12);
                }
                MeasuredData measuredData = new MeasuredData()
                {
                    DateTime = dateTime,
                    MeasuredParameterId = 3,
                    MonitoringPostId = 40,
                    Value = newValue
                };
                measuredDatas.Add(measuredData);
            }
            _context.MeasuredData.AddRange(measuredDatas);
            _context.SaveChanges();
        }
    }
}

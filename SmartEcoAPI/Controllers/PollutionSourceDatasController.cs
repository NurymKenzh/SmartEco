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
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PollutionSourceDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutionSourceDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PollutionSourceDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PollutionSourceData>>> GetPollutionSourceData(string SortOrder,
            string Language,
            int? PollutantId,
            int? PollutionSourceId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            int? PageSize,
            int? PageNumber)
        {
            GenerateRandomData();

            var pollutionSourceDatas = _context.PollutionSourceData
                .Include(p => p.Pollutant)
                .Include(p => p.PollutionSource)
                .Where(p => true);

            if (PollutantId != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.PollutantId == PollutantId);
            }
            if (PollutionSourceId != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.PollutionSourceId == PollutionSourceId);
            }
            if (DateTimeFrom != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.DateTime >= DateTimeFrom);
            }
            if (DateTimeTo != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.DateTime <= DateTimeTo);
            }

            switch (SortOrder)
            {
                case "Pollutant":
                    if (Language == "kk")
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.Pollutant.NameKK);
                    }
                    else if (Language == "en")
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.Pollutant.NameEN);
                    }
                    else
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.Pollutant.NameRU);
                    }
                    break;
                case "PollutantDesc":
                    if (Language == "kk")
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderByDescending(p => p.Pollutant.NameKK);
                    }
                    else if (Language == "en")
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderByDescending(p => p.Pollutant.NameEN);
                    }
                    else
                    {
                        pollutionSourceDatas = pollutionSourceDatas.OrderByDescending(p => p.Pollutant.NameRU);
                    }
                    break;
                case "PollutionSource":
                    pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.PollutionSource.Name);
                    break;
                case "PollutionSourceDesc":
                    pollutionSourceDatas = pollutionSourceDatas.OrderByDescending(p => p.PollutionSource.Name);
                    break;
                case "DateTime":
                    pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.DateTime);
                    break;
                case "DateTimeDesc":
                    pollutionSourceDatas = pollutionSourceDatas.OrderByDescending(p => p.DateTime);
                    break;
                default:
                    pollutionSourceDatas = pollutionSourceDatas.OrderBy(p => p.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await pollutionSourceDatas.ToListAsync();
        }

        // GET: api/PollutionSourceDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PollutionSourceData>> GetPollutionSourceData(long id)
        {
            //var pollutionSourceData = await _context.PollutionSourceData.FindAsync(id);
            var pollutionSourceData = await _context.PollutionSourceData
                .Include(p => p.Pollutant)
                .Include(p => p.PollutionSource)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pollutionSourceData == null)
            {
                return NotFound();
            }

            return pollutionSourceData;
        }

        // PUT: api/PollutionSourceDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPollutionSourceData(long id, PollutionSourceData pollutionSourceData)
        {
            if (id != pollutionSourceData.Id)
            {
                return BadRequest();
            }

            _context.Entry(pollutionSourceData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollutionSourceDataExists(id))
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

        // POST: api/PollutionSourceDatas
        [HttpPost]
        public async Task<ActionResult<PollutionSourceData>> PostPollutionSourceData(PollutionSourceData pollutionSourceData)
        {
            _context.PollutionSourceData.Add(pollutionSourceData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutionSourceData", new { id = pollutionSourceData.Id }, pollutionSourceData);
        }

        // DELETE: api/PollutionSourceDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PollutionSourceData>> DeletePollutionSourceData(long id)
        {
            //var pollutionSourceData = await _context.PollutionSourceData.FindAsync(id);
            var pollutionSourceData = await _context.PollutionSourceData
                .Include(p => p.Pollutant)
                .Include(p => p.PollutionSource)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pollutionSourceData == null)
            {
                return NotFound();
            }

            _context.PollutionSourceData.Remove(pollutionSourceData);
            await _context.SaveChangesAsync();

            return pollutionSourceData;
        }

        private bool PollutionSourceDataExists(long id)
        {
            return _context.PollutionSourceData.Any(e => e.Id == id);
        }

        // GET: api/PollutionSourceData/Count
        [HttpGet("count")]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetPollutionSourceDatasCount(int? PollutantId,
            int? PollutionSourceId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo)
        {
            var pollutionSourceDatas = _context.PollutionSourceData
                .Where(p => true);

            if (PollutantId != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.PollutantId == PollutantId);
            }
            if (PollutionSourceId != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.PollutionSourceId == PollutionSourceId);
            }
            if (DateTimeFrom != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.DateTime >= DateTimeFrom);
            }
            if (DateTimeTo != null)
            {
                pollutionSourceDatas = pollutionSourceDatas.Where(p => p.DateTime <= DateTimeTo);
            }

            int count = await pollutionSourceDatas.CountAsync();

            return Ok(count);
        }

        private void GenerateRandomData()
        {
            Random random = new Random();
            foreach (PollutionSource pollutionSource in _context.PollutionSource)
            {
                foreach (Pollutant pollutant in _context.Pollutant)
                {
                    List<PollutionSourceData> pollutionSourceDatas = _context.PollutionSourceData
                        .Where(p => p.PollutionSourceId == pollutionSource.Id && p.PollutantId == pollutant.Id)
                        .ToList();
                    for (DateTime dateTime = new DateTime(2019, 1, 1); dateTime <= DateTime.Now; dateTime = dateTime.AddMinutes(15))
                    {
                        if (pollutionSourceDatas.Count(p => p.DateTime == dateTime) == 0)
                        {
                            PollutionSourceData pollutionSourceData = new PollutionSourceData()
                            {
                                PollutionSourceId = pollutionSource.Id,
                                PollutantId = pollutant.Id,
                                DateTime = dateTime,
                                Value = random.Next(100, 1000)
                            };
                            _context.PollutionSourceData.Add(pollutionSourceData);
                        }
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}

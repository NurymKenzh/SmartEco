using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Akimato.Data;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog(string SortOrder,
            int? Number,
            int? PageSize,
            int? PageNumber)
        {
            var logs = _context.Log
                .Where(e => true);

            if (Number != null)
            {
                logs = logs.Where(e => e.Number == Number);
            }

            switch (SortOrder)
            {
                case "Number":
                    logs = logs.OrderBy(e => e.Number);
                    break;
                case "NumberDesc":
                    logs = logs.OrderByDescending(e => e.Number);
                    break;
                default:
                    logs = logs.OrderBy(e => e.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                logs = logs.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await logs.ToListAsync();
        }

        // GET: api/Logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLog(int id)
        {
            var log = await _context.Log.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Log>> DeleteLog(int id)
        {
            var log = await _context.Log.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.Log.Remove(log);
            await _context.SaveChangesAsync();

            return log;
        }

        //private bool LogExists(int id)
        //{
        //    return _context.Log.Any(e => e.Id == id);
        //}

        [HttpPost("AddNote")]
        public void AddNote(int Number,
            decimal NorthLatitude,
            decimal EastLongitude,
            DateTime DateTimeStart)
        {
            DateTime DateTimeEnd = DateTime.MinValue;
            Log(Number, NorthLatitude, EastLongitude, DateTimeStart, DateTimeEnd);
        }

        [HttpPost("EditNote")]
        public void EditNote(int Number,
            decimal NorthLatitude,
            decimal EastLongitude,
            DateTime DateTimeStart)
        {
            try
            {
                _context.Log.Last(m => m.Number == Number).DateTimeEnd = DateTimeStart;
            }
            catch
            {

            }

            DateTime DateTimeEnd = DateTime.MinValue;
            Log(Number, NorthLatitude, EastLongitude, DateTimeStart, DateTimeEnd);
        }

        public void Log(int Number,
            decimal NorthLatitude,
            decimal EastLongitude,
            DateTime DateTimeStart,
            DateTime DateTimeEnd)
        {
            Log log = new Log
            {
                Number = Number,
                NorthLatitude = NorthLatitude,
                EastLongitude = EastLongitude,
                DateTimeStart = DateTimeStart,
                DateTimeEnd = DateTimeEnd
            };

            _context.Log.Add(log);
            _context.SaveChanges();
        }
    }
}
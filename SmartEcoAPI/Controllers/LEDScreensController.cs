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
    public class LEDScreensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LEDScreensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LEDScreens
        /// <summary>
        /// Получение списка LED-экранов.
        /// </summary>
        /// <param name="SortOrder">
        /// Сортировка данных. Доступные значения: null (пустое поле), "Name", "MonitoringPost", "NameDesc", "MonitoringPostDesc".
        /// </param>
        /// <param name="Name">
        /// Наименование LED-экрана. Может быть пустым. Если задан, то возвращаемые данные будут отфильтрованы по наименованию LED-экрана.
        /// </param>
        /// <param name="MonitoringPostId">
        /// Id поста мониторинга. Может быть пустым. Если задан, то возвращаемые данные будут отфильтрованы по посту мониторинга.
        /// </param>
        /// <param name="PageSize">
        /// Все возвращаемые данные разделены на блоки (страницы). Данный параметр задает размер блока. Если не задан, то размер блока будет равен числу всех LED-экранов.
        /// </param>
        /// <param name="PageNumber">
        /// Номер возвращаемого блока. Если не задан, то номер блока равен 1.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<LEDScreen>>> GetLEDScreen(string SortOrder,
            string Name,
            int? MonitoringPostId,
            int? PageSize,
            int? PageNumber)
        {
            var ledScreens = _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                ledScreens = ledScreens.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (MonitoringPostId != null)
            {
                ledScreens = ledScreens.Where(m => m.MonitoringPostId == MonitoringPostId);
            }

            switch (SortOrder)
            {
                case "Name":
                    ledScreens = ledScreens.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    ledScreens = ledScreens.OrderByDescending(k => k.Name);
                    break;
                case "MonitoringPost":
                    ledScreens = ledScreens.OrderBy(k => k.MonitoringPost);
                    break;
                case "MonitoringPostDesc":
                    ledScreens = ledScreens.OrderByDescending(k => k.MonitoringPost);
                    break;
                default:
                    ledScreens = ledScreens.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                ledScreens = ledScreens.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await ledScreens.ToListAsync();
        }

        // GET: api/LEDScreens/5
        [HttpGet("{id}")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LEDScreen>> GetLEDScreen(int id)
        {
            var ledScreen = await _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ledScreen == null)
            {
                return NotFound();
            }

            return ledScreen;
        }

        // PUT: api/LEDScreens/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PutLEDScreen(int id, LEDScreen ledScreen)
        {
            if (id != ledScreen.Id)
            {
                return BadRequest();
            }

            _context.Entry(ledScreen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LEDScreenExists(id))
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

        // POST: api/LEDScreens
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LEDScreen>> PostLEDScreen(LEDScreen ledScreen)
        {
            _context.LEDScreen.Add(ledScreen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLEDScreen", new { id = ledScreen.Id }, ledScreen);
        }

        // DELETE: api/LEDScreens/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LEDScreen>> DeleteLEDScreen(int id)
        {
            var ledScreen = await _context.LEDScreen
                .Include(m => m.MonitoringPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ledScreen == null)
            {
                return NotFound();
            }

            _context.LEDScreen.Remove(ledScreen);
            await _context.SaveChangesAsync();

            return ledScreen;
        }

        private bool LEDScreenExists(int id)
        {
            return _context.LEDScreen.Any(e => e.Id == id);
        }

        // GET: api/LEDScreens/Count
        [HttpGet("count")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<LEDScreen>>> GetLEDScreenCount(string Name,
            int? MonitoringPostId)
        {
            var ledScreens = _context.LEDScreen
                .Where(k => true);

            if (!string.IsNullOrEmpty(Name))
            {
                ledScreens = ledScreens.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (MonitoringPostId != null)
            {
                ledScreens = ledScreens.Where(m => m.MonitoringPostId == MonitoringPostId);
            }

            int count = await ledScreens.CountAsync();

            return Ok(count);
        }

        //// GET: api/GetAQI
        ///// <summary>
        ///// Получение информации по AQI с поста мониторинга.
        ///// </summary>
        ///// <param name="ledScreenId">
        ///// Id LED-экрана.
        ///// </param>
        ///// <returns></returns>
        //[HttpGet("GetAQI")]
        //public JsonResult GetAQI(int ledScreenId)
        //{
        //    dynamic indexResult = null;
        //    string level = "";
        //    var jsonResult = Enumerable.Range(0, 0)
        //        .Select(e => new { AQI = .0m, Level = "" })
        //        .ToList();

        //    var ledScreens = _context.LEDScreen
        //        .Where(l => l.Id == ledScreenId)
        //        .LastOrDefault();

        //    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters
        //                .Where(m => m.MonitoringPostId == ledScreens.MonitoringPostId)
        //                .OrderBy(m => m.MeasuredParameter.NameRU)
        //                .Include(m => m.MeasuredParameter)
        //                .ToList();

        //    foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
        //    {
        //        var measuredData = _context.MeasuredData
        //            .Where(m => m.MonitoringPostId == ledScreens.MonitoringPostId && m.MeasuredParameterId == monitoringPostMeasuredParameter.MeasuredParameterId && m.Averaged == true && m.MeasuredParameter.MPCMaxSingle != null && m.DateTime != null && m.DateTime >= DateTime.Now.AddMinutes(-20))
        //            .LastOrDefault();
        //        if (measuredData != null)
        //        {
        //            decimal index = Convert.ToDecimal(measuredData.Value / measuredData.MeasuredParameter.MPCMaxSingle);
        //            if (Convert.ToDecimal(indexResult) < index)
        //            {
        //                indexResult = index;
        //            }
        //        }
        //    }
        //    if (indexResult != null)
        //    {
        //        if (indexResult <= 0.2m)
        //        {
        //            level = "Низкий";
        //        }
        //        else if (indexResult <= 0.5m)
        //        {
        //            level = "Повышенный";
        //        }
        //        else if (indexResult <= 1m)
        //        {
        //            level = "Высокий";
        //        }
        //        else
        //        {
        //            level = "Опасный";
        //        }

        //        decimal aqi = Convert.ToDecimal(indexResult);
        //        jsonResult.Add(new { AQI = aqi, Level = level });
        //    }
        //    else
        //    {
        //        jsonResult.Add(new { AQI = .0m, Level = "Data not found!" });
        //    }

        //    return new JsonResult(jsonResult);
        //}

        // GET: api/GetAQI
        /// <summary>
        /// Получение информации по AQI с поста мониторинга.
        /// </summary>
        /// <param name="ledScreenId">
        /// Id LED-экрана.
        /// </param>
        /// <returns></returns>
        [HttpGet("GetAQI")]
        public JsonResult GetAQI(int ledScreenId)
        {
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { fullname = "", index = "", value = .0m })
                .ToList();

            var ledScreens = _context.LEDScreen
                .Where(l => l.Id == ledScreenId)
                .LastOrDefault();

            var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters
                        .Where(m => m.MonitoringPostId == ledScreens.MonitoringPostId)
                        .OrderBy(m => m.MeasuredParameter.NameRU)
                        .Include(m => m.MeasuredParameter)
                        .ToList();

            foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
            {
                var measuredData = _context.MeasuredData
                    .Include(m => m.MeasuredParameter)
                    .Where(m => m.MonitoringPostId == ledScreens.MonitoringPostId && m.MeasuredParameterId == monitoringPostMeasuredParameter.MeasuredParameterId && m.Averaged == true && m.MeasuredParameter.MPCMaxSingle != null && m.DateTime != null && m.DateTime >= DateTime.Now.AddMinutes(-20))
                    .LastOrDefault();
                if (measuredData != null)
                {
                    string code = $"({measuredData.MeasuredParameter.KazhydrometCode}) мкг/м3";
                    decimal val = Convert.ToDecimal(measuredData.Value * 1000); //мг в мкг
                    jsonResult.Add(new { fullname = measuredData.MeasuredParameter.NameRU, index = code, value = val });
                }
            }

            return new JsonResult(jsonResult);
        }

        [HttpGet("GetAQIPosts")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public JsonResult GetAQIPosts()
        {
            dynamic indexResult = null;
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { Id = 0, AQI = indexResult })
                .ToList();
            //var emptyList = new List<Tuple<int, decimal>>()
            //    .Select(t => new { Id = 0, AQI = indexResult })
            //    .ToList();

            var monitoringPosts = _context.MonitoringPost
                .Where(m => m.Project != null && m.Project.Name == "Almaty" && m.PollutionEnvironmentId == 2 && m.TurnOnOff == true)
                .ToList();
            foreach (var monitoringPost in monitoringPosts)
            {
                indexResult = null;
                var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters
                            .Where(m => m.MonitoringPostId == monitoringPost.Id)
                            .OrderBy(m => m.MeasuredParameter.NameRU)
                            .Include(m => m.MeasuredParameter)
                            .ToList();

                foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
                {
                    var measuredData = _context.MeasuredData
                        .Where(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == monitoringPostMeasuredParameter.MeasuredParameterId && m.Averaged == true && m.MeasuredParameter.MPCMaxSingle != null && m.DateTime != null && m.DateTime >= DateTime.Now.AddMinutes(-60))
                        .LastOrDefault();
                    if (measuredData != null)
                    {
                        decimal index = Convert.ToDecimal(measuredData.Value / measuredData.MeasuredParameter.MPCMaxSingle);
                        if (Convert.ToDecimal(indexResult) < index)
                        {
                            indexResult = index;
                        }
                    }
                }
                
                if (indexResult != null)
                {
                    jsonResult.Add(new { Id = monitoringPost.Id, AQI = indexResult });
                }
            }

            return new JsonResult(jsonResult);
        }

        [HttpGet("GetPollutantsConcentration")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public JsonResult GetPollutantsConcentration(int MonitoringPostId)
        {
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { NameRU = "", NameEN = "", NameKK = "", AQI = .0m })
                .ToList();
            var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters
                        .Where(m => m.MonitoringPostId == MonitoringPostId)
                        .OrderBy(m => m.MeasuredParameter.NameRU)
                        .Include(m => m.MeasuredParameter)
                        .ToList();

            foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
            {
                var measuredData = _context.MeasuredData
                    .Where(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == monitoringPostMeasuredParameter.MeasuredParameterId && m.Averaged == true && m.MeasuredParameter.MPCMaxSingle != null && m.DateTime != null && m.DateTime >= DateTime.Now.AddMinutes(-60))
                    .LastOrDefault();
                if (measuredData != null)
                {
                    jsonResult.Add(new {
                        NameRU = monitoringPostMeasuredParameter.MeasuredParameter.NameRU,
                        NameEN = monitoringPostMeasuredParameter.MeasuredParameter.NameEN,
                        NameKK = monitoringPostMeasuredParameter.MeasuredParameter.NameKK,
                        AQI = Convert.ToDecimal(measuredData.Value / measuredData.MeasuredParameter.MPCMaxSingle) });
                }
            }

            return new JsonResult(jsonResult);
        }
    }
}
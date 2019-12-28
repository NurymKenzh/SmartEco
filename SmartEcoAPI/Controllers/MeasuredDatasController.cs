using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    public class PostData
    {
        public string Data { get; set; }
        public DateTime DateTimeServer { get; set; }
        public DateTime? DateTimePost { get; set; }
        public string MN { get; set; }
        public string IP { get; set; }
        public bool? Taken { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class MeasuredDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public int COMPCDivide = 1000; // было 10  // Id = 7
        public decimal? PValueMultiply = 0.750063755419211m, // Id = 1
            NO2ValueMultiply = 0.001m, // Id = 13
            SO2ValueMultiply = 0.001m, // Id = 9
            H2SValueMultiply = 0.001m, // Id = 20
            PM10ValueMultiply = 0.001m, // Id = 2
            PM25ValueMultiply = 0.001m; // Id = 3

        public MeasuredDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredDatas
        /// <summary>
        /// Получение измеренных данных с постов мониторинга. Работает только для авторизованных пользователей.
        /// </summary>
        /// <param name="SortOrder">
        /// Сортировка данных. Доступные значения: null, "MeasuredParameter", "DateTime", "MonitoringPost", "PollutionSource", "MeasuredParameterDesc", "DateTimeDesc", "MonitoringPostDesc", "PollutionSourceDesc".
        /// </param>
        /// <param name="Language">
        /// Язык возвращаемых данных. Доступные значения: null, "", "kk", "ru", "en".
        /// </param>
        /// <param name="MeasuredParameterId">
        /// Id измеряемого параметра. Может быть пустым. Если задан, то возвращаемые данные будут отфильтрованы по измеряемому параметру.
        /// </param>
        /// <param name="DateTimeFrom">
        /// Начальные дата и время. Поле может быть пустым. Если нет, то все возвращаемые данные будут больше или равны, чем заданные.
        /// Например, "2019-09-23T00:00:00".
        /// </param>
        /// <param name="DateTimeTo">
        /// Конечные дата и время. Поле может быть пустым. Если нет, то все возвращаемые данные будут меньше или равны, чем заданные.
        /// Например, "2019-09-23T00:00:00".
        /// </param>
        /// <param name="MonitoringPostId">
        /// Id поста мониторинга. Может быть пустым. Если задан, то возвращаемые данные будут отфильтрованы по посту мониторинга.
        /// </param>
        /// <param name="PollutionSourceId">
        /// Id источника выделения загрязнения. Может быть пустым. Если задан, то возвращаемые данные будут отфильтрованы по источнику выделения загрязнения.
        /// </param>
        /// <param name="PageSize">
        /// Все возвращаемые данные разделены на блоки (страницы). Данный параметр задает размер блока. Если не задан, то размер блока будет равен 10.
        /// </param>
        /// <param name="PageNumber">
        /// Номер возвращаемого блока. Если не задан, то номер блока равер 1.
        /// </param>
        /// <param name="Averaged">
        /// Данный параметр определяет какие данные будут возвращены: усредненные или нет. Если не задан, то будут возвращены усредненные данные.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Kazakhtelecom,Arys")]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetMeasuredData(string SortOrder,
            string Language,
            int? MeasuredParameterId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            int? MonitoringPostId,
            int? PollutionSourceId,
            int? PageSize,
            int? PageNumber,
            bool? Averaged = true)
        {
            //PopulateEcoserviceData();
            //GetPostsData();

            Person person = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name);
            if (!(new string[] { "admin", "moderator" }).Contains(person?.Role))
            {
                Averaged = true;
            }

            var measuredDatas = _context.MeasuredData
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .Include(m => m.PollutionSource)
                .Where(m => true);

            if (MonitoringPostId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MonitoringPostId == MonitoringPostId);
                var measuredParameters = _context.MeasuredParameter.ToList();
                var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId).ToList();
                foreach (var item in monitoringPostMeasuredParameters)
                {
                    measuredParameters.RemoveAll(m => m.Id == item.MeasuredParameterId);
                }
                foreach (var measuredParameter in measuredParameters)
                {
                    measuredDatas = measuredDatas.Where(m => m.MeasuredParameterId != measuredParameter.Id);
                }
            }
            else
            {
                var monitoringPosts = _context.MonitoringPost
                    .Where(m => true);

                var selection = $"SELECT * FROM public.\"MeasuredData\" WHERE";

                //var measuredParameters = _context.MeasuredParameter.ToList();

                //foreach (var monitoringPost in monitoringPosts)
                //{
                //    var measuredParameters = _context.MeasuredParameter.ToList();
                //    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id).ToList();
                //    foreach (var item in monitoringPostMeasuredParameters)
                //    {
                //        measuredParameters.RemoveAll(m => m.Id == item.MeasuredParameterId);
                //    }
                //    foreach (var measuredParameter in measuredParameters)
                //    {
                //        measuredDatas = measuredDatas.Where(m => m.MonitoringPostId != monitoringPost.Id && m.MeasuredParameterId != measuredParameter.Id);
                //    }
                //}

                foreach (var monitoringPost in monitoringPosts)
                {
                    var measuredParameters = _context.MeasuredParameter.ToList();
                    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id).ToList();
                    if (monitoringPostMeasuredParameters.Count != 0)
                    {
                        selection += $" \"MonitoringPostId\" = {monitoringPost.Id} AND (";
                        foreach (var item in monitoringPostMeasuredParameters)
                        {
                            selection += $"\"MeasuredParameterId\" = {item.MeasuredParameterId} OR ";
                        }
                        selection = selection.Substring(0, selection.Length - 4);
                        selection += $") OR";
                    }
                }
                selection = selection.Substring(0, selection.Length - 3);
                measuredDatas = _context.MeasuredData.FromSql(selection);
            }
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
                    (m.Year != null && m.Month != null && m.Year <= DateTimeTo.Value.Year && m.Month <= DateTimeTo.Value.Month));
            }
            if (PollutionSourceId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.PollutionSourceId == PollutionSourceId);
            }
            if (Averaged == true)
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == Averaged);
            }
            else
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == null || m.Averaged == false);
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

            List<MeasuredData> measuredDatasR = measuredDatas.ToList();
            if (measuredDatasR[0].MonitoringPost.DataProviderId != 1)
            {
                measuredDatasR = measuredDatasR
                    .Select(m =>
                    {
                        m.Value =
                            (m.MeasuredParameterId == 7 && (m.MonitoringPostId == 44 || m.MonitoringPostId == 50 || m.MonitoringPostId == 47)) ? m.Value / COMPCDivide :
                            m.MeasuredParameterId == 1 ? m.Value * PValueMultiply :
                            m.MeasuredParameterId == 13 ? m.Value * NO2ValueMultiply :
                            m.MeasuredParameterId == 9 ? m.Value * SO2ValueMultiply :
                            m.MeasuredParameterId == 20 ? m.Value * H2SValueMultiply :
                            m.MeasuredParameterId == 2 ? m.Value * PM10ValueMultiply :
                            m.MeasuredParameterId == 3 ? m.Value * PM25ValueMultiply :
                            m.Value;
                        return m;
                    })
                    .ToList();
            }

            return measuredDatasR;
        }

        // GET: api/MeasuredDatas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

            measuredData.Value = (measuredData.MeasuredParameterId == 7 && (measuredData.MonitoringPostId == 44 || measuredData.MonitoringPostId == 50 || measuredData.MonitoringPostId == 47)) ? measuredData.Value / COMPCDivide :
                measuredData.MeasuredParameterId == 1 ? measuredData.Value * PValueMultiply :
                measuredData.MeasuredParameterId == 13 ? measuredData.Value * NO2ValueMultiply :
                measuredData.MeasuredParameterId == 9 ? measuredData.Value * SO2ValueMultiply :
                measuredData.MeasuredParameterId == 20 ? measuredData.Value * H2SValueMultiply :
                measuredData.MeasuredParameterId == 2 ? measuredData.Value * PM10ValueMultiply :
                measuredData.MeasuredParameterId == 3 ? measuredData.Value * PM25ValueMultiply :
                measuredData.Value;

            return measuredData;
        }

        //// PUT: api/MeasuredDatas/5
        //[HttpPut("{id}")]
        //[Authorize(Roles = "admin,moderator")]
        //public async Task<IActionResult> PutMeasuredData(int id, MeasuredData measuredData)
        //{
        //    if (id != measuredData.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(measuredData).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MeasuredDataExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/MeasuredDatas
        //[HttpPost]
        //[Authorize(Roles = "admin,moderator")]
        //public async Task<ActionResult<MeasuredData>> PostMeasuredData(MeasuredData measuredData)
        //{
        //    _context.MeasuredData.Add(measuredData);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetMeasuredData", new { id = measuredData.Id }, measuredData);
        //}

        //// DELETE: api/MeasuredDatas/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "admin,moderator")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<ActionResult<MeasuredData>> DeleteMeasuredData(int id)
        //{
        //    //var measuredData = await _context.MeasuredData.FindAsync(id);
        //    var measuredData = await _context.MeasuredData
        //        .Include(m => m.MeasuredParameter)
        //        .Include(m => m.MonitoringPost)
        //        .Include(m => m.PollutionSource)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (measuredData == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.MeasuredData.Remove(measuredData);
        //    await _context.SaveChangesAsync();

        //    if (measuredData.MeasuredParameterId == 7)
        //    {
        //        measuredData.Value = measuredData.Value / COMPCDivide;
        //    }

        //    if (measuredData.MeasuredParameterId == 1)
        //    {
        //        measuredData.Value = measuredData.Value * PValueMultiply;
        //    }

        //    return measuredData;
        //}

        private bool MeasuredDataExists(int id)
        {
            return _context.MeasuredData.Any(e => e.Id == id);
        }

        // GET: api/MeasuredDatas/Count
        /// <summary>
        /// Получение количества измеренных данных с постов мониторинга. Работает только для авторизованных пользователей.
        /// </summary>
        /// <param name="MeasuredParameterId">
        /// Id измеряемого параметра. Может быть пустым. Если задан, то количество возвращаемых данных будет отфильтровано по измеряемому параметру.
        /// </param>
        /// <param name="DateTimeFrom">
        /// Начальные дата и время. Поле может быть пустым. Если нет, то количество возвращаемых данных будет отфильтровано: больше или равно, чем заданные.
        /// Например, "2019-09-23T00:00:00".
        /// </param>
        /// <param name="DateTimeTo">
        /// Конечные дата и время. Поле может быть пустым. Если нет, то количество возвращаемых данных будет отфильтровано: меньше или равно, чем заданные.
        /// Например, "2019-09-23T00:00:00".
        /// </param>
        /// <param name="MonitoringPostId">
        /// Id поста мониторинга. Может быть пустым. Если задан, то количество возвращаемыех данных будет отфильтровано по посту мониторинга.
        /// </param>
        /// <param name="PollutionSourceId">
        /// Id источника выделения загрязнения. Может быть пустым. Если задан, то количество возвращаемых данных будет отфильтровано по источнику выделения загрязнения.
        /// </param>
        /// <param name="Averaged">Данный параметр определяет количество каких данных будет возвращено: усредненных или нет. Если не задан, то будет возвращено количество усредненных данных.</param>
        /// <returns></returns>
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Kazakhtelecom,Arys")]
        public async Task<ActionResult<IEnumerable<MeasuredData>>> GetMeasuredDatasCount(int? MeasuredParameterId,
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            int? MonitoringPostId,
            int? PollutionSourceId,
            bool? Averaged = true)
        {
            Person person = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name);
            if (!(new string[] { "admin", "moderator" }).Contains(person.Role))
            {
                Averaged = true;
            }

            var measuredDatas = _context.MeasuredData
                 .Where(m => true);
            
            if (MonitoringPostId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.MonitoringPostId == MonitoringPostId);
                var measuredParameters = _context.MeasuredParameter.ToList();
                var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId).ToList();
                foreach (var item in monitoringPostMeasuredParameters)
                {
                    measuredParameters.RemoveAll(m => m.Id == item.MeasuredParameterId);
                }
                foreach (var measuredParameter in measuredParameters)
                {
                    measuredDatas = measuredDatas.Where(m => m.MeasuredParameterId != measuredParameter.Id);
                }
            }
            else
            {
                var monitoringPosts = _context.MonitoringPost
                    .Where(m => true);

                var selection = $"SELECT * FROM public.\"MeasuredData\" WHERE";

                foreach (var monitoringPost in monitoringPosts)
                {
                    var measuredParameters = _context.MeasuredParameter.ToList();
                    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id).ToList();
                    if (monitoringPostMeasuredParameters.Count != 0)
                    {
                        selection += $" \"MonitoringPostId\" = {monitoringPost.Id} AND (";
                        foreach (var item in monitoringPostMeasuredParameters)
                        {
                            selection += $"\"MeasuredParameterId\" = {item.MeasuredParameterId} OR ";
                        }
                        selection = selection.Substring(0, selection.Length - 4);
                        selection += $") OR";
                    }
                }
                selection = selection.Substring(0, selection.Length - 3);
                measuredDatas = _context.MeasuredData.FromSql(selection);
            }
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
            if (PollutionSourceId != null)
            {
                measuredDatas = measuredDatas.Where(m => m.PollutionSourceId == PollutionSourceId);
            }
            if (Averaged == true)
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == Averaged);
            }
            else
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == null || m.Averaged == false);
            }

            int count = await measuredDatas.CountAsync();

            return Ok(count);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void PopulateEcoserviceData()
        {
            Random rnd = new Random();
            DateTime start = (DateTime)_context.MeasuredData
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

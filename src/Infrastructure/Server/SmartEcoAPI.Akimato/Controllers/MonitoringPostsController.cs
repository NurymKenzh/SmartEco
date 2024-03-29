﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SmartEcoAPI.Akimato.Data;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MonitoringPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MonitoringPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MonitoringPosts
        [HttpGet]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Kazhydromet")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetMonitoringPost(string SortOrder,
            int? Number,
            string Name,
            int? DataProviderId,
            int? PollutionEnvironmentId,
            string MN,
            int? PageSize,
            int? PageNumber)
        {
            var monitoringPosts = _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Include(m => m.Project)
                .Where(m => true);

            if (Number != null)
            {
                monitoringPosts = monitoringPosts.Where(k => k.Number == Number);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                monitoringPosts = monitoringPosts.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (DataProviderId != null)
            {
                monitoringPosts = monitoringPosts.Where(m => m.DataProviderId == DataProviderId);
            }
            if (PollutionEnvironmentId != null)
            {
                monitoringPosts = monitoringPosts.Where(m => m.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (!string.IsNullOrEmpty(MN))
            {
                monitoringPosts = monitoringPosts.Where(m => m.MN.ToLower().Contains(MN.ToLower()));
            }

            switch (SortOrder)
            {
                case "Number":
                    monitoringPosts = monitoringPosts.OrderBy(k => k.Number);
                    break;
                case "NumberDesc":
                    monitoringPosts = monitoringPosts.OrderByDescending(k => k.Number);
                    break;
                case "Name":
                    monitoringPosts = monitoringPosts.OrderBy(k => k.Name);
                    break;
                case "NameDesc":
                    monitoringPosts = monitoringPosts.OrderByDescending(k => k.Name);
                    break;
                case "DataProvider":
                    monitoringPosts = monitoringPosts.OrderBy(k => k.DataProvider);
                    break;
                case "DataProviderDesc":
                    monitoringPosts = monitoringPosts.OrderByDescending(k => k.DataProvider);
                    break;
                case "PollutionEnvironment":
                    monitoringPosts = monitoringPosts.OrderBy(k => k.PollutionEnvironment);
                    break;
                case "PollutionEnvironmentDesc":
                    monitoringPosts = monitoringPosts.OrderByDescending(k => k.PollutionEnvironment);
                    break;
                case "MN":
                    monitoringPosts = monitoringPosts.OrderBy(k => k.MN);
                    break;
                case "MNDesc":
                    monitoringPosts = monitoringPosts.OrderByDescending(k => k.MN);
                    break;
                default:
                    monitoringPosts = monitoringPosts.OrderBy(m => m.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                monitoringPosts = monitoringPosts.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await monitoringPosts.ToListAsync();
        }

        // GET: api/MonitoringPosts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys")]
        public async Task<ActionResult<MonitoringPost>> GetMonitoringPost(long id)
        {
            //var monitoringPost = await _context.MonitoringPost.FindAsync(id);
            var monitoringPost = await _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monitoringPost == null)
            {
                return NotFound();
            }

            return monitoringPost;
        }

        // PUT: api/MonitoringPosts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutMonitoringPost(int id, MonitoringPost monitoringPost)
        {
            if (id != monitoringPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(monitoringPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonitoringPostExists(id))
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

        // POST: api/MonitoringPosts
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MonitoringPost>> PostMonitoringPost(MonitoringPost monitoringPost)
        {
            _context.MonitoringPost.Add(monitoringPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMonitoringPost", new { id = monitoringPost.Id }, monitoringPost);
        }

        // DELETE: api/MonitoringPosts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MonitoringPost>> DeleteMonitoringPost(int id)
        {
            //var monitoringPost = await _context.MonitoringPost.FindAsync(id);
            var monitoringPost = await _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitoringPost == null)
            {
                return NotFound();
            }

            try
            {
                _context.MeasuredData.RemoveRange(_context.MeasuredData.Where(m => m.MonitoringPostId == id));
                await _context.SaveChangesAsync();

                _context.MonitoringPost.Remove(monitoringPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }


            return monitoringPost;
        }

        private bool MonitoringPostExists(int id)
        {
            return _context.MonitoringPost.Any(e => e.Id == id);
        }

        // GET: api/MonitoringPosts/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetMonitoringPostsCount(int? Number,
            string Name,
            int? DataProviderId,
            int? PollutionEnvironmentId,
            string MN)
        {
            var monitoringPosts = _context.MonitoringPost
                 .Where(m => true);

            if (Number != null)
            {
                monitoringPosts = monitoringPosts.Where(k => k.Number == Number);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                monitoringPosts = monitoringPosts.Where(m => m.Name.ToLower().Contains(Name.ToLower()));
            }
            if (DataProviderId != null)
            {
                monitoringPosts = monitoringPosts.Where(m => m.DataProviderId == DataProviderId);
            }
            if (PollutionEnvironmentId != null)
            {
                monitoringPosts = monitoringPosts.Where(m => m.PollutionEnvironmentId == PollutionEnvironmentId);
            }
            if (!string.IsNullOrEmpty(MN))
            {
                monitoringPosts = monitoringPosts.Where(m => m.MN.ToLower().Contains(MN.ToLower()));
            }

            int count = await monitoringPosts.CountAsync();

            return Ok(count);
        }

        // GET: api/MonitoringPosts/Exceed
        [HttpGet("exceed")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Kazhydromet")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetEcoserviceMonitoringPostsExceed(int MPCExceedPastMinutes,
            int? DataProviderId)
        {
            // populate data (delete)
            MeasuredDatasController measuredDatasController = new MeasuredDatasController(_context);
            //measuredDatasController.PopulateEcoserviceData();
            //measuredDatasController.GetPostsData();

            DateTime minExceedDateTime = DateTime.Now.AddMinutes(-MPCExceedPastMinutes);

            var monitoringPosts = _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Include(m => m.Project)
                .Where(m => (m.DataProviderId == (int)DataProviderId) || DataProviderId == null);

            foreach (MonitoringPost monitoringPost in monitoringPosts)
            {
                bool exceed = _context.MeasuredData
                    .Where(m => m.MonitoringPostId == monitoringPost.Id
                        && m.DateTime >= minExceedDateTime
                        && m.Averaged == true
                        )
                    .Include(m => m.MeasuredParameter)
                    .ToList()
                    //.Select(m =>
                    //{
                    //    m.Value =
                    //        //(m.MeasuredParameterId == 7 && (m.MonitoringPostId == 44 || m.MonitoringPostId == 50 || m.MonitoringPostId == 47)) ? m.Value / measuredDatasController.COMPCDivide :
                    //        m.MeasuredParameterId == 1 ? m.Value * measuredDatasController.PValueMultiply :
                    //        m.MeasuredParameterId == 13 ? m.Value * measuredDatasController.NO2ValueMultiply :
                    //        m.MeasuredParameterId == 9 ? m.Value * measuredDatasController.SO2ValueMultiply :
                    //        m.MeasuredParameterId == 20 ? m.Value * measuredDatasController.H2SValueMultiply :
                    //        m.MeasuredParameterId == 2 ? m.Value * measuredDatasController.PM10ValueMultiply :
                    //        m.MeasuredParameterId == 3 ? m.Value * measuredDatasController.PM25ValueMultiply :
                    //        m.Value;
                    //    return m;
                    //})
                    .Where(m => m.Value > m.MeasuredParameter.MPCMaxSingle && m.MeasuredParameter.MPCMaxSingle != null)
                    .FirstOrDefault() != null;
                if (!exceed)
                {
                    monitoringPosts = monitoringPosts
                        .Where(m => m.Id != monitoringPost.Id);
                }
            }

            return await monitoringPosts.ToListAsync();
        }

        // GET: api/MonitoringPosts/Exceed
        [HttpGet("inactive")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Kazhydromet")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetEcoserviceMonitoringPostsInactive(int InactivePastMinutes,
            int? DataProviderId)
        {
            MeasuredDatasController measuredDatasController = new MeasuredDatasController(_context);

            DateTime minExceedDateTime = DateTime.Now.AddMinutes(-InactivePastMinutes);

            var monitoringPosts = _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Include(m => m.Project)
                .Where(m => (m.DataProviderId == (int)DataProviderId) || DataProviderId == null);
            var activePosts = _context.MeasuredData
                .Where(m => m.DateTime >= minExceedDateTime)
                .ToList();

            List<MonitoringPost> monitoringPostsInactive = new List<MonitoringPost>();

            foreach (MonitoringPost monitoringPost in monitoringPosts)
            {
                bool active = activePosts
                    .Where(m => m.MonitoringPostId == monitoringPost.Id)
                    //.Include(m => m.MeasuredParameter)
                    //.Where(m => m.Value > m.MeasuredParameter.MPC && m.MeasuredParameter.MPC != null)
                    .FirstOrDefault() != null;
                if (!active)
                {
                    monitoringPostsInactive.Add(monitoringPost);
                }
            }

            return monitoringPostsInactive.ToList();
        }

        //// POST: api/MonitoringPosts/MonitoringPostMeasuredParameter
        //[HttpPost("monitoringPostMeasuredParameter")]
        //[Authorize(Roles = "admin,moderator")]
        //public void PostMonitoringPostMeasuredParameter(
        //    int MonitoringPostId,
        //    int DataProviderId,
        //    int PollutionEnvironmentId,
        //    string CultureName,
        //    [FromQuery(Name = "MeasuredParametersId")] List<int> MeasuredParametersId,
        //    [FromQuery(Name = "Min")] List<string> Min,
        //    [FromQuery(Name = "Max")] List<string> Max,
        //    [FromQuery(Name = "MinMeasuredValue")] List<string> MinMeasuredValue,
        //    [FromQuery(Name = "MaxMeasuredValue")] List<string> MaxMeasuredValue,
        //    [FromQuery(Name = "Coefficient")] List<string> Coefficient)
        //{
        //    List<int> idMeasuredParameters = new List<int>();
        //    List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
        //    //Water
        //    if (PollutionEnvironmentId == 3)
        //    {
        //        measuredParameters = _context.MeasuredParameter.Where(m => m.PollutionEnvironmentId == 3).ToList();
        //    }
        //    else
        //    {
        //        //Kazhydromet
        //        if (DataProviderId == 1)
        //        {
        //            measuredParameters = _context.MeasuredParameter.Where(m => m.KazhydrometCode != null).ToList();
        //        }
        //        //Ecoservice, Urus
        //        else if (DataProviderId == 3 || DataProviderId == 2)
        //        {
        //            measuredParameters = _context.MeasuredParameter.Where(m => m.OceanusCode != null).ToList();
        //        }
        //        else
        //        {
        //            measuredParameters = _context.MeasuredParameter.ToList();
        //        }
        //    }

        //    if (CultureName == "ru")
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameRU).Select(m => m.Id).ToList();
        //    }
        //    else if (CultureName == "kk")
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameKK).Select(m => m.Id).ToList();
        //    }
        //    else
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameEN).Select(m => m.Id).ToList();
        //    }

        //    for (int i = 0; i < idMeasuredParameters.Count; i++)
        //    {
        //        foreach (var id in MeasuredParametersId)
        //        {
        //            if (idMeasuredParameters[i] == id)
        //            {
        //                decimal? minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef;
        //                if (Min[i] == "null")
        //                {
        //                    minDec = null;
        //                }
        //                else
        //                {
        //                    minDec = Decimal.Parse(Min[i], CultureInfo.InvariantCulture);
        //                }
        //                if (Max[i] == "null")
        //                {
        //                    maxDec = null;
        //                }
        //                else
        //                {
        //                    maxDec = Decimal.Parse(Max[i], CultureInfo.InvariantCulture);
        //                }
        //                if (MinMeasuredValue[i] == "null")
        //                {
        //                    minMeasuredDec = null;
        //                }
        //                else
        //                {
        //                    minMeasuredDec = Decimal.Parse(MinMeasuredValue[i], CultureInfo.InvariantCulture);
        //                }
        //                if (MaxMeasuredValue[i] == "null")
        //                {
        //                    maxMeasuredDec = null;
        //                }
        //                else
        //                {
        //                    maxMeasuredDec = Decimal.Parse(MaxMeasuredValue[i], CultureInfo.InvariantCulture);
        //                }
        //                if (Coefficient[i] == "null")
        //                {
        //                    coef = null;
        //                }
        //                else
        //                {
        //                    coef = Decimal.Parse(Coefficient[i], CultureInfo.InvariantCulture);
        //                }

        //                Task.WaitAll(MonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));

        //                //if (Min[i] == "null" && Max[i] != "null")
        //                //{
        //                //    MonitoringPostMeasuredParameter(MonitoringPostId, id, null, Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //}
        //                //else if (Max[i] == "null" && Min[i] != "null")
        //                //{
        //                //    MonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), null);
        //                //}
        //                //else if (Max[i] == "null" && Min[i] == "null")
        //                //{
        //                //    MonitoringPostMeasuredParameter(MonitoringPostId, id, null, null);
        //                //}
        //                //else
        //                //{
        //                //    MonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //}
        //            }
        //        }
        //    }
        //}

                    // POST: api/MonitoringPosts/MonitoringPostMeasuredParameter
        [HttpPost("monitoringPostMeasuredParameter")]
        [Authorize(Roles = "admin,moderator")]
        public void PostMonitoringPostMeasuredParameter([FromBody] JObject content)
        {
            dynamic datas = content;
            int MonitoringPostId = datas.MonitoringPostId;
            int DataProviderId = datas.DataProviderId;
            int PollutionEnvironmentId = datas.PollutionEnvironmentId;
            string CultureName = datas.CultureName;
            List<int> MeasuredParametersId = datas.MeasuredParametersId.ToObject<List<int>>();
            List<string> Min = datas.Min.ToObject<List<string>>();
            List<string> Max = datas.Max.ToObject<List<string>>();
            List<string> MinMeasuredValue = datas.MinMeasuredValue.ToObject<List<string>>();
            List<string> MaxMeasuredValue = datas.MaxMeasuredValue.ToObject<List<string>>();
            List<string> Coefficient = datas.Coefficient.ToObject<List<string>>();

            List<int> idMeasuredParameters = new List<int>();
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            //Water
            if (PollutionEnvironmentId == 3)
            {
                measuredParameters = _context.MeasuredParameter.Where(m => m.PollutionEnvironmentId == 3).ToList();
            }
            else
            {
                //Kazhydromet
                if (DataProviderId == 1)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.KazhydrometCode != null).ToList();
                }
                //Ecoservice, Urus
                else if (DataProviderId == 3 || DataProviderId == 2)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.OceanusCode != null).ToList();
                }
                else
                {
                    measuredParameters = _context.MeasuredParameter.ToList();
                }
            }

            if (CultureName == "ru")
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameRU).Select(m => m.Id).ToList();
            }
            else if (CultureName == "kk")
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameKK).Select(m => m.Id).ToList();
            }
            else
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameEN).Select(m => m.Id).ToList();
            }

            for (int i = 0; i < idMeasuredParameters.Count; i++)
            {
                foreach (var id in MeasuredParametersId)
                {
                    if (idMeasuredParameters[i] == id)
                    {
                        decimal? minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef;
                        if (Min[i] == "null")
                        {
                            minDec = null;
                        }
                        else
                        {
                            minDec = Decimal.Parse(Min[i], CultureInfo.InvariantCulture);
                        }
                        if (Max[i] == "null")
                        {
                            maxDec = null;
                        }
                        else
                        {
                            maxDec = Decimal.Parse(Max[i], CultureInfo.InvariantCulture);
                        }
                        if (MinMeasuredValue[i] == "null")
                        {
                            minMeasuredDec = null;
                        }
                        else
                        {
                            minMeasuredDec = Decimal.Parse(MinMeasuredValue[i], CultureInfo.InvariantCulture);
                        }
                        if (MaxMeasuredValue[i] == "null")
                        {
                            maxMeasuredDec = null;
                        }
                        else
                        {
                            maxMeasuredDec = Decimal.Parse(MaxMeasuredValue[i], CultureInfo.InvariantCulture);
                        }
                        if (Coefficient[i] == "null")
                        {
                            coef = null;
                        }
                        else
                        {
                            coef = Decimal.Parse(Coefficient[i], CultureInfo.InvariantCulture);
                        }

                        Task.WaitAll(MonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));
                    }
                }
            }
        }

        public async Task MonitoringPostMeasuredParameter(int MonitoringPostId,
            int MeasuredParameterId,
            decimal? Min,
            decimal? Max,
            decimal? MinMeasuredValue,
            decimal? MaxMeasuredValue,
            decimal? Coefficient)
        {
            MonitoringPostMeasuredParameters monitoringPostMeasuredParameters = new MonitoringPostMeasuredParameters
            {
                MonitoringPostId = MonitoringPostId,
                MeasuredParameterId = MeasuredParameterId,
                Min = Min,
                Max = Max,
                MinMeasuredValue = MinMeasuredValue,
                MaxMeasuredValue = MaxMeasuredValue,
                Coefficient = Coefficient
            };

            _context.MonitoringPostMeasuredParameters.Add(monitoringPostMeasuredParameters);
            await _context.SaveChangesAsync();
        }

        //// POST: api/MonitoringPosts/EditMonitoringPostMeasuredParameter
        //[HttpPost("editMonitoringPostMeasuredParameter")]
        //[Authorize(Roles = "admin,moderator")]
        //public async Task<ActionResult<MonitoringPostMeasuredParameters>> EditMonitoringPostMeasuredParameter(
        //    int MonitoringPostId,
        //    int DataProviderId,
        //    int PollutionEnvironmentId,
        //    string CultureName,
        //    [FromQuery(Name = "MeasuredParametersId")] List<int> MeasuredParametersId,
        //    [FromQuery(Name = "Min")] List<string> Min,
        //    [FromQuery(Name = "Max")] List<string> Max,
        //    [FromQuery(Name = "MinMeasuredValue")] List<string> MinMeasuredValue,
        //    [FromQuery(Name = "MaxMeasuredValue")] List<string> MaxMeasuredValue,
        //    [FromQuery(Name = "Coefficient")] List<string> Coefficient)
        //{
        //    List<int> idMeasuredParameters = new List<int>();
        //    List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
        //    //Water
        //    if (PollutionEnvironmentId == 3)
        //    {
        //        measuredParameters = _context.MeasuredParameter.Where(m => m.PollutionEnvironmentId == 3).ToList();
        //    }
        //    else
        //    {
        //        //Kazhydromet
        //        if (DataProviderId == 1)
        //        {
        //            measuredParameters = _context.MeasuredParameter.Where(m => m.KazhydrometCode != null).ToList();
        //        }
        //        //Ecoservice, Urus
        //        else if (DataProviderId == 3 || DataProviderId == 2)
        //        {
        //            measuredParameters = _context.MeasuredParameter.Where(m => m.OceanusCode != null).ToList();
        //        }
        //        else
        //        {
        //            measuredParameters = _context.MeasuredParameter.ToList();
        //        }
        //    }

        //    if (CultureName == "ru")
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameRU).Select(m => m.Id).ToList();
        //    }
        //    else if (CultureName == "kk")
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameKK).Select(m => m.Id).ToList();
        //    }
        //    else
        //    {
        //        idMeasuredParameters = measuredParameters.OrderBy(m => m.NameEN).Select(m => m.Id).ToList();
        //    }

        //    List<int> idMonitoringPostMeasuredParametersId = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId).OrderBy(m => m.MeasuredParameterId).Select(m => m.MeasuredParameterId).ToList();
        //    for (int i = 0; i < idMeasuredParameters.Count; i++)
        //    {
        //        foreach (var id in MeasuredParametersId)
        //        {
        //            if (idMeasuredParameters[i] == id)
        //            {
        //                decimal? minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef;
        //                if (Min[i] == "null")
        //                {
        //                    minDec = null;
        //                }
        //                else
        //                {
        //                    minDec = Decimal.Parse(Min[i], CultureInfo.InvariantCulture);
        //                }
        //                if (Max[i] == "null")
        //                {
        //                    maxDec = null;
        //                }
        //                else
        //                {
        //                    maxDec = Decimal.Parse(Max[i], CultureInfo.InvariantCulture);
        //                }
        //                if (MinMeasuredValue[i] == "null")
        //                {
        //                    minMeasuredDec = null;
        //                }
        //                else
        //                {
        //                    minMeasuredDec = Decimal.Parse(MinMeasuredValue[i], CultureInfo.InvariantCulture);
        //                }
        //                if (MaxMeasuredValue[i] == "null")
        //                {
        //                    maxMeasuredDec = null;
        //                }
        //                else
        //                {
        //                    maxMeasuredDec = Decimal.Parse(MaxMeasuredValue[i], CultureInfo.InvariantCulture);
        //                }
        //                if (Coefficient[i] == "null")
        //                {
        //                    coef = null;
        //                }
        //                else
        //                {
        //                    coef = Decimal.Parse(Coefficient[i], CultureInfo.InvariantCulture);
        //                }

        //                try
        //                {
        //                    Task.WaitAll(PutMonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));
        //                }
        //                catch (Exception ex)
        //                {
        //                    Task.WaitAll(MonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));
        //                }

        //                //if (Min[i] == "null" && Max[i] != "null")
        //                //{
        //                //    try
        //                //    {
        //                //        PutMonitoringPostMeasuredParameter(MonitoringPostId, id, null, Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //    }
        //                //    catch
        //                //    {
        //                //        MonitoringPostMeasuredParameter(MonitoringPostId, id, null, Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //    }
        //                //}
        //                //else if (Max[i] == "null" && Min[i] != "null")
        //                //{
        //                //    try
        //                //    {
        //                //        PutMonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), null);
        //                //    }
        //                //    catch
        //                //    {
        //                //        MonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), null);
        //                //    }
        //                //}
        //                //else if (Max[i] == "null" && Min[i] == "null")
        //                //{
        //                //    try
        //                //    {
        //                //        PutMonitoringPostMeasuredParameter(MonitoringPostId, id, null, null);
        //                //    }
        //                //    catch
        //                //    {
        //                //        MonitoringPostMeasuredParameter(MonitoringPostId, id, null, null);
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    try
        //                //    {
        //                //        PutMonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //    }
        //                //    catch
        //                //    {
        //                //        MonitoringPostMeasuredParameter(MonitoringPostId, id, Decimal.Parse(Min[i], CultureInfo.InvariantCulture), Decimal.Parse(Max[i], CultureInfo.InvariantCulture));
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //    bool check = false;
        //    foreach (var idAll in idMonitoringPostMeasuredParametersId)
        //    {
        //        foreach (var id in MeasuredParametersId)
        //        {
        //            if (idAll == id)
        //            {
        //                check = true;
        //                break;
        //            }
        //            else
        //            {
        //                check = false;
        //            }
        //        }
        //        if (!check)
        //        {
        //            var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == idAll && m.MonitoringPostId == MonitoringPostId).First();
        //            //DeleteMonitoringPostMeasuredParameter(monitoringPostMeasuredParameters);
        //            _context.MonitoringPostMeasuredParameters.Remove(monitoringPostMeasuredParameters);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    return Ok(1);
        //}

        // POST: api/MonitoringPosts/EditMonitoringPostMeasuredParameter
        [HttpPost("editMonitoringPostMeasuredParameter")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<MonitoringPostMeasuredParameters>> EditMonitoringPostMeasuredParameter([FromBody] JObject content)
        {
            dynamic datas = content;
            int MonitoringPostId = datas.MonitoringPostId;
            int DataProviderId = datas.DataProviderId;
            int PollutionEnvironmentId = datas.PollutionEnvironmentId;
            string CultureName = datas.CultureName;
            List<int> MeasuredParametersId = datas.MeasuredParametersId.ToObject<List<int>>();
            List<string> Min = datas.Min.ToObject<List<string>>();
            List<string> Max = datas.Max.ToObject<List<string>>();
            List<string> MinMeasuredValue = datas.MinMeasuredValue.ToObject<List<string>>();
            List<string> MaxMeasuredValue = datas.MaxMeasuredValue.ToObject<List<string>>();
            List<string> Coefficient = datas.Coefficient.ToObject<List<string>>();

            List<int> idMeasuredParameters = new List<int>();
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            //Water
            if (PollutionEnvironmentId == 3)
            {
                measuredParameters = _context.MeasuredParameter.Where(m => m.PollutionEnvironmentId == 3).ToList();
            }
            else
            {
                //Kazhydromet
                if (DataProviderId == 1)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.KazhydrometCode != null).ToList();
                }
                //Ecoservice, Urus
                else if (DataProviderId == 3 || DataProviderId == 2)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.OceanusCode != null).ToList();
                }
                else
                {
                    measuredParameters = _context.MeasuredParameter.ToList();
                }
            }

            if (CultureName == "ru")
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameRU).Select(m => m.Id).ToList();
            }
            else if (CultureName == "kk")
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameKK).Select(m => m.Id).ToList();
            }
            else
            {
                idMeasuredParameters = measuredParameters.OrderBy(m => m.NameEN).Select(m => m.Id).ToList();
            }

            List<int> idMonitoringPostMeasuredParametersId = _context.MonitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId).OrderBy(m => m.MeasuredParameterId).Select(m => m.MeasuredParameterId).ToList();
            for (int i = 0; i < idMeasuredParameters.Count; i++)
            {
                foreach (var id in MeasuredParametersId)
                {
                    if (idMeasuredParameters[i] == id)
                    {
                        decimal? minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef;
                        if (Min[i] == "null")
                        {
                            minDec = null;
                        }
                        else
                        {
                            minDec = Decimal.Parse(Min[i], CultureInfo.InvariantCulture);
                        }
                        if (Max[i] == "null")
                        {
                            maxDec = null;
                        }
                        else
                        {
                            maxDec = Decimal.Parse(Max[i], CultureInfo.InvariantCulture);
                        }
                        if (MinMeasuredValue[i] == "null")
                        {
                            minMeasuredDec = null;
                        }
                        else
                        {
                            minMeasuredDec = Decimal.Parse(MinMeasuredValue[i], CultureInfo.InvariantCulture);
                        }
                        if (MaxMeasuredValue[i] == "null")
                        {
                            maxMeasuredDec = null;
                        }
                        else
                        {
                            maxMeasuredDec = Decimal.Parse(MaxMeasuredValue[i], CultureInfo.InvariantCulture);
                        }
                        if (Coefficient[i] == "null")
                        {
                            coef = null;
                        }
                        else
                        {
                            coef = Decimal.Parse(Coefficient[i], CultureInfo.InvariantCulture);
                        }

                        try
                        {
                            Task.WaitAll(PutMonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));
                        }
                        catch (Exception ex)
                        {
                            Task.WaitAll(MonitoringPostMeasuredParameter(MonitoringPostId, id, minDec, maxDec, minMeasuredDec, maxMeasuredDec, coef));
                        }
                    }
                }
            }
            bool check = false;
            foreach (var idAll in idMonitoringPostMeasuredParametersId)
            {
                foreach (var id in MeasuredParametersId)
                {
                    if (idAll == id)
                    {
                        check = true;
                        break;
                    }
                    else
                    {
                        check = false;
                    }
                }
                if (!check)
                {
                    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == idAll && m.MonitoringPostId == MonitoringPostId).First();
                    //DeleteMonitoringPostMeasuredParameter(monitoringPostMeasuredParameters);
                    _context.MonitoringPostMeasuredParameters.Remove(monitoringPostMeasuredParameters);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(1);
        }

        public async Task PutMonitoringPostMeasuredParameter(
            int MonitoringPostId,
            int MeasuredParameterId,
            decimal? Min,
            decimal? Max,
            decimal? MinMeasuredValue,
            decimal? MaxMeasuredValue,
            decimal? Coefficient)
        {
            var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId && m.MonitoringPostId == MonitoringPostId).First();
            monitoringPostMeasuredParameters.MonitoringPostId = MonitoringPostId;
            monitoringPostMeasuredParameters.MeasuredParameterId = MeasuredParameterId;
            monitoringPostMeasuredParameters.Min = Min;
            monitoringPostMeasuredParameters.Max = Max;
            monitoringPostMeasuredParameters.MinMeasuredValue = MinMeasuredValue;
            monitoringPostMeasuredParameters.MaxMeasuredValue = MaxMeasuredValue;
            monitoringPostMeasuredParameters.Coefficient = Coefficient;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMonitoringPostMeasuredParameter(
            MonitoringPostMeasuredParameters monitoringPostMeasuredParameters)
        {
            try
            {
                _context.MonitoringPostMeasuredParameters.Remove(monitoringPostMeasuredParameters);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }

        // GET: api/MonitoringPosts/GetMonitoringPostMeasuredParameter
        [HttpPost("getMonitoringPostMeasuredParameters")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Kazhydromet")]
        public List<MonitoringPostMeasuredParameters> GetMonitoringPostMeasuredParameters(
            int MonitoringPostId,
            int DataProviderId,
            int PollutionEnvironmentId)
        {
            List<MonitoringPostMeasuredParameters> monitoringPostMeasuredParameter = _context.MonitoringPostMeasuredParameters
                .Where(m => m.MonitoringPostId == MonitoringPostId)
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .OrderBy(m => m.MonitoringPostId)
                .ToList();
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();

            //Water
            if (PollutionEnvironmentId == 3)
            {
                measuredParameters = _context.MeasuredParameter.Where(m => m.PollutionEnvironmentId == 3).ToList();
            }
            else
            {
                //Kazhydromet
                if (DataProviderId == 1)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.KazhydrometCode != null).ToList();
                }
                //Ecoservice, Urus
                else if (DataProviderId == 3 || DataProviderId == 2)
                {
                    measuredParameters = _context.MeasuredParameter.Where(m => m.OceanusCode != null).ToList();
                }
                else
                {
                    measuredParameters = _context.MeasuredParameter.ToList();
                }
            }

            measuredParameters = measuredParameters.OrderBy(m => m.Id).ToList();

            List<MonitoringPostMeasuredParameters> monitoringPostMeasuredParameterWithNull = new List<MonitoringPostMeasuredParameters>();
            bool check = false;
            for (int i = 0; i < measuredParameters.Count; i++)
            {
                foreach (var id in monitoringPostMeasuredParameter)
                {
                    if (measuredParameters[i].Id == id.MeasuredParameterId)
                    {
                        var item = new MonitoringPostMeasuredParameters
                        {
                            Id = measuredParameters[i].Id,
                            MonitoringPostId = id.MonitoringPostId,
                            MeasuredParameterId = id.MeasuredParameterId,
                            MeasuredParameter = measuredParameters[i],
                            Min = id.Min,
                            Max = id.Max,
                            MinMeasuredValue = id.MinMeasuredValue,
                            MaxMeasuredValue = id.MaxMeasuredValue,
                            Coefficient = id.Coefficient,
                            Sensor = true
                        };
                        monitoringPostMeasuredParameterWithNull.Add(item);
                        check = true;
                    }
                }
                if (!check)
                {
                    var item = new MonitoringPostMeasuredParameters
                    {
                        Id = measuredParameters[i].Id,
                        MonitoringPostId = -1,
                        MeasuredParameterId = measuredParameters[i].Id,
                        MeasuredParameter = measuredParameters[i],
                        Min = null,
                        Max = null,
                        MinMeasuredValue = null,
                        MaxMeasuredValue = null,
                        Coefficient = null,
                        Sensor = false
                    };
                    monitoringPostMeasuredParameterWithNull.Add(item);
                }
                check = false;
            }
            return monitoringPostMeasuredParameterWithNull;
        }

        // GET: api/MonitoringPosts/GetMonitoringPostMeasuredParameterForMap
        [HttpPost("getMonitoringPostMeasuredParametersForMap")]
        //[Authorize(Roles = "admin,moderator,KaragandaRegion,Arys,Almaty,Kazhydromet")]
        public List<MonitoringPostMeasuredParameters> GetMonitoringPostMeasuredParametersForMap(int MonitoringPostId)
        {
            List<MonitoringPostMeasuredParameters> monitoringPostMeasuredParameter = _context.MonitoringPostMeasuredParameters
                .Where(m => m.MonitoringPostId == MonitoringPostId)
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MeasuredParameter.MeasuredParameterUnit)
                .Include(m => m.MonitoringPost)
                .OrderBy(m => m.MonitoringPostId)
                .ToList();
            return monitoringPostMeasuredParameter;
        }
    }
}

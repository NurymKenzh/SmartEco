using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LEDDisplayController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LEDDisplayController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost(nameof(GetLedDatas))]
        public async Task<JsonResult> GetLedDatas([FromBody] JObject content)
        {
            dynamic datas = content;
            DateTime DateTime = datas.DateTime;
            bool Averaged = datas.Averaged;
            string NamePosts = datas.NamePosts;

            var measuredDatas = await _context.MeasuredData
                .Include(m => m.MonitoringPost)
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MeasuredParameter.MeasuredParameterUnit)
                .Where(m => m.DateTime != null && m.DateTime > DateTime && m.Averaged == Averaged && m.MonitoringPost.Name.Contains(NamePosts) && m.MeasuredParameter.MPCMaxSingle != null)
                .OrderByDescending(m => m.DateTime)
                .ToListAsync();

            var mpmp = await _context.MonitoringPostMeasuredParameters
                .Include(m => m.MonitoringPost)
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MeasuredParameter.MeasuredParameterUnit)
                .Where(m => m.MonitoringPost.Name.Contains(NamePosts) && m.MeasuredParameter.MPCMaxSingle != null)
                .ToListAsync();

            var measuredParameters = mpmp
                .GroupBy(m => m.MeasuredParameterId)
                .Select(m => m.First().MeasuredParameter)
                .OrderBy(m => m.NameRU)
                .ToList();

            var monitoringPosts = await _context.MonitoringPost
                .Where(m => m.Name.Contains(NamePosts))
                .ToListAsync();

            return new JsonResult(
                new
                {
                    MeasuredDatas = measuredDatas,
                    MonitoringPostMeasuredParameters = mpmp,
                    MeasuredParameters = measuredParameters,
                    MonitoringPosts = monitoringPosts
                });
        }
    }
}

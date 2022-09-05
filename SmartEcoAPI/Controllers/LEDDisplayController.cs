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
            try
            {
                dynamic datas = content;
                DateTime DateTime = datas.DateTime;
                bool Averaged = datas.Averaged;
                string NamePosts = datas.NamePosts;
                int? DataProviderId = datas.DataProviderId;
                int? ProjectId = datas.ProjectId;

                Func<MonitoringPost, bool> expression;

                if (ProjectId is null)
                    expression = CreateConditionalByName(NamePosts);
                else
                    expression = CreateConditionalByProject((int)DataProviderId, (int)ProjectId);

                var monitoringPosts = _context.MonitoringPost
                    .Where(expression)
                    .ToList();
                var monitoringPostsId = monitoringPosts.Select(mp => mp.Id).ToList();

                var measuredDatas = await _context.MeasuredData
                    .Include(m => m.MonitoringPost)
                    .Include(m => m.MeasuredParameter)
                    .Include(m => m.MeasuredParameter.MeasuredParameterUnit)
                    .Where(m => m.DateTime != null && m.DateTime > DateTime && m.Averaged == Averaged && m.MeasuredParameter.MPCMaxSingle != null)
                    .Where(m => monitoringPostsId.Contains(m.MonitoringPost.Id))
                    .OrderByDescending(m => m.DateTime)
                    .ToListAsync();

                var mpmp = await _context.MonitoringPostMeasuredParameters
                    .Include(m => m.MonitoringPost)
                    .Include(m => m.MeasuredParameter)
                    .Include(m => m.MeasuredParameter.MeasuredParameterUnit)
                    .Where(m => m.MeasuredParameter.MPCMaxSingle != null)
                    .Where(m => monitoringPostsId.Contains(m.MonitoringPostId))
                    .ToListAsync();

                var measuredParameters = mpmp
                    .GroupBy(m => m.MeasuredParameterId)
                    .Select(m => m.First().MeasuredParameter)
                    .OrderBy(m => m.NameRU)
                    .ToList();

                return new JsonResult(
                    new
                    {
                        MeasuredDatas = measuredDatas,
                        MonitoringPostMeasuredParameters = mpmp,
                        MeasuredParameters = measuredParameters,
                        MonitoringPosts = monitoringPosts
                    });
            }
            catch (Exception ex)
            {
                return new JsonResult(
                    new
                    {
                        MeasuredDatas = new List<MeasuredData>(),
                        MonitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameters>(),
                        MeasuredParameters = new List<MeasuredParameter>(),
                        MonitoringPosts = new List<MonitoringPost>()
                    });
            }
        }

        private Func<MonitoringPost, bool> CreateConditionalByProject(int DataProviderId, int ProjectId)
            => m => m.DataProviderId == DataProviderId && m.ProjectId == ProjectId;

        private Func<MonitoringPost, bool> CreateConditionalByName(string NamePosts)
            => m => m.Name.Contains(NamePosts);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Akimato.Data;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AActivityExecutorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AActivityExecutorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AActivityExecutors
        [HttpGet]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task<ActionResult<IEnumerable<AActivityExecutor>>> GetAActivityExecutors()
        {
            var aActivityExecutors = _context.AActivityExecutor
               .Include(a => a.AActivity)
               .Include(t => t.Executor)
               .Where(k => true);

            return await aActivityExecutors.ToListAsync();
        }

        // POST: api/AActivities/SetRelated
        [HttpPost("SetRelated")]
        [Authorize(Roles = "admin,moderator,Almaty,Shymkent")]
        public async Task SetRelated(
            int AActivityId,
            [FromQuery(Name = "idExecutors")] List<int> idExecutors,
            [FromQuery(Name = "Contribution")] List<decimal> Contribution)
        {
            var aActivityExecutors = _context.AActivityExecutor
                .Where(a => a.AActivityId == AActivityId)
                .ToList();
            foreach (var aActivityExecutor in aActivityExecutors)
            {
                //Delete
                if (!idExecutors.Contains(aActivityExecutor.Id))
                {
                    _context.Remove(aActivityExecutor);
                }
                //Edit
                else
                {
                    var index = idExecutors.IndexOf(aActivityExecutor.Id);
                    aActivityExecutor.Contribution = Contribution[index];
                }
            }
            //Create
            foreach (var idExecutor in idExecutors)
            {
                if (!aActivityExecutors.Select(a => a.Id).Contains(idExecutor))
                {
                    AActivityExecutor activityExecutor = new AActivityExecutor
                    {
                        AActivityId = AActivityId,
                        ExecutorId = idExecutor,
                        Contribution = Contribution[idExecutors.IndexOf(idExecutor)]
                    };
                    _context.Add(activityExecutor);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringPostsController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;

        //public MonitoringPostsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: MonitoringPosts
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.MonitoringPost.Include(m => m.DataProvider).Include(m => m.PollutionEnvironment);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        //// GET: MonitoringPosts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var monitoringPost = await _context.MonitoringPost
        //        .Include(m => m.DataProvider)
        //        .Include(m => m.PollutionEnvironment)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (monitoringPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(monitoringPost);
        //}

        //// GET: MonitoringPosts/Create
        //public IActionResult Create()
        //{
        //    ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id");
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id");
        //    return View();
        //}

        //// POST: MonitoringPosts/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(monitoringPost);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //    return View(monitoringPost);
        //}

        //// GET: MonitoringPosts/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var monitoringPost = await _context.MonitoringPost.FindAsync(id);
        //    if (monitoringPost == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //    return View(monitoringPost);
        //}

        //// POST: MonitoringPosts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost)
        //{
        //    if (id != monitoringPost.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(monitoringPost);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MonitoringPostExists(monitoringPost.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //    return View(monitoringPost);
        //}

        //// GET: MonitoringPosts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var monitoringPost = await _context.MonitoringPost
        //        .Include(m => m.DataProvider)
        //        .Include(m => m.PollutionEnvironment)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (monitoringPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(monitoringPost);
        //}

        //// POST: MonitoringPosts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var monitoringPost = await _context.MonitoringPost.FindAsync(id);
        //    _context.MonitoringPost.Remove(monitoringPost);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool MonitoringPostExists(int id)
        //{
        //    return _context.MonitoringPost.Any(e => e.Id == id);
        //}

        private readonly ApplicationDbContext _context;

        public MonitoringPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MonitoringPosts
        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetMonitoringPost(string SortOrder,
            int? Number,
            string Name,
            int? DataProviderId,
            int? PollutionEnvironmentId,
            int? PageSize,
            int? PageNumber)
        {
            var monitoringPosts = _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
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
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
        public async Task<ActionResult<MonitoringPost>> GetMonitoringPost(long id)
        {
            //var monitoringPost = await _context.MonitoringPost.FindAsync(id);
            var monitoringPost = await _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitoringPost == null)
            {
                return NotFound();
            }

            _context.MonitoringPost.Remove(monitoringPost);
            await _context.SaveChangesAsync();

            return monitoringPost;
        }

        private bool MonitoringPostExists(int id)
        {
            return _context.MonitoringPost.Any(e => e.Id == id);
        }

        // GET: api/MonitoringPosts/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetMonitoringPostsCount(int? Number,
            string Name,
            int? DataProviderId,
            int? PollutionEnvironmentId)
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

            int count = await monitoringPosts.CountAsync();

            return Ok(count);
        }

        // GET: api/MonitoringPosts/Exceed
        [HttpGet("exceed")]
        [Authorize(Roles = "admin,moderator,KaragandaRegion")]
        public async Task<ActionResult<IEnumerable<MonitoringPost>>> GetEcoserviceMonitoringPostsExceed(int MPCExceedPastMinutes,
            int? DataProviderId)
        {
            // populate data (delete)
            MeasuredDatasController measuredDatasController = new MeasuredDatasController(_context);
            measuredDatasController.PopulateEcoserviceData();

            DateTime minExceedDateTime = DateTime.Now.AddMinutes(-MPCExceedPastMinutes);

            var monitoringPosts = _context.MonitoringPost
                .Include(m => m.DataProvider)
                .Include(m => m.PollutionEnvironment)
                .Where(m => (m.DataProviderId == (int)DataProviderId) || DataProviderId == null);

            foreach (MonitoringPost monitoringPost in monitoringPosts)
            {
                bool exceed = _context.MeasuredData
                    .Where(m => m.MonitoringPostId == monitoringPost.Id
                        && m.DateTime >= minExceedDateTime)
                    .Include(m => m.MeasuredParameter)
                    .Where(m => m.Value > m.MeasuredParameter.MPC && m.MeasuredParameter.MPC != null)
                    .FirstOrDefault() != null;
                if (!exceed)
                {
                    monitoringPosts = monitoringPosts
                        .Where(m => m.Id != monitoringPost.Id);
                }
            }

            return await monitoringPosts.ToListAsync();
        }
    }
}

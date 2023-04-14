using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
{
    public class PollutionSourceDatasController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PollutionSourceDatasController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PollutionSourceDatas
        public async Task<IActionResult> Index(string SortOrder,
            int? PollutantIdFilter,
            int? PollutionSourceIdFilter,
            DateTime? DateTimeFromFilter,
            DateTime? DateTimeToFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PollutionSourceData> pollutionSourceDatas = new List<PollutionSourceData>();

            ViewBag.PollutantIdFilter = PollutantIdFilter;
            ViewBag.PollutionSourceIdFilter = PollutionSourceIdFilter;
            ViewBag.DateTimeFromFilter = (DateTimeFromFilter)?.ToString("yyyy-MM-dd");
            ViewBag.DateTimeToFilter = (DateTimeToFilter)?.ToString("yyyy-MM-dd");

            ViewBag.PollutantSort = SortOrder == "Pollutant" ? "PollutantDesc" : "Pollutant";
            ViewBag.PollutionSourceSort = SortOrder == "PollutionSource" ? "PollutionSourceDesc" : "PollutionSource";
            ViewBag.DateTimeSort = SortOrder == "DateTime" ? "DateTimeDesc" : "DateTime";

            string url = "api/PollutionSourceDatas",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            // Language
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Language={new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name}";
            }
            if (PollutantIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutantId={PollutantIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutantId={PollutantIdFilter}";
            }
            if (PollutionSourceIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionSourceId={PollutionSourceIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutionSourceId={PollutionSourceIdFilter}";
            }
            if (DateTimeFromFilter != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={DateTimeFromFilter?.ToString(dateTimeFormatInfo)}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"DateTimeFrom={DateTimeFromFilter?.ToString(dateTimeFormatInfo)}";
            }
            if (DateTimeToFilter != null)
            {
                DateTimeToFilter = DateTimeToFilter?.AddDays(1).AddMilliseconds(-1);
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={DateTimeToFilter?.ToString(dateTimeFormatInfo)}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"DateTimeTo={DateTimeToFilter?.ToString(dateTimeFormatInfo)}";
            }
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);
            ViewBag.PageSizeList = new SelectList(pageSizeList.OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), "Key", "Value");
            if (PageSize == null)
            {
                PageSize = Convert.ToInt32(pageSizeList.Min(p => p.Value));
            }
            if (PageSize == 0)
            {
                PageSize = null;
            }
            if (PageSize != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageSize={PageSize.ToString()}";
                if (PageNumber == null)
                {
                    PageNumber = 1;
                }
            }
            if (PageNumber != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageNumber={PageNumber.ToString()}";
            }
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route),
                responseCount = await _HttpApiClient.GetAsync(url + "/count" + routeCount);
            if (response.IsSuccessStatusCode)
            {
                pollutionSourceDatas = await response.Content.ReadAsAsync<List<PollutionSourceData>>();
            }
            int pollutionSourceDatasCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                pollutionSourceDatasCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(pollutionSourceDatasCount / (decimal)PageSize) : 1;
            ViewBag.StartPage = PageNumber - 5;
            ViewBag.EndPage = PageNumber + 4;
            if (ViewBag.StartPage <= 0)
            {
                ViewBag.EndPage -= (ViewBag.StartPage - 1);
                ViewBag.StartPage = 1;
            }
            if (ViewBag.EndPage > ViewBag.TotalPages)
            {
                ViewBag.EndPage = ViewBag.TotalPages;
                if (ViewBag.EndPage > 10)
                {
                    ViewBag.StartPage = ViewBag.EndPage - 9;
                }
            }

            List<Pollutant> pollutants = new List<Pollutant>();
            string urlPollutants = "api/Pollutants",
                routePollutants = "";
            HttpResponseMessage responsePollutants = await _HttpApiClient.GetAsync(urlPollutants + routePollutants);
            if (responsePollutants.IsSuccessStatusCode)
            {
                pollutants = await responsePollutants.Content.ReadAsAsync<List<Pollutant>>();
            }
            ViewBag.Pollutants = new SelectList(pollutants.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionSource> pollutionSources = new List<PollutionSource>();
            string urlPollutionSources = "api/PollutionSources",
                routePollutionSources = "";
            HttpResponseMessage responsePollutionSources = await _HttpApiClient.GetAsync(urlPollutionSources + routePollutionSources);
            if (responsePollutionSources.IsSuccessStatusCode)
            {
                pollutionSources = await responsePollutionSources.Content.ReadAsAsync<List<PollutionSource>>();
            }
            ViewBag.PollutionSources = new SelectList(pollutionSources.OrderBy(m => m.Name), "Id", "Name");

            return View(pollutionSourceDatas);
        }

        // GET: PollutionSourceDatas/Details/5
        public async Task<IActionResult> Details(long? id,
            string SortOrder,
            int? PollutantIdFilter,
            int? PollutionSourceIdFilter,
            DateTime? DateTimeFromFilter,
            DateTime? DateTimeToFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutantIdFilter = PollutantIdFilter;
            ViewBag.PollutionSourceIdFilter = PollutionSourceIdFilter;
            ViewBag.DateTimeFromFilter = DateTimeFromFilter;
            ViewBag.DateTimeToFilter = DateTimeToFilter;
            if (id == null)
            {
                return NotFound();
            }

            PollutionSourceData pollutionSourceData = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionSourceDatas/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionSourceData = await response.Content.ReadAsAsync<PollutionSourceData>();
            }
            if (pollutionSourceData == null)
            {
                return NotFound();
            }

            return View(pollutionSourceData);
        }

        //// GET: PollutionSourceDatas/Create
        //public IActionResult Create()
        //{
        //    ViewData["PollutantId"] = new SelectList(_context.Pollutant, "Id", "Id");
        //    return View();
        //}

        //// POST: PollutionSourceDatas/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,PollutantId,PollutionSourceId,DateTime,Value")] PollutionSourceData pollutionSourceData)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(pollutionSourceData);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PollutantId"] = new SelectList(_context.Pollutant, "Id", "Id", pollutionSourceData.PollutantId);
        //    return View(pollutionSourceData);
        //}

        //// GET: PollutionSourceDatas/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pollutionSourceData = await _context.PollutionSourceData.FindAsync(id);
        //    if (pollutionSourceData == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PollutantId"] = new SelectList(_context.Pollutant, "Id", "Id", pollutionSourceData.PollutantId);
        //    return View(pollutionSourceData);
        //}

        //// POST: PollutionSourceDatas/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,PollutantId,PollutionSourceId,DateTime,Value")] PollutionSourceData pollutionSourceData)
        //{
        //    if (id != pollutionSourceData.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(pollutionSourceData);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PollutionSourceDataExists(pollutionSourceData.Id))
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
        //    ViewData["PollutantId"] = new SelectList(_context.Pollutant, "Id", "Id", pollutionSourceData.PollutantId);
        //    return View(pollutionSourceData);
        //}

        //// GET: PollutionSourceDatas/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pollutionSourceData = await _context.PollutionSourceData
        //        .Include(p => p.Pollutant)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (pollutionSourceData == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pollutionSourceData);
        //}

        //// POST: PollutionSourceDatas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var pollutionSourceData = await _context.PollutionSourceData.FindAsync(id);
        //    _context.PollutionSourceData.Remove(pollutionSourceData);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PollutionSourceDataExists(long id)
        //{
        //    return _context.PollutionSourceData.Any(e => e.Id == id);
        //}
    }
}

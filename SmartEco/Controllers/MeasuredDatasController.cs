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
using Newtonsoft.Json;
using SmartEco.Data;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class MeasuredDatasController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public MeasuredDatasController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: MeasuredDatas
        public async Task<IActionResult> Index(string SortOrder,
            int? MeasuredParameterIdFilter,
            DateTime? DateTimeFromFilter,
            DateTime? DateTimeToFilter,
            int? MonitoringPostIdFilter,
            int? PollutionSourceIdFilter,
            bool? AveragedFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();

            ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
            ViewBag.DateTimeFromFilter = (DateTimeFromFilter)?.ToString("yyyy-MM-dd");
            ViewBag.DateTimeToFilter = (DateTimeToFilter)?.ToString("yyyy-MM-dd");
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            ViewBag.PollutionSourceIdFilter = PollutionSourceIdFilter;
            ViewBag.AveragedFilter = AveragedFilter;

            ViewBag.MeasuredParameterSort = SortOrder == "MeasuredParameter" ? "MeasuredParameterDesc" : "MeasuredParameter";
            ViewBag.DateTimeSort = SortOrder == "DateTime" ? "DateTimeDesc" : "DateTime";
            ViewBag.MonitoringPostSort = SortOrder == "MonitoringPost" ? "MonitoringPostDesc" : "MonitoringPost";
            ViewBag.PollutionSourceSort = SortOrder == "PollutionSource" ? "PollutionSourceDesc" : "PollutionSource";

            string url = "api/MeasuredDatas",
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
            if (MeasuredParameterIdFilter!=null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"MeasuredParameterId={MeasuredParameterIdFilter}";
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
            if (MonitoringPostIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringPostId={MonitoringPostIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"MonitoringPostId={MonitoringPostIdFilter}";
            }
            if (PollutionSourceIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionSourceId={PollutionSourceIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutionSourceId={PollutionSourceIdFilter}";
            }
            if (AveragedFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Averaged={AveragedFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Averaged={AveragedFilter}";
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
                measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();

                string csv = "";
                foreach(MeasuredData measuredData in measuredDatas)
                {
                    csv += measuredData.MonitoringPost.NorthLatitude + ";" + 
                        measuredData.MonitoringPost.EastLongitude + ";" +
                        measuredData.Value + ";" + 
                        measuredData.DateTime.ToString() + 
                        Environment.NewLine;
                }

            }
            int measuredDatasCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                measuredDatasCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(measuredDatasCount / (decimal)PageSize) : 1;
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

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = new SelectList(measuredParameters.OrderBy(m => m.Name), "Id", "Name");

            List<EcomonMonitoringPoint> ecomonMonitoringPoints = new List<EcomonMonitoringPoint>();
            string urlEcomonMonitoringPoints = "api/EcomonMonitoringPoints",
                routeEcomonMonitoringPoints = "";
            HttpResponseMessage responseEcomonMonitoringPoints = await _HttpApiClient.GetAsync(urlEcomonMonitoringPoints + routeEcomonMonitoringPoints);
            if (responseEcomonMonitoringPoints.IsSuccessStatusCode)
            {
                ecomonMonitoringPoints = await responseEcomonMonitoringPoints.Content.ReadAsAsync<List<EcomonMonitoringPoint>>();
            }
            ViewBag.EcomonMonitoringPoints = new SelectList(ecomonMonitoringPoints.OrderBy(m => m.Number), "Id", "Number");

            List<KazHydrometAirPost> kazHydrometAirPosts = new List<KazHydrometAirPost>();
            string urlKazHydrometAirPosts = "api/KazHydrometAirPosts",
                routeKazHydrometAirPosts = "";
            HttpResponseMessage responseKazHydrometAirPosts = await _HttpApiClient.GetAsync(urlKazHydrometAirPosts + routeKazHydrometAirPosts);
            if (responseKazHydrometAirPosts.IsSuccessStatusCode)
            {
                kazHydrometAirPosts = await responseKazHydrometAirPosts.Content.ReadAsAsync<List<KazHydrometAirPost>>();
            }
            ViewBag.KazHydrometAirPosts = new SelectList(kazHydrometAirPosts.OrderBy(m => m.Name), "Id", "Name");

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionSource> pollutionSources = new List<PollutionSource>();
            string urlPollutionSources = "api/PollutionSources",
                routePollutionSources = "";
            HttpResponseMessage responsePollutionSources = await _HttpApiClient.GetAsync(urlPollutionSources + routePollutionSources);
            if (responsePollutionSources.IsSuccessStatusCode)
            {
                pollutionSources = await responsePollutionSources.Content.ReadAsAsync<List<PollutionSource>>();
            }
            ViewBag.PollutionSources = new SelectList(pollutionSources.OrderBy(m => m.Name), "Id", "Name");

            return View(measuredDatas);
        }

        // GET: MeasuredDatas/Details/5
        public async Task<IActionResult> Details(long? id,
            string SortOrder,
            int? MeasuredParameterIdFilter,
            DateTime? DateTimeFromFilter,
            DateTime? DateTimeToFilter,
            int? MonitoringPostIdFilter,
            int? PollutionSourceIdFilter,
            bool? AveragedFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
            ViewBag.DateTimeFromFilter = DateTimeFromFilter;
            ViewBag.DateTimeToFilter = DateTimeToFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            ViewBag.PollutionSourceIdFilter = PollutionSourceIdFilter;
            ViewBag.AveragedFilter = AveragedFilter;
            if (id == null)
            {
                return NotFound();
            }

            MeasuredData measuredData = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MeasuredDatas/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                measuredData = await response.Content.ReadAsAsync<MeasuredData>();
            }
            if (measuredData == null)
            {
                return NotFound();
            }

            return View(measuredData);
        }

        //// GET: MeasuredDatas/Create
        //public IActionResult Create(string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    ViewData["EcomonMonitoringPointId"] = new SelectList(_context.Set<EcomonMonitoringPoint>(), "Id", "Id");
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id");
        //    return View();
        //}

        //// POST: MeasuredDatas/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,MeasuredParameterId,DateTime,Value,EcomonMonitoringPointId,Ecomontimestamp_ms")] MeasuredData measuredData,
        //    string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(measuredData);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["EcomonMonitoringPointId"] = new SelectList(_context.Set<EcomonMonitoringPoint>(), "Id", "Id", measuredData.EcomonMonitoringPointId);
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", measuredData.MeasuredParameterId);
        //    return View(measuredData);
        //}

        //// GET: MeasuredDatas/Edit/5
        //public async Task<IActionResult> Edit(long? id,
        //    string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var measuredData = await _context.MeasuredData.FindAsync(id);
        //    if (measuredData == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["EcomonMonitoringPointId"] = new SelectList(_context.Set<EcomonMonitoringPoint>(), "Id", "Id", measuredData.EcomonMonitoringPointId);
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", measuredData.MeasuredParameterId);
        //    return View(measuredData);
        //}

        //// POST: MeasuredDatas/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,MeasuredParameterId,DateTime,Value,EcomonMonitoringPointId,Ecomontimestamp_ms")] MeasuredData measuredData,
        //    string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    if (id != measuredData.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(measuredData);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MeasuredDataExists(measuredData.Id))
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
        //    ViewData["EcomonMonitoringPointId"] = new SelectList(_context.Set<EcomonMonitoringPoint>(), "Id", "Id", measuredData.EcomonMonitoringPointId);
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", measuredData.MeasuredParameterId);
        //    return View(measuredData);
        //}

        //// GET: MeasuredDatas/Delete/5
        //public async Task<IActionResult> Delete(long? id,
        //    string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var measuredData = await _context.MeasuredData
        //        .Include(m => m.EcomonMonitoringPoint)
        //        .Include(m => m.MeasuredParameter)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (measuredData == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(measuredData);
        //}

        //// POST: MeasuredDatas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id,
        //    string SortOrder,
        //    int? MeasuredParameterIdFilter,
        //    DateTime? DateTimeFromFilter,
        //    DateTime? DateTimeToFilter,
        //    int? PageSize,
        //    int? PageNumber)
        //{
        //    ViewBag.SortOrder = SortOrder;
        //    ViewBag.PageSize = PageSize;
        //    ViewBag.PageNumber = PageNumber;
        //    ViewBag.MeasuredParameterIdFilter = MeasuredParameterIdFilter;
        //    ViewBag.DateTimeFromFilter = DateTimeFromFilter;
        //    ViewBag.DateTimeToFilter = DateTimeToFilter;
        //    var measuredData = await _context.MeasuredData.FindAsync(id);
        //    _context.MeasuredData.Remove(measuredData);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool MeasuredDataExists(long id)
        //{
        //    return _context.MeasuredData.Any(e => e.Id == id);
        //}

        [HttpPost]
        public async Task<IActionResult> GetMeasuredDatas(
            int? MonitoringPostId,
            int? MeasuredParameterId,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            List<MeasuredData> measureddatas = new List<MeasuredData>();
            List<MeasuredData> measuredDatasDailyAverage = new List<MeasuredData>();
            List<MeasuredData> measureddatasDailyAverage = new List<MeasuredData>();
            decimal? dailyAverage = null;
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            string url = "api/MeasuredDatas",
                route = "",
                routeDailyAverage = "";
            // First condition - for select chart and table. Second condition - for Analytics
            if (MeasuredParameterId != null || (MonitoringPostId == null && MeasuredParameterId == null))
            {
                // SortOrder=DateTime
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"SortOrder=DateTime";

                    routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                    routeDailyAverage += $"SortOrder=DateTime";
                }
                // MonitoringPostId
                if (MonitoringPostId != null)
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MonitoringPostId={MonitoringPostId}";

                    routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                    routeDailyAverage += $"MonitoringPostId={MonitoringPostId}";
                }
                // MeasuredParameterId
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterId}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"MeasuredParameterId={MeasuredParameterId}";
                // AveragedFilter
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"Averaged={Averaged}";

                    routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                    routeDailyAverage += $"Averaged=false";
                }
                // dateTimeFrom
                {
                    DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";

                    routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                    routeDailyAverage += $"DateTimeFrom={DateTime.Now.AddDays(-1).ToString(dateTimeFormatInfo)}";
                }
                // dateTimeTo
                {
                    DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";

                    routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                    routeDailyAverage += $"DateTimeTo={DateTime.Now.ToString(dateTimeFormatInfo)}";
                }
                HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
                if (response.IsSuccessStatusCode)
                {
                    measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measureddatas = measuredDatas.OrderByDescending(m => m.DateTime).ToList();

                HttpResponseMessage responseDailyAverage = await _HttpApiClient.GetAsync(url + routeDailyAverage);
                if (responseDailyAverage.IsSuccessStatusCode)
                {
                    measuredDatasDailyAverage = await responseDailyAverage.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measureddatasDailyAverage = measuredDatasDailyAverage.OrderByDescending(m => m.DateTime).ToList();

                if (measureddatasDailyAverage.Count != 0)
                {
                    dailyAverage = measureddatasDailyAverage.Sum(m => m.Value) / measureddatasDailyAverage.Count();
                }
            }
            return Json(new
            {
                measureddatas,
                dailyAverage
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetMeasuredDatasPollutionSource(
            int? PollutionSourceId,
            int? MeasuredParameterId,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            List<MeasuredData> measureddatas = new List<MeasuredData>();
            List<MeasuredData> measuredDatasDailyAverage = new List<MeasuredData>();
            List<MeasuredData> measureddatasDailyAverage = new List<MeasuredData>();
            decimal? dailyAverage = null;
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            string url = "api/MeasuredDatas/pollutionSource",
                route = "",
                routeDailyAverage = "";
            // SortOrder=DateTime
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder=DateTime";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"SortOrder=DateTime";
            }
            // PollutionSourceId
            if (PollutionSourceId != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionSourceId={PollutionSourceId}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"PollutionSourceId={PollutionSourceId}";
            }
            // MeasuredParameterId
            if (MeasuredParameterId != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterId}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"MeasuredParameterId={MeasuredParameterId}";
            }
            // AveragedFilter
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Averaged={Averaged}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"Averaged={Averaged}";
            }
            // dateTimeFrom
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"DateTimeFrom={DateTime.Now.AddDays(-1).ToString(dateTimeFormatInfo)}";
            }
            // dateTimeTo
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";

                routeDailyAverage += string.IsNullOrEmpty(routeDailyAverage) ? "?" : "&";
                routeDailyAverage += $"DateTimeTo={DateTime.Now.ToString(dateTimeFormatInfo)}";
            }
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
            }
            measureddatas = measuredDatas.OrderByDescending(m => m.DateTime).ToList();

            // For select chart
            if (MeasuredParameterId != null)
            {
                HttpResponseMessage responseDailyAverage = await _HttpApiClient.GetAsync(url + routeDailyAverage);
                if (responseDailyAverage.IsSuccessStatusCode)
                {
                    measuredDatasDailyAverage = await responseDailyAverage.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measureddatasDailyAverage = measuredDatasDailyAverage.OrderByDescending(m => m.DateTime).ToList();

                if (measureddatasDailyAverage.Count != 0)
                {
                    dailyAverage = measureddatasDailyAverage.Sum(m => m.Value) / measureddatasDailyAverage.Count();
                }
            }
            // For select table
            else
            {
                var measuredParameters = measureddatas
                    .Where(m => m.MeasuredParameterId != 5 && m.MeasuredParameterId != 6) //Скор. ветра, Напр. ветра
                    .Select(m => new { m.MeasuredParameter.Id, m.MeasuredParameter.Name })
                    .Distinct()
                    .ToList();
                var dateTimes = measureddatas
                    .Select(m => new { m.DateTime })
                    .Distinct()
                    .ToList();
                string[,] array = new string[dateTimes.Count, measuredParameters.Count + 1];

                for (int i = 0; i < dateTimes.Count; i++)
                {
                    var measuredData = measureddatas.Where(m => m.DateTime == dateTimes[i].DateTime);
                    array[i, 0] = dateTimes[i].DateTime.ToString();
                    for (int j = 0; j < measuredParameters.Count; j++)
                    {
                        var data = measuredData.Where(m => m.MeasuredParameterId == measuredParameters[j].Id).FirstOrDefault();
                        if(data != null)
                        {
                            array[i, j + 1] = Math.Round(Convert.ToDecimal(data.Value), 3).ToString();
                        }
                        else
                        {
                            array[i, j + 1] = "";
                        }
                    }
                }

                return Json(new
                {
                    array,
                    measuredParameters
                });
            }
            return Json(new
            {
                measureddatas,
                dailyAverage
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetMeasuredDatasAnalytic(
            int? MonitoringPostId,
            int? MeasuredParameterId,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            List<MeasuredData> measureddatas = new List<MeasuredData>();
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            string url = "api/MeasuredDatas",
                route = "";
            // First condition - for select chart and table. Second condition - for Analytics
            if (MeasuredParameterId != null || (MonitoringPostId == null && MeasuredParameterId == null))
            {
                // SortOrder=DateTime
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"SortOrder=DateTime";
                }
                // MonitoringPostId
                if (MonitoringPostId != null)
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MonitoringPostId={MonitoringPostId}";
                }
                // MeasuredParameterId
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterId}";
                // AveragedFilter
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"Averaged={Averaged}";
                }
                // dateTimeFrom
                {
                    DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";
                }
                // dateTimeTo
                {
                    DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";
                }
                HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
                if (response.IsSuccessStatusCode)
                {
                    measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measureddatas = measuredDatas.OrderByDescending(m => m.DateTime).ToList();
            }
            return Json(new
            {
                measureddatas
            });
        }
    }
}

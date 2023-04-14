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
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
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
        public async Task<(List<MeasuredData>, decimal?)> GetPostMeasuredDatas(
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
                var measuredData = measuredDatas.FirstOrDefault();
                if (measuredData != null)
                {
                    //Water
                    if (measuredData.MonitoringPost.PollutionEnvironmentId == 3)
                    {
                        measuredDatas = measuredDatas
                            .Select(m => new MeasuredData
                            {
                                Averaged = m.Averaged,
                                DateTime = Convert.ToDateTime(m.DateTimeOrYearMonth),
                                Id = m.Id,
                                MeasuredParameter = m.MeasuredParameter,
                                MeasuredParameterId = m.MeasuredParameterId,
                                MonitoringPost = m.MonitoringPost,
                                MonitoringPostId = m.MonitoringPostId,
                                Month = m.Month,
                                Value = m.Value,
                                Year = m.Year
                            })
                            .ToList();
                    }
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
            return (measureddatas, dailyAverage);
        }

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
            var data = await GetPostMeasuredDatas(MonitoringPostId, MeasuredParameterId, DateFrom, TimeFrom, DateTo, TimeTo, Averaged);

            List<MeasuredData> measureddatas = data.Item1;
            decimal? dailyAverage = data.Item2;
            
            return Json(new
            {
                measureddatas,
                dailyAverage
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetSeveralMeasuredDatas(
            int? MonitoringPostId,
            List<int> MeasuredParameterIds,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measureddatas = new List<MeasuredData>();
            List<decimal?> dailyAverage = new List<decimal?>();
            foreach (int MeasuredParameterId in MeasuredParameterIds)
            {
                var data = await GetPostMeasuredDatas(MonitoringPostId, MeasuredParameterId, DateFrom, TimeFrom, DateTo, TimeTo, Averaged);
                measureddatas.AddRange(data.Item1);
                dailyAverage.Add(data.Item2);
            }
            measureddatas = measureddatas.OrderByDescending(m => m.DateTime).ToList();
            return Json(new
            {
                measureddatas,
                dailyAverage
            });
        }

        public class PollutionSourceTable
        {
            public DateTime? dateTime;
            public decimal? volumetricFlow;
            public decimal? sulphurDioxide;
            public decimal? flow;
            public decimal? mpc;
            public decimal? excess;
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
                List<PollutionSourceTable> pollutionSourceTables = new List<PollutionSourceTable>();

                var measuredParameters = measureddatas
                    //.Where(m => m.MeasuredParameterId != 5 && m.MeasuredParameterId != 6) //Скор. ветра, Напр. ветра
                    .Where(m => m.MeasuredParameterId == 9 || m.MeasuredParameterId == 25) //Диоксид серы, Расход
                    .OrderBy(m => m.MeasuredParameterId)
                    .Select(m => new { m.MeasuredParameter.Id, m.MeasuredParameter.Name })
                    .Distinct()
                    .ToList();
                var dateTimes = measureddatas
                    .Select(m => new { m.DateTime })
                    .Distinct()
                    .ToList();
                string[,] array = new string[dateTimes.Count, measuredParameters.Count + 4];

                //for (int i = 0; i < dateTimes.Count; i++)
                //{
                //    var measuredData = measureddatas.Where(m => m.DateTime == dateTimes[i].DateTime);
                //    array[i, 0] = dateTimes[i].DateTime.ToString();
                //    array[i, 1] = "99.805"; //V - Объемный расход м3/с
                //    for (int j = 0; j < measuredParameters.Count; j++)
                //    {
                //        var data = measuredData.Where(m => m.MeasuredParameterId == measuredParameters[j].Id).FirstOrDefault();
                //        if(data != null)
                //        {
                //            array[i, j + 2] = Math.Round(Convert.ToDecimal(data.Value), 3).ToString();
                //        }
                //        else
                //        {
                //            array[i, j + 2] = "";
                //        }
                //    }
                //    array[i, 4] = measureddatas.Where(m => m.MeasuredParameterId == 25).FirstOrDefault().MeasuredParameter.MPCMaxSingle.ToString(); //ПДКм.р. г/с
                //    if (array[i, 3] == "")
                //    {
                //        array[i, 5] = "-";
                //    }
                //    else if (Convert.ToDecimal(array[i, 3]) > Convert.ToDecimal(array[i, 4]))
                //    {
                //        array[i, 5] = (Convert.ToDecimal(array[i, 3]) - Convert.ToDecimal(array[i, 4])).ToString(); //Превышение г/с
                //    }
                //    else
                //    {
                //        array[i, 5] = "-";
                //    }
                //}
                //var header = new List<string>() { "Дата и время", "Объемный расход" };
                //header.AddRange(measuredParameters.Select(m => m.Name));
                //header.AddRange(new List<string>() { "ПДКм.р.", "Превышение" });

                for (int i = 0; i < dateTimes.Count; i++)
                {
                    PollutionSourceTable pollutionSourceTable = new PollutionSourceTable();

                    var measuredData = measureddatas.Where(m => m.DateTime == dateTimes[i].DateTime);
                    pollutionSourceTable.dateTime = dateTimes[i].DateTime;
                    pollutionSourceTable.volumetricFlow = 99.805m; //V - Объемный расход м3/с
                    for (int j = 0; j < measuredParameters.Count; j++)
                    {
                        var data = measuredData.Where(m => m.MeasuredParameterId == measuredParameters[j].Id).FirstOrDefault();
                        if (data != null)
                        {
                            if (data.MeasuredParameterId == 9)
                            {
                                pollutionSourceTable.sulphurDioxide = Math.Round(Convert.ToDecimal(data.Value), 3);
                            }
                            if (data.MeasuredParameterId == 25)
                            {
                                pollutionSourceTable.flow = Math.Round(Convert.ToDecimal(data.Value), 3);
                            }
                        }
                    }
                    pollutionSourceTable.mpc = measureddatas.Where(m => m.MeasuredParameterId == 25).FirstOrDefault().MeasuredParameter.MPCMaxSingle; //ПДКм.р. г/с
                    if (pollutionSourceTable.flow > pollutionSourceTable.mpc)
                    {
                        pollutionSourceTable.excess = pollutionSourceTable.flow - pollutionSourceTable.mpc; //Превышение г/с
                    }
                    pollutionSourceTables.Add(pollutionSourceTable);
                }

                //return Json(new
                //{
                //    array,
                //    header
                //});

                return Json(new
                {
                    pollutionSourceTables
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartEco.Data;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class MonitoringPostsController : Controller
    {
        //        private readonly ApplicationDbContext _context;

        //        public MonitoringPostsController(ApplicationDbContext context)
        //        {
        //            _context = context;
        //        }

        //        // GET: MonitoringPosts
        //        public async Task<IActionResult> Index()
        //        {
        //            var applicationDbContext = _context.MonitoringPost.Include(m => m.DataProvider).Include(m => m.PollutionEnvironment);
        //            return View(await applicationDbContext.ToListAsync());
        //        }

        //        // GET: MonitoringPosts/Details/5
        //        public async Task<IActionResult> Details(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return NotFound();
        //            }

        //            var monitoringPost = await _context.MonitoringPost
        //                .Include(m => m.DataProvider)
        //                .Include(m => m.PollutionEnvironment)
        //                .FirstOrDefaultAsync(m => m.Id == id);
        //            if (monitoringPost == null)
        //            {
        //                return NotFound();
        //            }

        //            return View(monitoringPost);
        //        }

        //        // GET: MonitoringPosts/Create
        //        public IActionResult Create()
        //        {
        //            ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id");
        //            ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id");
        //            return View();
        //        }

        //        // POST: MonitoringPosts/Create
        //        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Create([Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                _context.Add(monitoringPost);
        //                await _context.SaveChangesAsync();
        //                return RedirectToAction(nameof(Index));
        //            }
        //            ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //            ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //            return View(monitoringPost);
        //        }

        //        // GET: MonitoringPosts/Edit/5
        //        public async Task<IActionResult> Edit(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return NotFound();
        //            }

        //            var monitoringPost = await _context.MonitoringPost.FindAsync(id);
        //            if (monitoringPost == null)
        //            {
        //                return NotFound();
        //            }
        //            ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //            ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //            return View(monitoringPost);
        //        }

        //        // POST: MonitoringPosts/Edit/5
        //        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost)
        //        {
        //            if (id != monitoringPost.Id)
        //            {
        //                return NotFound();
        //            }

        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //                    _context.Update(monitoringPost);
        //                    await _context.SaveChangesAsync();
        //                }
        //                catch (DbUpdateConcurrencyException)
        //                {
        //                    if (!MonitoringPostExists(monitoringPost.Id))
        //                    {
        //                        return NotFound();
        //                    }
        //                    else
        //                    {
        //                        throw;
        //                    }
        //                }
        //                return RedirectToAction(nameof(Index));
        //            }
        //            ViewData["DataProviderId"] = new SelectList(_context.DataProvider, "Id", "Id", monitoringPost.DataProviderId);
        //            ViewData["PollutionEnvironmentId"] = new SelectList(_context.PollutionEnvironment, "Id", "Id", monitoringPost.PollutionEnvironmentId);
        //            return View(monitoringPost);
        //        }

        //        // GET: MonitoringPosts/Delete/5
        //        public async Task<IActionResult> Delete(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return NotFound();
        //            }

        //            var monitoringPost = await _context.MonitoringPost
        //                .Include(m => m.DataProvider)
        //                .Include(m => m.PollutionEnvironment)
        //                .FirstOrDefaultAsync(m => m.Id == id);
        //            if (monitoringPost == null)
        //            {
        //                return NotFound();
        //            }

        //            return View(monitoringPost);
        //        }

        //        // POST: MonitoringPosts/Delete/5
        //        [HttpPost, ActionName("Delete")]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> DeleteConfirmed(int id)
        //        {
        //            var monitoringPost = await _context.MonitoringPost.FindAsync(id);
        //            _context.MonitoringPost.Remove(monitoringPost);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }

        //        private bool MonitoringPostExists(int id)
        //        {
        //            return _context.MonitoringPost.Any(e => e.Id == id);
        //        }
        //    }
        //}

        private readonly HttpApiClientController _HttpApiClient;

        public MonitoringPostsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: MonitoringPosts
        public async Task<IActionResult> Index(string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();

            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;

            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";
            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.DataProviderSort = SortOrder == "DataProvider" ? "DataProviderDesc" : "DataProvider";
            ViewBag.PollutionEnvironmentSort = SortOrder == "PollutionEnvironment" ? "PollutionEnvironmentDesc" : "PollutionEnvironment";

            string url = "api/MonitoringPosts",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (NumberFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={NumberFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Number={NumberFilter}";
            }
            if (NameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Name={NameFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Name={NameFilter}";
            }
            if (DataProviderIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DataProviderId={DataProviderIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"DataProviderId={DataProviderIdFilter}";
            }
            if (PollutionEnvironmentIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
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
                monitoringPosts = await response.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            int monitoringPostCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                monitoringPostCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(monitoringPostCount / (decimal)PageSize) : 1;
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

            List<DataProvider> dataProviders = new List<DataProvider>();
            string urlDataProviders = "api/DataProviders",
                routeDataProviders = "";
            HttpResponseMessage responseDataProviders = await _HttpApiClient.GetAsync(urlDataProviders + routeDataProviders);
            if (responseDataProviders.IsSuccessStatusCode)
            {
                dataProviders = await responseDataProviders.Content.ReadAsAsync<List<DataProvider>>();
            }
            ViewBag.DataProviders = new SelectList(dataProviders.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            return View(monitoringPosts);
        }

        // GET: MonitoringPosts/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            MonitoringPost monitoringPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MonitoringPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                monitoringPost = await response.Content.ReadAsAsync<MonitoringPost>();
            }
            if (monitoringPost == null)
            {
                return NotFound();
            }

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }

            return View(monitoringPost);
        }

        // GET: MonitoringPosts/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;

            List<DataProvider> dataProviders = new List<DataProvider>();
            string urlDataProviders = "api/DataProviders",
                routeDataProviders = "";
            HttpResponseMessage responseDataProviders = await _HttpApiClient.GetAsync(urlDataProviders + routeDataProviders);
            if (responseDataProviders.IsSuccessStatusCode)
            {
                dataProviders = await responseDataProviders.Content.ReadAsAsync<List<DataProvider>>();
            }
            ViewBag.DataProviders = new SelectList(dataProviders.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = measuredParameters.Where(m => m.OceanusCode != null).OrderBy(m => m.Name);

            return View();
        }

        // POST: MonitoringPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,MN,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost,
            int?[] Sensors,
            string[] Minimum,
            string[] Maximum,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (ModelState.IsValid)
            {
                int logNumber = monitoringPost.Number;
                decimal logNorthLatitude = monitoringPost.NorthLatitude;
                decimal logEastLongitude = monitoringPost.EastLongitude;
                DateTime logDateTimeStart = DateTime.Now;

                string url = "api/Logs/AddNote",
                route = "";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={logNumber.ToString()}";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeStart={logDateTimeStart.ToString()}";

                HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/MonitoringPosts", monitoringPost);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();
                    int id = Convert.ToInt32(OutputViewText.Substring(OutputViewText.IndexOf(':') + 1, OutputViewText.IndexOf(',') - (OutputViewText.IndexOf(':') + 1)));

                    url = "api/MonitoringPosts/monitoringPostMeasuredParameter";
                    route = "";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MonitoringPostId={id.ToString()}";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"CultureName={HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name.ToString()}";

                    foreach (var sensor in Sensors)
                    {
                        route += string.IsNullOrEmpty(route) ? "?" : "&";
                        route += $"MeasuredParametersId={sensor.ToString()}".Replace(',', '.');
                    }

                    foreach (var min in Minimum)
                    {
                        if (min == null)
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Min=null";
                        }
                        else
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Min={min.ToString()}".Replace(',', '.');
                        }
                    }

                    foreach (var max in Maximum)
                    {
                        if (max == null)
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Max=null";
                        }
                        else
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Max={max.ToString()}".Replace(',', '.');
                        }
                    }

                    HttpResponseMessage responseMPMP = await _HttpApiClient.PostAsync(url + route, null);
                }
                catch
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                    {
                        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                    }
                    return View(monitoringPost);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter,
                        NameFilter = ViewBag.NameFilter,
                        DataProviderIdFilter = ViewBag.DataProviderIdFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
                    });
            }
            return View(monitoringPost);
        }

        // GET: MonitoringPosts/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            MonitoringPost monitoringPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MonitoringPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                monitoringPost = await response.Content.ReadAsAsync<MonitoringPost>();
            }

            List<DataProvider> dataProviders = new List<DataProvider>();
            string urlDataProviders = "api/DataProviders",
                routeDataProviders = "";
            HttpResponseMessage responseDataProviders = await _HttpApiClient.GetAsync(urlDataProviders + routeDataProviders);
            if (responseDataProviders.IsSuccessStatusCode)
            {
                dataProviders = await responseDataProviders.Content.ReadAsAsync<List<DataProvider>>();
            }
            ViewBag.DataProviders = new SelectList(dataProviders.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            if(!string.IsNullOrEmpty(monitoringPost.AdditionalInformation))
            {
                monitoringPost.AdditionalInformation = monitoringPost.AdditionalInformation.Replace("\r\n", "\r");
            }

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = measuredParameters.Where(m => m.OceanusCode != null);

            List<MonitoringPostMeasuredParameters> monitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameters>();
            string urlMonitoringPostMeasuredParameters = "api/MonitoringPosts/getMonitoringPostMeasuredParameters";
            string routeMonitoringPostMeasuredParameters = "";
            routeMonitoringPostMeasuredParameters += string.IsNullOrEmpty(routeMonitoringPostMeasuredParameters) ? "?" : "&";
            routeMonitoringPostMeasuredParameters += $"MonitoringPostId={id.ToString()}";
            HttpResponseMessage responseMPMP = await _HttpApiClient.PostAsync(urlMonitoringPostMeasuredParameters + routeMonitoringPostMeasuredParameters, null);
            if (responseMPMP.IsSuccessStatusCode)
            {
                monitoringPostMeasuredParameters = await responseMPMP.Content.ReadAsAsync<List<MonitoringPostMeasuredParameters>>();
            }
            ViewBag.MonitoringPostMeasuredParameters = monitoringPostMeasuredParameters.OrderBy(m => m.MeasuredParameter.Name);

            return View(monitoringPost);
        }

        // POST: MonitoringPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,NorthLatitude,EastLongitude,AdditionalInformation,MN,DataProviderId,PollutionEnvironmentId")] MonitoringPost monitoringPost,
            int?[] Sensors,
            string[] Minimum,
            string[] Maximum,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id != monitoringPost.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                int logNumber = monitoringPost.Number;
                decimal logNorthLatitude = monitoringPost.NorthLatitude;
                decimal logEastLongitude = monitoringPost.EastLongitude;
                DateTime logDateTimeStart = DateTime.Now;

                string url = "api/Logs/EditNote",
                route = "";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={logNumber.ToString()}";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeStart={logDateTimeStart.ToString()}";

                HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/MonitoringPosts/{monitoringPost.Id}", monitoringPost);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();

                    url = "api/MonitoringPosts/editMonitoringPostMeasuredParameter";
                    route = "";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MonitoringPostId={id.ToString()}";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"CultureName={HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name.ToString()}";

                    foreach (var sensor in Sensors)
                    {
                        route += string.IsNullOrEmpty(route) ? "?" : "&";
                        route += $"MeasuredParametersId={sensor.ToString()}".Replace(',', '.');
                    }

                    foreach (var min in Minimum)
                    {
                        if (min == null)
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Min=null";
                        }
                        else
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Min={min.ToString()}".Replace(',', '.');
                        }
                    }

                    foreach (var max in Maximum)
                    {
                        if (max == null)
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Max=null";
                        }
                        else
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"Max={max.ToString()}".Replace(',', '.');
                        }
                    }

                    HttpResponseMessage responseMPMP = await _HttpApiClient.PostAsync(url + route, null);
                }
                catch
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    if(errors!=null)
                    {
                        foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                        {
                            ModelState.AddModelError(property.Name, property.Value[0].ToString());
                        }
                    }
                    else
                    {
                        monitoringPost = await response.Content.ReadAsAsync<MonitoringPost>();
                        return RedirectToAction(nameof(Index),
                            new
                            {
                                SortOrder = ViewBag.SortOrder,
                                PageSize = ViewBag.PageSize,
                                PageNumber = ViewBag.PageNumber,
                                NumberFilter = ViewBag.NumberFilter,
                                NameFilter = ViewBag.NameFilter,
                                DataProviderIdFilter = ViewBag.DataProviderIdFilter,
                                PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
                            });
                    }
                    return View(monitoringPost);
                }

                monitoringPost = await response.Content.ReadAsAsync<MonitoringPost>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter,
                        NameFilter = ViewBag.NameFilter,
                        DataProviderIdFilter = ViewBag.DataProviderIdFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
                    });
            }
            return View(monitoringPost);
        }

        // GET: MonitoringPosts/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            MonitoringPost monitoringPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MonitoringPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                monitoringPost = await response.Content.ReadAsAsync<MonitoringPost>();
            }
            if (monitoringPost == null)
            {
                return NotFound();
            }

            return View(monitoringPost);
        }

        // POST: MonitoringPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? NumberFilter,
            string NameFilter,
            int? DataProviderIdFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            ViewBag.NameFilter = NameFilter;
            ViewBag.DataProviderIdFilter = DataProviderIdFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/MonitoringPosts/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter,
                        NameFilter = ViewBag.NameFilter,
                        DataProviderIdFilter = ViewBag.DataProviderIdFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
                    });
        }

        [HttpPost]
        public async Task<ActionResult> GetMPCExceedEcoservicePosts()
        {
            int MPCExceedPastMinutes = Startup.Configuration.GetValue<int>("MPCExceedPastMinutes");
            int? dataProviderId = null;

            string urlDataProviders = "api/DataProviders",
                routeDataProviders = "";
            routeDataProviders += string.IsNullOrEmpty(routeDataProviders) ? "?" : "&";
            routeDataProviders += $"Name={Startup.Configuration["EcoserviceName"].ToString()}";
            HttpResponseMessage responseDataProviders = await _HttpApiClient.GetAsync(urlDataProviders + routeDataProviders);
            if (responseDataProviders.IsSuccessStatusCode)
            {
                dataProviderId = (await responseDataProviders.Content.ReadAsAsync<List<DataProvider>>()).FirstOrDefault()?.Id;
            }

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlGetEcoserviceMonitoringPostsExceed = "api/MonitoringPosts/exceed",
                routeGetEcoserviceMonitoringPostsExceed = "";
            routeGetEcoserviceMonitoringPostsExceed += string.IsNullOrEmpty(routeGetEcoserviceMonitoringPostsExceed) ? "?" : "&";
            routeGetEcoserviceMonitoringPostsExceed += $"MPCExceedPastMinutes={MPCExceedPastMinutes.ToString()}";
            routeGetEcoserviceMonitoringPostsExceed += string.IsNullOrEmpty(routeGetEcoserviceMonitoringPostsExceed) ? "?" : "&";
            routeGetEcoserviceMonitoringPostsExceed += $"DataProviderId={dataProviderId.ToString()}";
            HttpResponseMessage responseGetEcoserviceMonitoringPostsExceed = await _HttpApiClient.GetAsync(urlGetEcoserviceMonitoringPostsExceed + routeGetEcoserviceMonitoringPostsExceed);
            if (responseGetEcoserviceMonitoringPostsExceed.IsSuccessStatusCode)
            {
                monitoringPosts = await responseGetEcoserviceMonitoringPostsExceed.Content.ReadAsAsync<List<MonitoringPost>>();
            }

            int[] ids = monitoringPosts.Select(m => m.Id).ToArray();
            return Json(new
            {
                ids
            });
        }

        [HttpPost]
        public async Task<ActionResult> GetInactiveEcoservicePosts()
        {
            int InactivePastMinutes = Startup.Configuration.GetValue<int>("InactivePastMinutes");
            int? dataProviderId = null;

            string urlDataProviders = "api/DataProviders",
                routeDataProviders = "";
            routeDataProviders += string.IsNullOrEmpty(routeDataProviders) ? "?" : "&";
            routeDataProviders += $"Name={Startup.Configuration["EcoserviceName"].ToString()}";
            HttpResponseMessage responseDataProviders = await _HttpApiClient.GetAsync(urlDataProviders + routeDataProviders);
            if (responseDataProviders.IsSuccessStatusCode)
            {
                dataProviderId = (await responseDataProviders.Content.ReadAsAsync<List<DataProvider>>()).FirstOrDefault()?.Id;
            }

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlGetEcoserviceMonitoringPostsExceed = "api/MonitoringPosts/inactive",
                routeGetEcoserviceMonitoringPostsExceed = "";
            routeGetEcoserviceMonitoringPostsExceed += string.IsNullOrEmpty(routeGetEcoserviceMonitoringPostsExceed) ? "?" : "&";
            routeGetEcoserviceMonitoringPostsExceed += $"InactivePastMinutes={InactivePastMinutes.ToString()}";
            routeGetEcoserviceMonitoringPostsExceed += string.IsNullOrEmpty(routeGetEcoserviceMonitoringPostsExceed) ? "?" : "&";
            routeGetEcoserviceMonitoringPostsExceed += $"DataProviderId={dataProviderId.ToString()}";
            HttpResponseMessage responseGetEcoserviceMonitoringPostsExceed = await _HttpApiClient.GetAsync(urlGetEcoserviceMonitoringPostsExceed + routeGetEcoserviceMonitoringPostsExceed);
            if (responseGetEcoserviceMonitoringPostsExceed.IsSuccessStatusCode)
            {
                monitoringPosts = await responseGetEcoserviceMonitoringPostsExceed.Content.ReadAsAsync<List<MonitoringPost>>();
            }

            int[] ids = monitoringPosts.Select(m => m.Id).ToArray();
            return Json(new
            {
                ids
            });
        }

        //private bool MonitoringPostExists(int id)
        //{
        //    return _context.MonitoringPost.Any(e => e.Id == id);
        //}
    }
}

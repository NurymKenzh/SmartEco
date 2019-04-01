using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class PollutionSourcesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PollutionSourcesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PollutionSources
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PollutionSource> pollutionSources = new List<PollutionSource>();

            ViewBag.NameFilter = NameFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";

            string url = "api/PollutionSources",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (!string.IsNullOrEmpty(NameFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Name={NameFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Name={NameFilter}";
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
                pollutionSources = await response.Content.ReadAsAsync<List<PollutionSource>>();
            }
            int pollutionSourcesCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                pollutionSourcesCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(pollutionSourcesCount / (decimal)PageSize) : 1;
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

            return View(pollutionSources);
        }

        // GET: PollutionSources/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            if (id == null)
            {
                return NotFound();
            }

            PollutionSource pollutionSource = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionSources/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionSource = await response.Content.ReadAsAsync<PollutionSource>();
            }
            if (pollutionSource == null)
            {
                return NotFound();
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
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = (DateTime.Now).ToString("HH:mm:ss");
            return View(pollutionSource);
        }

        // GET: PollutionSources/Create
        public IActionResult Create(string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            return View();
        }

        // POST: PollutionSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude")] PollutionSource pollutionSource,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            if (ModelState.IsValid)
            {
                //int logNumber = pollutionSource.Number;
                //decimal logNorthLatitude = pollutionSource.NorthLatitude;
                //decimal logEastLongitude = pollutionSource.EastLongitude;
                //DateTime logDateTimeStart = DateTime.Now;

                //string url = "api/Logs/AddNote",
                //route = "";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"Number={logNumber.ToString()}";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"DateTimeStart={logDateTimeStart.ToString()}";

                //HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/PollutionSources", pollutionSource);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                    {
                        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                    }
                    return View(pollutionSource);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter
                    });
            }
            return View(pollutionSource);
        }

        // GET: PollutionSources/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            PollutionSource pollutionSource = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionSources/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionSource = await response.Content.ReadAsAsync<PollutionSource>();
            }
            return View(pollutionSource);
        }

        // POST: PollutionSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude")] PollutionSource pollutionSource,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            if (id != pollutionSource.Id)
            {
            }
            if (ModelState.IsValid)
            {
                //int logNumber = pollutionSource.Number;
                //decimal logNorthLatitude = pollutionSource.NorthLatitude;
                //decimal logEastLongitude = pollutionSource.EastLongitude;
                //DateTime logDateTimeStart = DateTime.Now;

                //string url = "api/Logs/EditNote",
                //route = "";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"Number={logNumber.ToString()}";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"DateTimeStart={logDateTimeStart.ToString()}";

                //HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/PollutionSources/{pollutionSource.Id}", pollutionSource);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                    {
                        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                    }
                    return View(pollutionSource);
                }

                pollutionSource = await response.Content.ReadAsAsync<PollutionSource>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter
                    });
            }
            return View(pollutionSource);
        }

        // GET: PollutionSources/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            if (id == null)
            {
                return NotFound();
            }

            PollutionSource pollutionSource = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionSources/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionSource = await response.Content.ReadAsAsync<PollutionSource>();
            }
            if (pollutionSource == null)
            {
                return NotFound();
            }

            return View(pollutionSource);
        }

        // POST: PollutionSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/PollutionSources/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter
                    });
        }

        //private bool PollutionSourceExists(int id)
        //{
        //    return _context.PollutionSource.Any(e => e.Id == id);
        //}

        [HttpPost]
        public async Task<IActionResult> GetPollutionSourceDatas(int PollutantId,
            int PollutionSourceId,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo)
        {
            List<PollutionSourceData> pollutionSourceDatas = new List<PollutionSourceData>();
            PollutionSourceData[] pollutionsourcedatas = null;
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            string url = "api/PollutionSourceDatas",
                route = "";
            // PollutantId
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutantId={PollutantId}";
            }
            // PollutionSourceId
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionSourceId={PollutionSourceId}";
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
                pollutionSourceDatas = await response.Content.ReadAsAsync<List<PollutionSourceData>>();
            }
            pollutionsourcedatas = pollutionSourceDatas
                .OrderBy(p => p.DateTime)
                .ToArray();
            return Json(new
            {
                pollutionsourcedatas
            });
        }
    }
}

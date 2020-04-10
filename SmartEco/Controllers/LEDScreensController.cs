using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartEco.Data;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class LEDScreensController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public LEDScreensController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: LEDScreens
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<LEDScreen> ledScreens = new List<LEDScreen>();

            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.MonitoringPostSort = SortOrder == "MonitoringPost" ? "MonitoringPostDesc" : "MonitoringPost";

            string url = "api/LEDScreens",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (NameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Name={NameFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Name={NameFilter}";
            }
            if (MonitoringPostIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringPostId={MonitoringPostIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"MonitoringPostId={MonitoringPostIdFilter}";
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
                ledScreens = await response.Content.ReadAsAsync<List<LEDScreen>>();
            }
            int ledScreenCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                ledScreenCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(ledScreenCount / (decimal)PageSize) : 1;
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

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");

            return View(ledScreens);
        }

        // GET: LEDScreens/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            LEDScreen ledScreen = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/LEDScreens/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ledScreen = await response.Content.ReadAsAsync<LEDScreen>();
            }
            if (ledScreen == null)
            {
                return NotFound();
            }

            return View(ledScreen);
        }

        // GET: LEDScreens/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: LEDScreens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude,MonitoringPostId")] LEDScreen ledScreen,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/LEDScreens", ledScreen);

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
                    return View(ledScreen);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        MonitoringPostIdFilter = ViewBag.MonitoringPostIdFilter
                    });
            }
            return View(ledScreen);
        }

        // GET: LEDScreens/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            LEDScreen ledScreen = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/LEDScreens/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ledScreen = await response.Content.ReadAsAsync<LEDScreen>();
            }
            
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");

            return View(ledScreen);
        }

        // POST: LEDScreens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude,MonitoringPostId")] LEDScreen ledScreen,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            if (id != ledScreen.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/LEDScreens/{ledScreen.Id}", ledScreen);

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
                    return View(ledScreen);
                }

                ledScreen = await response.Content.ReadAsAsync<LEDScreen>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        MonitoringPostIdFilter = ViewBag.MonitoringPostIdFilter
                    });
            }
            return View(ledScreen);
        }

        // GET: LEDScreens/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            LEDScreen ledScreen = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/LEDScreens/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ledScreen = await response.Content.ReadAsAsync<LEDScreen>();
            }
            if (ledScreen == null)
            {
                return NotFound();
            }

            return View(ledScreen);
        }

        // POST: LEDScreens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            int? MonitoringPostIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.MonitoringPostIdFilter = MonitoringPostIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/LEDScreens/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        MonitoringPostIdFilter = ViewBag.MonitoringPostIdFilter
                    });
        }
    }
}

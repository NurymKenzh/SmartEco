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
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
{
    public class ExecutorsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public ExecutorsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Executors
        public async Task<IActionResult> Index(string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Executor> executors = new List<Executor>();

            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;

            ViewBag.FullNameSort = SortOrder == "FullName" ? "FullNameDesc" : "FullName";
            ViewBag.PositionSort = SortOrder == "Position" ? "PositionDesc" : "Position";

            string url = "api/Executors",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (!string.IsNullOrEmpty(FullNameFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"FullName={FullNameFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"FullName={FullNameFilter}";
            }
            if (!string.IsNullOrEmpty(PositionFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Position={PositionFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Position={PositionFilter}";
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
                executors = await response.Content.ReadAsAsync<List<Executor>>();
            }
            int executorsCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                executorsCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(executorsCount / (decimal)PageSize) : 1;
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

            return View(executors);
        }

        // GET: Executors/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            if (id == null)
            {
                return NotFound();
            }

            Executor executor = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Executors/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                executor = await response.Content.ReadAsAsync<Executor>();
            }
            if (executor == null)
            {
                return NotFound();
            }

            decimal efficiency = 0m;
            string urlExecutors = "api/Executors/CalcEfficiency",
                routeExecutors = "";

            routeExecutors += string.IsNullOrEmpty(routeExecutors) ? "?" : "&";
            routeExecutors += $"ExecutorId={id.ToString()}";

            HttpResponseMessage responseExecutors = await _HttpApiClient.GetAsync(urlExecutors + routeExecutors);
            if (responseExecutors.IsSuccessStatusCode)
            {
                efficiency = await responseExecutors.Content.ReadAsAsync<decimal>();
            }

            ViewBag.Efficiency = efficiency;

            return View(executor);
        }

        // GET: Executors/Create
        public IActionResult Create(string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            return View();
        }

        // POST: Executors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Position")] Executor executor,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Executors", executor);

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
                    return View(executor);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        FullNameFilter = ViewBag.FullNameFilter,
                        PositionFilter = ViewBag.PositionFilter
                    });
            }
            return View(executor);
        }

        // GET: Executors/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            Executor executor = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Executors/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                executor = await response.Content.ReadAsAsync<Executor>();
            }

            decimal efficiency = 0m;
            string urlExecutors = "api/Executors/CalcEfficiency",
                routeExecutors = "";

            routeExecutors += string.IsNullOrEmpty(routeExecutors) ? "?" : "&";
            routeExecutors += $"ExecutorId={id.ToString()}";

            HttpResponseMessage responseExecutors = await _HttpApiClient.GetAsync(urlExecutors + routeExecutors);
            if (responseExecutors.IsSuccessStatusCode)
            {
                efficiency = await responseExecutors.Content.ReadAsAsync<decimal>();
            }

            ViewBag.Efficiency = efficiency;

            return View(executor);
        }

        // POST: Executors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Position")] Executor executor,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            if (id != executor.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Executors/{executor.Id}", executor);

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
                    return View(executor);
                }

                executor = await response.Content.ReadAsAsync<Executor>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        FullNameFilter = ViewBag.FullNameFilter,
                        PositionFilter = ViewBag.PositionFilter
                    });
            }

            decimal efficiency = 0m;
            string urlExecutors = "api/Executors/CalcEfficiency",
                routeExecutors = "";

            routeExecutors += string.IsNullOrEmpty(routeExecutors) ? "?" : "&";
            routeExecutors += $"ExecutorId={id.ToString()}";

            HttpResponseMessage responseExecutors = await _HttpApiClient.GetAsync(urlExecutors + routeExecutors);
            if (responseExecutors.IsSuccessStatusCode)
            {
                efficiency = await responseExecutors.Content.ReadAsAsync<decimal>();
            }

            ViewBag.Efficiency = efficiency;

            return View(executor);
        }

        // GET: Executors/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            if (id == null)
            {
                return NotFound();
            }

            Executor executor = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Executors/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                executor = await response.Content.ReadAsAsync<Executor>();
            }
            if (executor == null)
            {
                return NotFound();
            }

            decimal efficiency = 0m;
            string urlExecutors = "api/Executors/CalcEfficiency",
                routeExecutors = "";

            routeExecutors += string.IsNullOrEmpty(routeExecutors) ? "?" : "&";
            routeExecutors += $"ExecutorId={id.ToString()}";

            HttpResponseMessage responseExecutors = await _HttpApiClient.GetAsync(urlExecutors + routeExecutors);
            if (responseExecutors.IsSuccessStatusCode)
            {
                efficiency = await responseExecutors.Content.ReadAsAsync<decimal>();
            }

            ViewBag.Efficiency = efficiency;

            return View(executor);
        }

        // POST: Executors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string FullNameFilter,
            string PositionFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FullNameFilter = FullNameFilter;
            ViewBag.PositionFilter = PositionFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/Executors/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        FullNameFilter = ViewBag.FullNameFilter,
                        PositionFilter = ViewBag.PositionFilter
                    });

        }
    }
}

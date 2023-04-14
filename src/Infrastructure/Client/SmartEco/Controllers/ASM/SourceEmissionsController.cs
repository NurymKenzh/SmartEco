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
using SmartEco.Models.ASM;

namespace SmartEco.Controllers.ASM
{
    public class SourceEmissionsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public SourceEmissionsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: SourceEmissions
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<SourceEmission> sourceEmissions = new List<SourceEmission>();

            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.SourceAirPollutionSort = SortOrder == "SourceAirPollution" ? "SourceAirPollutionDesc" : "SourceAirPollution";

            string url = "api/SourceEmissions",
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
            if (SourceAirPollutionIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SourceAirPollutionId={SourceAirPollutionIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"SourceAirPollutionId={SourceAirPollutionIdFilter}";
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
                sourceEmissions = await response.Content.ReadAsAsync<List<SourceEmission>>();
            }
            int sourceEmissionCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                sourceEmissionCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(sourceEmissionCount / (decimal)PageSize) : 1;
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

            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();
            string urlSourceAirPollutions = "api/SourceAirPollutions",
                routeSourceAirPollutions = "";
            HttpResponseMessage responseSourceAirPollutions = await _HttpApiClient.GetAsync(urlSourceAirPollutions + routeSourceAirPollutions);
            if (responseSourceAirPollutions.IsSuccessStatusCode)
            {
                sourceAirPollutions = await responseSourceAirPollutions.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            ViewBag.SourceAirPollutions = new SelectList(sourceAirPollutions.OrderBy(m => m.Name), "Id", "Name");

            return View(sourceEmissions);
        }

        // GET: SourceEmissions/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SourceEmission sourceEmission = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceEmissions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceEmission = await response.Content.ReadAsAsync<SourceEmission>();
            }
            if (sourceEmission == null)
            {
                return NotFound();
            }

            return View(sourceEmission);
        }

        // GET: SourceEmissions/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;

            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();
            string urlSourceAirPollutions = "api/SourceAirPollutions",
                routeSourceAirPollutions = "";
            HttpResponseMessage responseSourceAirPollutions = await _HttpApiClient.GetAsync(urlSourceAirPollutions + routeSourceAirPollutions);
            if (responseSourceAirPollutions.IsSuccessStatusCode)
            {
                sourceAirPollutions = await responseSourceAirPollutions.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            ViewBag.SourceAirPollutions = new SelectList(sourceAirPollutions.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: SourceEmissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude,SourceAirPollutionId")] SourceEmission sourceEmission,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/SourceEmissions", sourceEmission);

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
                    return View(sourceEmission);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        SourceAirPollutionIdFilter = ViewBag.SourceAirPollutionIdFilter
                    });
            }

            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();
            string urlSourceAirPollutions = "api/SourceAirPollutions",
                routeSourceAirPollutions = "";
            HttpResponseMessage responseSourceAirPollutions = await _HttpApiClient.GetAsync(urlSourceAirPollutions + routeSourceAirPollutions);
            if (responseSourceAirPollutions.IsSuccessStatusCode)
            {
                sourceAirPollutions = await responseSourceAirPollutions.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            ViewBag.SourceAirPollutions = new SelectList(sourceAirPollutions.OrderBy(m => m.Name), "Id", "Name", sourceEmission.SourceAirPollutionId);

            return View(sourceEmission);
        }

        // GET: SourceEmissions/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            SourceEmission sourceEmission = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceEmissions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceEmission = await response.Content.ReadAsAsync<SourceEmission>();
            }

            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();
            string urlSourceAirPollutions = "api/SourceAirPollutions",
                routeSourceAirPollutions = "";
            HttpResponseMessage responseSourceAirPollutions = await _HttpApiClient.GetAsync(urlSourceAirPollutions + routeSourceAirPollutions);
            if (responseSourceAirPollutions.IsSuccessStatusCode)
            {
                sourceAirPollutions = await responseSourceAirPollutions.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            ViewBag.SourceAirPollutions = new SelectList(sourceAirPollutions.OrderBy(m => m.Name), "Id", "Name", sourceEmission.SourceAirPollutionId);

            return View(sourceEmission);
        }

        // POST: SourceEmissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude,SourceAirPollutionId")] SourceEmission sourceEmission,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            if (id != sourceEmission.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/SourceEmissions/{sourceEmission.Id}", sourceEmission);

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
                    return View(sourceEmission);
                }

                sourceEmission = await response.Content.ReadAsAsync<SourceEmission>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        SourceAirPollutionIdFilter = ViewBag.SourceAirPollutionIdFilter
                    });
            }

            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();
            string urlSourceAirPollutions = "api/SourceAirPollutions",
                routeSourceAirPollutions = "";
            HttpResponseMessage responseSourceAirPollutions = await _HttpApiClient.GetAsync(urlSourceAirPollutions + routeSourceAirPollutions);
            if (responseSourceAirPollutions.IsSuccessStatusCode)
            {
                sourceAirPollutions = await responseSourceAirPollutions.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            ViewBag.SourceAirPollutions = new SelectList(sourceAirPollutions.OrderBy(m => m.Name), "Id", "Name", sourceEmission.SourceAirPollutionId);

            return View(sourceEmission);
        }

        // GET: SourceEmissions/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SourceEmission sourceEmission = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceEmissions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceEmission = await response.Content.ReadAsAsync<SourceEmission>();
            }
            if (sourceEmission == null)
            {
                return NotFound();
            }

            return View(sourceEmission);
        }

        // POST: SourceEmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            int? SourceAirPollutionIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.SourceAirPollutionIdFilter = SourceAirPollutionIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/SourceEmissions/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        SourceAirPollutionIdFilter = ViewBag.SourceAirPollutionIdFilter
                    });
        }
    }
}

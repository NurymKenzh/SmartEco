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
using SmartEco.Models.AMS;

namespace SmartEco.Controllers.AMS
{
    public class SourceAirPollutionsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public SourceAirPollutionsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: SourceAirPollutions
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<SourceAirPollution> sourceAirPollutions = new List<SourceAirPollution>();

            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.ManufactorySort = SortOrder == "Manufactory" ? "ManufactoryDesc" : "Manufactory";

            string url = "api/SourceAirPollutions",
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
            if (ManufactoryIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"ManufactoryId={ManufactoryIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"ManufactoryId={ManufactoryIdFilter}";
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
                sourceAirPollutions = await response.Content.ReadAsAsync<List<SourceAirPollution>>();
            }
            int sourceAirPollutionCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                sourceAirPollutionCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(sourceAirPollutionCount / (decimal)PageSize) : 1;
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

            List<Manufactory> manufactories = new List<Manufactory>();
            string urlManufactories = "api/Manufactories",
                routeManufactories = "";
            HttpResponseMessage responseManufactories = await _HttpApiClient.GetAsync(urlManufactories + routeManufactories);
            if (responseManufactories.IsSuccessStatusCode)
            {
                manufactories = await responseManufactories.Content.ReadAsAsync<List<Manufactory>>();
            }
            ViewBag.Manufactories = new SelectList(manufactories.OrderBy(m => m.Name), "Id", "Name");

            return View(sourceAirPollutions);
        }

        // GET: SourceAirPollutions/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SourceAirPollution sourceAirPollution = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceAirPollutions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceAirPollution = await response.Content.ReadAsAsync<SourceAirPollution>();
            }
            if (sourceAirPollution == null)
            {
                return NotFound();
            }

            return View(sourceAirPollution);
        }

        // GET: SourceAirPollutions/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;

            List<Manufactory> manufactories = new List<Manufactory>();
            string urlManufactories = "api/Manufactories",
                routeManufactories = "";
            HttpResponseMessage responseManufactories = await _HttpApiClient.GetAsync(urlManufactories + routeManufactories);
            if (responseManufactories.IsSuccessStatusCode)
            {
                manufactories = await responseManufactories.Content.ReadAsAsync<List<Manufactory>>();
            }
            ViewBag.Manufactories = new SelectList(manufactories.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: SourceAirPollutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude,ManufactoryId")] SourceAirPollution sourceAirPollution,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/SourceAirPollutions", sourceAirPollution);

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
                    return View(sourceAirPollution);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        ManufactoryIdFilter = ViewBag.ManufactoryIdFilter
                    });
            }

            List<Manufactory> manufactories = new List<Manufactory>();
            string urlManufactories = "api/Manufactories",
                routeManufactories = "";
            HttpResponseMessage responseManufactories = await _HttpApiClient.GetAsync(urlManufactories + routeManufactories);
            if (responseManufactories.IsSuccessStatusCode)
            {
                manufactories = await responseManufactories.Content.ReadAsAsync<List<Manufactory>>();
            }
            ViewBag.Manufactories = new SelectList(manufactories.OrderBy(m => m.Name), "Id", "Name", sourceAirPollution.ManufactoryId);

            return View(sourceAirPollution);
        }

        // GET: SourceAirPollutions/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            SourceAirPollution sourceAirPollution = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceAirPollutions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceAirPollution = await response.Content.ReadAsAsync<SourceAirPollution>();
            }

            List<Manufactory> manufactories = new List<Manufactory>();
            string urlManufactories = "api/Manufactories",
                routeManufactories = "";
            HttpResponseMessage responseManufactories = await _HttpApiClient.GetAsync(urlManufactories + routeManufactories);
            if (responseManufactories.IsSuccessStatusCode)
            {
                manufactories = await responseManufactories.Content.ReadAsAsync<List<Manufactory>>();
            }
            ViewBag.Manufactories = new SelectList(manufactories.OrderBy(m => m.Name), "Id", "Name", sourceAirPollution.ManufactoryId);

            return View(sourceAirPollution);
        }

        // POST: SourceAirPollutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude,ManufactoryId")] SourceAirPollution sourceAirPollution,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            if (id != sourceAirPollution.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/SourceAirPollutions/{sourceAirPollution.Id}", sourceAirPollution);

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
                    return View(sourceAirPollution);
                }

                sourceAirPollution = await response.Content.ReadAsAsync<SourceAirPollution>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        ManufactoryIdFilter = ViewBag.ManufactoryIdFilter
                    });
            }

            List<Manufactory> manufactories = new List<Manufactory>();
            string urlManufactories = "api/Manufactories",
                routeManufactories = "";
            HttpResponseMessage responseManufactories = await _HttpApiClient.GetAsync(urlManufactories + routeManufactories);
            if (responseManufactories.IsSuccessStatusCode)
            {
                manufactories = await responseManufactories.Content.ReadAsAsync<List<Manufactory>>();
            }
            ViewBag.Manufactories = new SelectList(manufactories.OrderBy(m => m.Name), "Id", "Name", sourceAirPollution.ManufactoryId);

            return View(sourceAirPollution);
        }

        // GET: SourceAirPollutions/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SourceAirPollution sourceAirPollution = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SourceAirPollutions/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                sourceAirPollution = await response.Content.ReadAsAsync<SourceAirPollution>();
            }
            if (sourceAirPollution == null)
            {
                return NotFound();
            }

            return View(sourceAirPollution);
        }

        // POST: SourceAirPollutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            int? ManufactoryIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.ManufactoryIdFilter = ManufactoryIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/SourceAirPollutions/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        ManufactoryIdFilter = ViewBag.ManufactoryIdFilter
                    });
        }
    }
}

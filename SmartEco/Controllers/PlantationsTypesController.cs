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
    public class PlantationsTypesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PlantationsTypesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PlantationsTypes
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PlantationsType> plantationsTypes = new List<PlantationsType>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            ViewBag.NameSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            string url = "api/PlantationsTypes",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (NameKKFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameKK={NameKKFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameKK={NameKKFilter}";
            }
            if (NameRUFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameRU={NameRUFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameRU={NameRUFilter}";
            }
            if (NameENFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameEN={NameENFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameEN={NameENFilter}";
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
                plantationsTypes = await response.Content.ReadAsAsync<List<PlantationsType>>();
            }
            int plantationsTypeCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                plantationsTypeCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(plantationsTypeCount / (decimal)PageSize) : 1;
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

            return View(plantationsTypes);
        }

        // GET: PlantationsTypes/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id == null)
            {
                return NotFound();
            }

            PlantationsType plantationsType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsType = await response.Content.ReadAsAsync<PlantationsType>();
            }
            if (plantationsType == null)
            {
                return NotFound();
            }

            return View(plantationsType);
        }

        // GET: PlantationsTypes/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            return View();
        }

        // POST: PlantationsTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameKK,NameRU,NameEN")] PlantationsType plantationsType,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/PlantationsTypes", plantationsType);

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
                    return View(plantationsType);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
            }
            return View(plantationsType);
        }

        // GET: PlantationsTypes/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            PlantationsType plantationsType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsType = await response.Content.ReadAsAsync<PlantationsType>();
            }

            return View(plantationsType);
        }

        // POST: PlantationsTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameKK,NameRU,NameEN")] PlantationsType plantationsType,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id != plantationsType.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/PlantationsTypes/{plantationsType.Id}", plantationsType);

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
                    return View(plantationsType);
                }

                plantationsType = await response.Content.ReadAsAsync<PlantationsType>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
            }
            return View(plantationsType);
        }

        // GET: PlantationsTypes/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id == null)
            {
                return NotFound();
            }

            PlantationsType plantationsType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsType = await response.Content.ReadAsAsync<PlantationsType>();
            }
            if (plantationsType == null)
            {
                return NotFound();
            }

            return View(plantationsType);
        }

        // POST: PlantationsTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/PlantationsTypes/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
        }
       
    }
}

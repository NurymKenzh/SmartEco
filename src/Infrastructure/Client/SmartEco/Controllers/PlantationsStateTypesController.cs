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
    public class PlantationsStateTypesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PlantationsStateTypesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PlantationsStateTypes
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            ViewBag.NameSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            string url = "api/PlantationsStateTypes",
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
                plantationsStateTypes = await response.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            int plantationsStateTypeCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                plantationsStateTypeCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(plantationsStateTypeCount / (decimal)PageSize) : 1;
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

            return View(plantationsStateTypes);
        }

        // GET: PlantationsStateTypes/Details/5
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

            PlantationsStateType plantationsStateType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStateTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsStateType = await response.Content.ReadAsAsync<PlantationsStateType>();
            }
            if (plantationsStateType == null)
            {
                return NotFound();
            }

            return View(plantationsStateType);
        }

        // GET: PlantationsStateTypes/Create
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

        // POST: PlantationsStateTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameKK,NameRU,NameEN")] PlantationsStateType plantationsStateType,
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
                    "api/PlantationsStateTypes", plantationsStateType);

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
                    return View(plantationsStateType);
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
            return View(plantationsStateType);
        }

        // GET: PlantationsStateTypes/Edit/5
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
            PlantationsStateType plantationsStateType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStateTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsStateType = await response.Content.ReadAsAsync<PlantationsStateType>();
            }

            return View(plantationsStateType);
        }

        // POST: PlantationsStateTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameKK,NameRU,NameEN")] PlantationsStateType plantationsStateType,
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
            if (id != plantationsStateType.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/PlantationsStateTypes/{plantationsStateType.Id}", plantationsStateType);

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
                    return View(plantationsStateType);
                }

                plantationsStateType = await response.Content.ReadAsAsync<PlantationsStateType>();
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
            return View(plantationsStateType);
        }

        // GET: PlantationsStateTypes/Delete/5
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

            PlantationsStateType plantationsStateType = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStateTypes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsStateType = await response.Content.ReadAsAsync<PlantationsStateType>();
            }
            if (plantationsStateType == null)
            {
                return NotFound();
            }

            return View(plantationsStateType);
        }

        // POST: PlantationsStateTypes/Delete/5
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
                $"api/PlantationsStateTypes/{id}");
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

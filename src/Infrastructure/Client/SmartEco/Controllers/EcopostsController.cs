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
    public class EcopostsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public EcopostsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Ecoposts
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Ecopost> ecoposts = new List<Ecopost>();

            ViewBag.NameFilter = NameFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";

            string url = "api/Ecoposts",
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
                ecoposts = await response.Content.ReadAsAsync<List<Ecopost>>();
            }
            int ecopostCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                ecopostCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(ecopostCount / (decimal)PageSize) : 1;
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

            return View(ecoposts);
        }

        // GET: Ecoposts/Details/5
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

            Ecopost ecopost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Ecoposts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecopost = await response.Content.ReadAsAsync<Ecopost>();
            }
            if (ecopost == null)
            {
                return NotFound();
            }

            return View(ecopost);
        }

        // GET: Ecoposts/Create
        public async Task<IActionResult> Create(string SortOrder,
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

        // POST: Ecoposts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude")] Ecopost ecopost,
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
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Ecoposts", ecopost);

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
                    return View(ecopost);
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

            return View(ecopost);
        }

        // GET: Ecoposts/Edit/5
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
            Ecopost ecopost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Ecoposts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecopost = await response.Content.ReadAsAsync<Ecopost>();
            }

            return View(ecopost);
        }

        // POST: Ecoposts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude")] Ecopost ecopost,
            string SortOrder,
            string NameFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            if (id != ecopost.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Ecoposts/{ecopost.Id}", ecopost);

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
                    return View(ecopost);
                }

                ecopost = await response.Content.ReadAsAsync<Ecopost>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter
                    });
            }

            return View(ecopost);
        }

        // GET: Ecoposts/Delete/5
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

            Ecopost ecopost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Ecoposts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecopost = await response.Content.ReadAsAsync<Ecopost>();
            }
            if (ecopost == null)
            {
                return NotFound();
            }

            return View(ecopost);
        }

        // POST: Ecoposts/Delete/5
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
                $"api/Ecoposts/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter
                    });
        }
    }
}

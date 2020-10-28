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
    public class GreemPlantsPassportsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public GreemPlantsPassportsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: GreemPlantsPassports
        public async Task<IActionResult> Index(string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();

            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;

            ViewBag.GreenObjectSort = SortOrder == "GreenObject" ? "GreenObjectDesc" : "GreenObject";
            ViewBag.KATOSort = SortOrder == "KATO" ? "KATODesc" : "KATO";

            string url = "api/GreemPlantsPassports",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (GreenObjectFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"GreenObject={GreenObjectFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"GreenObject={GreenObjectFilter}";
            }
            if (KATOIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"KATOId={KATOIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"KATOId={KATOIdFilter}";
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
                greemPlantsPassports = await response.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            int greemPlantsPassportCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                greemPlantsPassportCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(greemPlantsPassportCount / (decimal)PageSize) : 1;
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

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            return View(greemPlantsPassports);
        }

        // GET: GreemPlantsPassports/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            GreemPlantsPassport greemPlantsPassport = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/GreemPlantsPassports/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                greemPlantsPassport = await response.Content.ReadAsAsync<GreemPlantsPassport>();
            }
            if (greemPlantsPassport == null)
            {
                return NotFound();
            }

            return View(greemPlantsPassport);
        }

        // GET: GreemPlantsPassports/Create
        public async Task<IActionResult> Create(string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: GreemPlantsPassports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Number,NorthLatitude,EastLongitude,KATOId")] GreemPlantsPassport greemPlantsPassport,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/GreemPlantsPassports", greemPlantsPassport);

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
                    return View(greemPlantsPassport);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreenObjectFilter = ViewBag.GreenObjectFilter,
                        KATOIdFilter = ViewBag.KATOIdFilter
                    });
            }
            return View(greemPlantsPassport);
        }

        // GET: GreemPlantsPassports/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            GreemPlantsPassport greemPlantsPassport = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/GreemPlantsPassports/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                greemPlantsPassport = await response.Content.ReadAsAsync<GreemPlantsPassport>();
            }

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            return View(greemPlantsPassport);
        }

        // POST: GreemPlantsPassports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Number,NorthLatitude,EastLongitude,KATOId")] GreemPlantsPassport greemPlantsPassport,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            if (id != greemPlantsPassport.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/GreemPlantsPassports/{greemPlantsPassport.Id}", greemPlantsPassport);

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
                    return View(greemPlantsPassport);
                }

                greemPlantsPassport = await response.Content.ReadAsAsync<GreemPlantsPassport>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreenObjectFilter = ViewBag.GreenObjectFilter,
                        KATOIdFilter = ViewBag.KATOIdFilter
                    });
            }
            return View(greemPlantsPassport);
        }

        // GET: GreemPlantsPassports/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            GreemPlantsPassport greemPlantsPassport = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/GreemPlantsPassports/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                greemPlantsPassport = await response.Content.ReadAsAsync<GreemPlantsPassport>();
            }
            if (greemPlantsPassport == null)
            {
                return NotFound();
            }

            return View(greemPlantsPassport);
        }

        // POST: GreemPlantsPassports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string GreenObjectFilter,
            int? KATOIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreenObjectFilter = GreenObjectFilter;
            ViewBag.KATOIdFilter = KATOIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/GreemPlantsPassports/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreenObjectFilter = ViewBag.GreenObjectFilter,
                        KATOIdFilter = ViewBag.KATOIdFilter
                    });
        }
    }
}

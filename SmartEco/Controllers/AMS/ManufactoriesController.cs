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
    public class ManufactoriesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public ManufactoriesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Manufactories
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Manufactory> manufactories = new List<Manufactory>();

            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.EnterpriseSort = SortOrder == "Enterprise" ? "EnterpriseDesc" : "Enterprise";

            string url = "api/Manufactories",
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
            if (EnterpriseIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EnterpriseId={EnterpriseIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"EnterpriseId={EnterpriseIdFilter}";
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
                manufactories = await response.Content.ReadAsAsync<List<Manufactory>>();
            }
            int manufactoryCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                manufactoryCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(manufactoryCount / (decimal)PageSize) : 1;
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

            List<Enterprise> enterprises = new List<Enterprise>();
            string urlEnterprises = "api/Enterprises",
                routeEnterprises = "";
            HttpResponseMessage responseEnterprises = await _HttpApiClient.GetAsync(urlEnterprises + routeEnterprises);
            if (responseEnterprises.IsSuccessStatusCode)
            {
                enterprises = await responseEnterprises.Content.ReadAsAsync<List<Enterprise>>();
            }
            ViewBag.Enterprises = new SelectList(enterprises.OrderBy(m => m.Name), "Id", "Name");

            return View(manufactories);
        }

        // GET: Manufactories/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Manufactory manufactory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Manufactories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                manufactory = await response.Content.ReadAsAsync<Manufactory>();
            }
            if (manufactory == null)
            {
                return NotFound();
            }

            return View(manufactory);
        }

        // GET: Manufactories/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;

            List<Enterprise> enterprises = new List<Enterprise>();
            string urlEnterprises = "api/Enterprises",
                routeEnterprises = "";
            HttpResponseMessage responseEnterprises = await _HttpApiClient.GetAsync(urlEnterprises + routeEnterprises);
            if (responseEnterprises.IsSuccessStatusCode)
            {
                enterprises = await responseEnterprises.Content.ReadAsAsync<List<Enterprise>>();
            }
            ViewBag.Enterprises = new SelectList(enterprises.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: Manufactories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NorthLatitude,EastLongitude,EnterpriseId")] Manufactory manufactory,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Manufactories", manufactory);

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
                    return View(manufactory);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        EnterpriseIdFilter = ViewBag.EnterpriseIdFilter
                    });
            }

            List<Enterprise> enterprises = new List<Enterprise>();
            string urlEnterprises = "api/Enterprises",
                routeEnterprises = "";
            HttpResponseMessage responseEnterprises = await _HttpApiClient.GetAsync(urlEnterprises + routeEnterprises);
            if (responseEnterprises.IsSuccessStatusCode)
            {
                enterprises = await responseEnterprises.Content.ReadAsAsync<List<Enterprise>>();
            }
            ViewBag.Enterprises = new SelectList(enterprises.OrderBy(m => m.Name), "Id", "Name", manufactory.EnterpriseId);

            return View(manufactory);
        }

        // GET: Manufactories/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            Manufactory manufactory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Manufactories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                manufactory = await response.Content.ReadAsAsync<Manufactory>();
            }

            List<Enterprise> enterprises = new List<Enterprise>();
            string urlEnterprises = "api/Enterprises",
                routeEnterprises = "";
            HttpResponseMessage responseEnterprises = await _HttpApiClient.GetAsync(urlEnterprises + routeEnterprises);
            if (responseEnterprises.IsSuccessStatusCode)
            {
                enterprises = await responseEnterprises.Content.ReadAsAsync<List<Enterprise>>();
            }
            ViewBag.Enterprises = new SelectList(enterprises.OrderBy(m => m.Name), "Id", "Name", manufactory.EnterpriseId);

            return View(manufactory);
        }

        // POST: Manufactories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NorthLatitude,EastLongitude,EnterpriseId")] Manufactory manufactory,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            if (id != manufactory.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Manufactories/{manufactory.Id}", manufactory);

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
                    return View(manufactory);
                }

                manufactory = await response.Content.ReadAsAsync<Manufactory>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        EnterpriseIdFilter = ViewBag.EnterpriseIdFilter
                    });
            }

            List<Enterprise> enterprises = new List<Enterprise>();
            string urlEnterprises = "api/Enterprises",
                routeEnterprises = "";
            HttpResponseMessage responseEnterprises = await _HttpApiClient.GetAsync(urlEnterprises + routeEnterprises);
            if (responseEnterprises.IsSuccessStatusCode)
            {
                enterprises = await responseEnterprises.Content.ReadAsAsync<List<Enterprise>>();
            }
            ViewBag.Enterprises = new SelectList(enterprises.OrderBy(m => m.Name), "Id", "Name", manufactory.EnterpriseId);

            return View(manufactory);
        }

        // GET: Manufactories/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Manufactory manufactory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Manufactories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                manufactory = await response.Content.ReadAsAsync<Manufactory>();
            }
            if (manufactory == null)
            {
                return NotFound();
            }

            return View(manufactory);
        }

        // POST: Manufactories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            int? EnterpriseIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.EnterpriseIdFilter = EnterpriseIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/Manufactories/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        EnterpriseIdFilter = ViewBag.EnterpriseIdFilter
                    });
        }
    }
}

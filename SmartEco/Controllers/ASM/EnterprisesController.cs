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
using SmartEco.Models.ASM;

namespace SmartEco.Controllers.ASM
{
    public class EnterprisesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public EnterprisesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Enterprises
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Enterprise> enterprises = new List<Enterprise>();

            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.CitySort = SortOrder == "City" ? "CityDesc" : "City";
            ViewBag.CompanySort = SortOrder == "Company" ? "CompanyDesc" : "Company";

            string url = "api/Enterprises",
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
            if (CityFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"City={CityFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"City={CityFilter}";
            }
            if (CompanyIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"CompanyId={CompanyIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"CompanyId={CompanyIdFilter}";
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
                enterprises = await response.Content.ReadAsAsync<List<Enterprise>>();
            }
            int enterpriseCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                enterpriseCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(enterpriseCount / (decimal)PageSize) : 1;
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

            List<Company> companies = new List<Company>();
            string urlCompanies = "api/Companies",
                routeCompanies = "";
            HttpResponseMessage responseCompanies = await _HttpApiClient.GetAsync(urlCompanies + routeCompanies);
            if (responseCompanies.IsSuccessStatusCode)
            {
                companies = await responseCompanies.Content.ReadAsAsync<List<Company>>();
            }
            ViewBag.Companies = new SelectList(companies.OrderBy(m => m.Name), "Id", "Name");

            return View(enterprises);
        }

        // GET: Enterprises/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Enterprise enterprise = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Enterprises/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                enterprise = await response.Content.ReadAsAsync<Enterprise>();
            }
            if (enterprise == null)
            {
                return NotFound();
            }

            return View(enterprise);
        }

        // GET: Enterprises/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;

            List<Company> companies = new List<Company>();
            string urlCompanies = "api/Companies",
                routeCompanies = "";
            HttpResponseMessage responseCompanies = await _HttpApiClient.GetAsync(urlCompanies + routeCompanies);
            if (responseCompanies.IsSuccessStatusCode)
            {
                companies = await responseCompanies.Content.ReadAsAsync<List<Company>>();
            }
            ViewBag.Companies = new SelectList(companies.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: Enterprises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,City,NorthLatitude,EastLongitude,CompanyId")] Enterprise enterprise,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Enterprises", enterprise);

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
                    return View(enterprise);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        CityFilter = ViewBag.CityFilter,
                        CompanyIdFilter = ViewBag.CompanyIdFilter
                    });
            }

            List<Company> companies = new List<Company>();
            string urlCompanies = "api/Companies",
                routeCompanies = "";
            HttpResponseMessage responseCompanies = await _HttpApiClient.GetAsync(urlCompanies + routeCompanies);
            if (responseCompanies.IsSuccessStatusCode)
            {
                companies = await responseCompanies.Content.ReadAsAsync<List<Company>>();
            }
            ViewBag.Companies = new SelectList(companies.OrderBy(m => m.Name), "Id", "Name", enterprise.CompanyId);

            return View(enterprise);
        }

        // GET: Enterprises/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            Enterprise enterprise = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Enterprises/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                enterprise = await response.Content.ReadAsAsync<Enterprise>();
            }

            List<Company> companies = new List<Company>();
            string urlCompanies = "api/Companies",
                routeCompanies = "";
            HttpResponseMessage responseCompanies = await _HttpApiClient.GetAsync(urlCompanies + routeCompanies);
            if (responseCompanies.IsSuccessStatusCode)
            {
                companies = await responseCompanies.Content.ReadAsAsync<List<Company>>();
            }
            ViewBag.Companies = new SelectList(companies.OrderBy(m => m.Name), "Id", "Name", enterprise.CompanyId);

            return View(enterprise);
        }

        // POST: Enterprises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,City,NorthLatitude,EastLongitude,CompanyId")] Enterprise enterprise,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            if (id != enterprise.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Enterprises/{enterprise.Id}", enterprise);

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
                    return View(enterprise);
                }

                enterprise = await response.Content.ReadAsAsync<Enterprise>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        CityFilter = ViewBag.CityFilter,
                        CompanyIdFilter = ViewBag.CompanyIdFilter
                    });
            }

            List<Company> companies = new List<Company>();
            string urlCompanies = "api/Companies",
                routeCompanies = "";
            HttpResponseMessage responseCompanies = await _HttpApiClient.GetAsync(urlCompanies + routeCompanies);
            if (responseCompanies.IsSuccessStatusCode)
            {
                companies = await responseCompanies.Content.ReadAsAsync<List<Company>>();
            }
            ViewBag.Companies = new SelectList(companies.OrderBy(m => m.Name), "Id", "Name", enterprise.CompanyId);

            return View(enterprise);
        }

        // GET: Enterprises/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Enterprise enterprise = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Enterprises/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                enterprise = await response.Content.ReadAsAsync<Enterprise>();
            }
            if (enterprise == null)
            {
                return NotFound();
            }

            return View(enterprise);
        }

        // POST: Enterprises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameFilter,
            string CityFilter,
            int? CompanyIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameFilter = NameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.CompanyIdFilter = CompanyIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/Enterprises/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameFilter = ViewBag.NameFilter,
                        CityFilter = ViewBag.CityFilter,
                        CompanyIdFilter = ViewBag.CompanyIdFilter
                    });
        }

    }
}

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
    public class SpeciallyProtectedNaturalTerritoriesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public SpeciallyProtectedNaturalTerritoriesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: SpeciallyProtectedNaturalTerritories
        public async Task<IActionResult> Index(string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<SpeciallyProtectedNaturalTerritory> speciallyProtectedNaturalTerritories = new List<SpeciallyProtectedNaturalTerritory>();

            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;

            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.AuthorizedAuthoritySort = SortOrder == "AuthorizedAuthority" ? "AuthorizedAuthorityDesc" : "AuthorizedAuthority";

            string url = "api/SpeciallyProtectedNaturalTerritories",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (NameENFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameEN={NameENFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameEN={NameENFilter}";
            }
            if (NameRUFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameRU={NameRUFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameRU={NameRUFilter}";
            }
            if (NameKKFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameKK={NameKKFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameKK={NameKKFilter}";
            }
            if (AuthorizedAuthorityIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"AuthorizedAuthorityId={AuthorizedAuthorityIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"AuthorizedAuthorityId={AuthorizedAuthorityIdFilter}";
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
                speciallyProtectedNaturalTerritories = await response.Content.ReadAsAsync<List<SpeciallyProtectedNaturalTerritory>>();
            }
            int speciallyProtectedNaturalTerritoryCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                speciallyProtectedNaturalTerritoryCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(speciallyProtectedNaturalTerritoryCount / (decimal)PageSize) : 1;
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

            List<AuthorizedAuthority> authorizedAuthorities = new List<AuthorizedAuthority>();
            string urlAuthorizedAuthorities = "api/AuthorizedAuthorities",
                routeAuthorizedAuthorities = "";
            HttpResponseMessage responseAuthorizedAuthorities = await _HttpApiClient.GetAsync(urlAuthorizedAuthorities + routeAuthorizedAuthorities);
            if (responseAuthorizedAuthorities.IsSuccessStatusCode)
            {
                authorizedAuthorities = await responseAuthorizedAuthorities.Content.ReadAsAsync<List<AuthorizedAuthority>>();
            }
            ViewBag.AuthorizedAuthorities = new SelectList(authorizedAuthorities.OrderBy(m => m.Name), "Id", "Name");

            return View(speciallyProtectedNaturalTerritories);
        }

        // GET: SpeciallyProtectedNaturalTerritories/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciallyProtectedNaturalTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciallyProtectedNaturalTerritory = await response.Content.ReadAsAsync<SpeciallyProtectedNaturalTerritory>();
            }
            if (speciallyProtectedNaturalTerritory == null)
            {
                return NotFound();
            }

            return View(speciallyProtectedNaturalTerritory);
        }

        // GET: SpeciallyProtectedNaturalTerritories/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;

            List<AuthorizedAuthority> authorizedAuthorities = new List<AuthorizedAuthority>();
            string urlAuthorizedAuthorities = "api/AuthorizedAuthorities",
                routeAuthorizedAuthorities = "";
            HttpResponseMessage responseAuthorizedAuthorities = await _HttpApiClient.GetAsync(urlAuthorizedAuthorities + routeAuthorizedAuthorities);
            if (responseAuthorizedAuthorities.IsSuccessStatusCode)
            {
                authorizedAuthorities = await responseAuthorizedAuthorities.Content.ReadAsAsync<List<AuthorizedAuthority>>();
            }
            ViewBag.AuthorizedAuthorities = new SelectList(authorizedAuthorities.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: SpeciallyProtectedNaturalTerritories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameKK,NameRU,NameEN,AuthorizedAuthorityId,Areahectares")] SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/SpeciallyProtectedNaturalTerritories", speciallyProtectedNaturalTerritory);

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
                    return View(speciallyProtectedNaturalTerritory);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameENFilter = ViewBag.NameENFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        AuthorizedAuthorityIdFilter = ViewBag.AuthorizedAuthorityIdFilter
                    });
            }
            return View(speciallyProtectedNaturalTerritory);
        }

        // GET: SpeciallyProtectedNaturalTerritories/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciallyProtectedNaturalTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciallyProtectedNaturalTerritory = await response.Content.ReadAsAsync<SpeciallyProtectedNaturalTerritory>();
            }

            List<AuthorizedAuthority> authorizedAuthorities = new List<AuthorizedAuthority>();
            string urlAuthorizedAuthorities = "api/AuthorizedAuthorities",
                routeAuthorizedAuthorities = "";
            HttpResponseMessage responseAuthorizedAuthorities = await _HttpApiClient.GetAsync(urlAuthorizedAuthorities + routeAuthorizedAuthorities);
            if (responseAuthorizedAuthorities.IsSuccessStatusCode)
            {
                authorizedAuthorities = await responseAuthorizedAuthorities.Content.ReadAsAsync<List<AuthorizedAuthority>>();
            }
            ViewBag.AuthorizedAuthorities = new SelectList(authorizedAuthorities.OrderBy(m => m.Name), "Id", "Name");

            return View(speciallyProtectedNaturalTerritory);
        }

        // POST: SpeciallyProtectedNaturalTerritories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Number,NorthLatitude,EastLongitude,AuthorizedAuthorityId")] SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            if (id != speciallyProtectedNaturalTerritory.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/SpeciallyProtectedNaturalTerritories/{speciallyProtectedNaturalTerritory.Id}", speciallyProtectedNaturalTerritory);

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
                    return View(speciallyProtectedNaturalTerritory);
                }

                speciallyProtectedNaturalTerritory = await response.Content.ReadAsAsync<SpeciallyProtectedNaturalTerritory>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameENFilter = ViewBag.NameENFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        AuthorizedAuthorityIdFilter = ViewBag.AuthorizedAuthorityIdFilter
                    });
            }
            return View(speciallyProtectedNaturalTerritory);
        }

        // GET: SpeciallyProtectedNaturalTerritories/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SpeciallyProtectedNaturalTerritory speciallyProtectedNaturalTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciallyProtectedNaturalTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciallyProtectedNaturalTerritory = await response.Content.ReadAsAsync<SpeciallyProtectedNaturalTerritory>();
            }
            if (speciallyProtectedNaturalTerritory == null)
            {
                return NotFound();
            }

            return View(speciallyProtectedNaturalTerritory);
        }

        // POST: SpeciallyProtectedNaturalTerritories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameENFilter,
            string NameRUFilter,
            string NameKKFilter,
            int? AuthorizedAuthorityIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.AuthorizedAuthorityIdFilter = AuthorizedAuthorityIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/SpeciallyProtectedNaturalTerritories/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameENFilter = ViewBag.NameENFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        AuthorizedAuthorityIdFilter = ViewBag.AuthorizedAuthorityIdFilter
                    });
        }
    }
}

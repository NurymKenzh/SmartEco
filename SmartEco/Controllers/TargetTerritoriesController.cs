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
    public class TargetTerritoriesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public TargetTerritoriesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: TargetTerritories
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;

            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.GISConnectionCodeSort = SortOrder == "GISConnectionCode" ? "GISConnectionCodeDesc" : "GISConnectionCode";
            ViewBag.TerritoryTypeSort = SortOrder == "TerritoryType" ? "TerritoryTypeDesc" : "TerritoryType";

            string url = "api/TargetTerritories",
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
            if (GISConnectionCodeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"GISConnectionCode={GISConnectionCodeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"GISConnectionCode={GISConnectionCodeFilter}";
            }
            if (TerritoryTypeIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TerritoryTypeId={TerritoryTypeIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TerritoryTypeId={TerritoryTypeIdFilter}";
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
                targetTerritories = await response.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            int targetTerritoryCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                targetTerritoryCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(targetTerritoryCount / (decimal)PageSize) : 1;
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

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            return View(targetTerritories);
        }

        // GET: TargetTerritories/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            TargetTerritory targetTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetTerritory = await response.Content.ReadAsAsync<TargetTerritory>();
            }
            if (targetTerritory == null)
            {
                return NotFound();
            }

            return View(targetTerritory);
        }

        // GET: TargetTerritories/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.TransportPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 7).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometWaterPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 3 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometAirPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 2 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");

            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();
            string urlKazHydrometSoilPosts = "api/KazHydrometSoilPosts",
                routeKazHydrometSoilPosts = "";
            HttpResponseMessage responseKazHydrometSoilPosts = await _HttpApiClient.GetAsync(urlKazHydrometSoilPosts + routeKazHydrometSoilPosts);
            if (responseKazHydrometSoilPosts.IsSuccessStatusCode)
            {
                kazHydrometSoilPosts = await responseKazHydrometSoilPosts.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            }
            ViewBag.KazHydrometSoilPosts = new SelectList(kazHydrometSoilPosts.OrderBy(m => m.Name), "Id", "Name");

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            ViewBag.CityDistrictCATO = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Code");
            ViewBag.CityDistrictNameKK = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "NameKK");
            ViewBag.CityDistrictNameRU = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "NameRU");

            ViewBag.City = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["City"]).Id;
            ViewBag.CityDistrict = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["CityDistrict"]).Id;
            ViewBag.OtherTerritoryType = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["OtherTerritoryType"]).Id;
            ViewBag.AlmatyCATO = Startup.Configuration["AlmatyCATO"];
            ViewBag.AlmatyKK = Startup.Configuration["AlmatyKK"];
            ViewBag.AlmatyRU = Startup.Configuration["AlmatyRU"];

            return View();
        }

        // POST: TargetTerritories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TerritoryTypeId,KATOId,NameKK,NameRU,GISConnectionCode,AdditionalInformationKK,AdditionalInformationRU,MonitoringPostId,KazHydrometSoilPostId")] TargetTerritory targetTerritory,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/TargetTerritories", targetTerritory);

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
                    return View(targetTerritory);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        GISConnectionCodeFilter = ViewBag.GISConnectionCodeFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter
                    });
            }

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.TransportPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 7).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometWaterPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 3 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometAirPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 2 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");

            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();
            string urlKazHydrometSoilPosts = "api/KazHydrometSoilPosts",
                routeKazHydrometSoilPosts = "";
            HttpResponseMessage responseKazHydrometSoilPosts = await _HttpApiClient.GetAsync(urlKazHydrometSoilPosts + routeKazHydrometSoilPosts);
            if (responseKazHydrometSoilPosts.IsSuccessStatusCode)
            {
                kazHydrometSoilPosts = await responseKazHydrometSoilPosts.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            }
            ViewBag.KazHydrometSoilPosts = new SelectList(kazHydrometSoilPosts.OrderBy(m => m.Name), "Id", "Name");

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            return View(targetTerritory);
        }

        // GET: TargetTerritories/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            TargetTerritory targetTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetTerritory = await response.Content.ReadAsAsync<TargetTerritory>();
            }

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.TransportPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 7).OrderBy(m => m.Name), "Id", "Name", targetTerritory.MonitoringPostId);
            ViewBag.KazHydrometWaterPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 3 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name", targetTerritory.MonitoringPostId);
            ViewBag.KazHydrometAirPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 2 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name", targetTerritory.MonitoringPostId);

            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();
            string urlKazHydrometSoilPosts = "api/KazHydrometSoilPosts",
                routeKazHydrometSoilPosts = "";
            HttpResponseMessage responseKazHydrometSoilPosts = await _HttpApiClient.GetAsync(urlKazHydrometSoilPosts + routeKazHydrometSoilPosts);
            if (responseKazHydrometSoilPosts.IsSuccessStatusCode)
            {
                kazHydrometSoilPosts = await responseKazHydrometSoilPosts.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            }
            ViewBag.KazHydrometSoilPosts = new SelectList(kazHydrometSoilPosts.OrderBy(m => m.Name), "Id", "Name", targetTerritory.KazHydrometSoilPostId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name", targetTerritory.TerritoryTypeId);

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", targetTerritory.KATOId);

            ViewBag.CityDistrictCATO = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Code");
            ViewBag.CityDistrictNameKK = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "NameKK");
            ViewBag.CityDistrictNameRU = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "NameRU");

            ViewBag.City = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["City"]).Id;
            ViewBag.CityDistrict = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["CityDistrict"]).Id;
            ViewBag.OtherTerritoryType = territoryTypes.FirstOrDefault(t => t.NameRU == Startup.Configuration["OtherTerritoryType"]).Id;
            ViewBag.AlmatyCATO = Startup.Configuration["AlmatyCATO"];
            ViewBag.AlmatyKK = Startup.Configuration["AlmatyKK"];
            ViewBag.AlmatyRU = Startup.Configuration["AlmatyRU"];

            return View(targetTerritory);
        }

        // POST: TargetTerritories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TerritoryTypeId,KATOId,NameKK,NameRU,GISConnectionCode,AdditionalInformationKK,AdditionalInformationRU,MonitoringPostId,KazHydrometSoilPostId")] TargetTerritory targetTerritory,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            if (id != targetTerritory.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/TargetTerritories/{targetTerritory.Id}", targetTerritory);

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
                    return View(targetTerritory);
                }

                targetTerritory = await response.Content.ReadAsAsync<TargetTerritory>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        GISConnectionCodeFilter = ViewBag.GISConnectionCodeFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter
                    });
            }

            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            string urlMonitoringPosts = "api/MonitoringPosts",
                routeMonitoringPosts = "";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts + routeMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            ViewBag.TransportPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 7).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometWaterPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 3 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.KazHydrometAirPosts = new SelectList(monitoringPosts.Where(m => m.PollutionEnvironmentId == 2 && m.DataProviderId == 1).OrderBy(m => m.Name), "Id", "Name");

            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();
            string urlKazHydrometSoilPosts = "api/KazHydrometSoilPosts",
                routeKazHydrometSoilPosts = "";
            HttpResponseMessage responseKazHydrometSoilPosts = await _HttpApiClient.GetAsync(urlKazHydrometSoilPosts + routeKazHydrometSoilPosts);
            if (responseKazHydrometSoilPosts.IsSuccessStatusCode)
            {
                kazHydrometSoilPosts = await responseKazHydrometSoilPosts.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            }
            ViewBag.KazHydrometSoilPosts = new SelectList(kazHydrometSoilPosts.OrderBy(m => m.Name), "Id", "Name");

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            return View(targetTerritory);
        }

        // GET: TargetTerritories/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            TargetTerritory targetTerritory = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetTerritories/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetTerritory = await response.Content.ReadAsAsync<TargetTerritory>();
            }
            if (targetTerritory == null)
            {
                return NotFound();
            }

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = targetValues.Where(t => t.TargetTerritoryId == id).AsQueryable();

            List<AActivity> aActivities = new List<AActivity>();
            string urlAActivities = "api/AActivities",
                routeAActivities = "";
            HttpResponseMessage responseAActivities = await _HttpApiClient.GetAsync(urlAActivities + routeAActivities);
            if (responseAActivities.IsSuccessStatusCode)
            {
                aActivities = await responseAActivities.Content.ReadAsAsync<List<AActivity>>();
            }
            ViewBag.AActivities = aActivities.Where(t => t.TargetTerritoryId == id).AsQueryable();

            return View(targetTerritory);
        }

        // POST: TargetTerritories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string GISConnectionCodeFilter,
            int? TerritoryTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.GISConnectionCodeFilter = GISConnectionCodeFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/TargetTerritories/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        GISConnectionCodeFilter = ViewBag.GISConnectionCodeFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter
                    });
        }
    }
}

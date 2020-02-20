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
    public class TargetValuesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public TargetValuesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: TargetValues
        public async Task<IActionResult> Index(string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<TargetValue> targetValues = new List<TargetValue>();

            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;

            ViewBag.PollutionEnvironmentIdSort = SortOrder == "PollutionEnvironmentId" ? "PollutionEnvironmentIdDesc" : "PollutionEnvironmentId";
            ViewBag.TargetIdSort = SortOrder == "TargetId" ? "TargetIdDesc" : "TargetId";
            ViewBag.MeasuredParameterUnitIdSort = SortOrder == "MeasuredParameterUnitId" ? "MeasuredParameterUnitIdDesc" : "MeasuredParameterUnitId";
            ViewBag.TerritoryTypeIdSort = SortOrder == "TerritoryTypeId" ? "TerritoryTypeIdDesc" : "TerritoryTypeId";
            ViewBag.TargetTerritoryIdSort = SortOrder == "TargetTerritoryId" ? "TargetTerritoryIdDesc" : "TargetTerritoryId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.TargetValueTypeSort = SortOrder == "TargetValueType" ? "TargetValueTypeDesc" : "TargetValueType";

            string url = "api/TargetValues",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }

            if (PollutionEnvironmentIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
            }
            if (TargetIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TargetId={TargetIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TargetId={TargetIdFilter}";
            }
            if (MeasuredParameterUnitIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterUnitId={MeasuredParameterUnitIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"MeasuredParameterUnitId={MeasuredParameterUnitIdFilter}";
            }
            if (TerritoryTypeIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TerritoryTypeId={TerritoryTypeIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TerritoryTypeId={TerritoryTypeIdFilter}";
            }
            if (TargetTerritoryIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TargetTerritoryId={TargetTerritoryIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TargetTerritoryId={TargetTerritoryIdFilter}";
            }
            if (YearFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Year={YearFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Year={YearFilter}";
            }
            if (TargetValueTypeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TargetValueType={TargetValueTypeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TargetValueType={TargetValueTypeFilter}";
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
                targetValues = await response.Content.ReadAsAsync<List<TargetValue>>();
            }
            int targetValueCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                targetValueCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(targetValueCount / (decimal)PageSize) : 1;
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

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            ViewBag.Targets = new SelectList(targets.OrderBy(m => m.Name), "Id", "Name");

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name");

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.OrderBy(m => m.Name), "Id", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text");
            ViewBag.TargetValueType = new List<SelectListItem>() {
                new SelectListItem() { Text=Resources.Controllers.SharedResources.Actual, Value="true"},
                new SelectListItem() { Text=Resources.Controllers.SharedResources.Planned, Value="false"}
            }.OrderBy(s => s.Text);

            return View(targetValues);
        }

        // GET: TargetValues/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            if (id == null)
            {
                return NotFound();
            }

            TargetValue targetValue = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetValues/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetValue = await response.Content.ReadAsAsync<TargetValue>();
            }
            if (targetValue == null)
            {
                return NotFound();
            }

            return View(targetValue);
        }

        // GET: TargetValues/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            //ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            List<PollutionEnvironment> pollutionEnvironmentsUsed = new List<PollutionEnvironment>();
            foreach (var pollutionEnvironment in pollutionEnvironments)
            {
                foreach (var target in targets)
                {
                    if (pollutionEnvironment.Id == target.PollutionEnvironmentId)
                    {
                        pollutionEnvironmentsUsed.Add(pollutionEnvironment);
                        break;
                    }
                }
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == pollutionEnvironmentsUsed.OrderBy(p => p.Name).Select(p => p.Id).FirstOrDefault()).OrderBy(t => t.Name), "Id", "Name");

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.Where(t => t.PollutionEnvironmentId == pollutionEnvironmentsUsed.OrderBy(p => p.Name).Select(p => p.Id).FirstOrDefault()).OrderBy(t => t.Name).FirstOrDefault().MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(m => m.Name), "Id", "Name");

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == territoryTypes.OrderBy(tt => tt.Name).Select(tt => tt.Id).FirstOrDefault()).OrderBy(t => t.Name), "Id", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text");

            TargetValue model = new TargetValue
            {
                TargetValueType = true
            };

            return View(model);
        }

        // POST: TargetValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TargetId,TargetTerritoryId,Year,TargetValueType,Value,AdditionalInformationKK,AdditionalInformationRU")] TargetValue targetValue,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/TargetValues", targetValue);

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
                    return View(targetValue);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter,
                        TargetIdFilter = ViewBag.TargetIdFilter,
                        MeasuredParameterUnitIdFilter = ViewBag.MeasuredParameterUnitIdFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter,
                        TargetTerritoryIdFilter = ViewBag.TargetTerritoryIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        TargetValueTypeFilter = ViewBag.TargetValueTypeFilter
                    });
            }

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            //ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            List<PollutionEnvironment> pollutionEnvironmentsUsed = new List<PollutionEnvironment>();
            foreach (var pollutionEnvironment in pollutionEnvironments)
            {
                foreach (var target in targets)
                {
                    if (pollutionEnvironment.Id == target.PollutionEnvironmentId)
                    {
                        pollutionEnvironmentsUsed.Add(pollutionEnvironment);
                        break;
                    }
                }
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(m => m.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == targetValue.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", targetValue.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == targetValue.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", targetValue.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == targetValue.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", targetValue.Year);

            return View(targetValue);
        }

        // GET: TargetValues/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            TargetValue targetValue = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetValues/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetValue = await response.Content.ReadAsAsync<TargetValue>();
            }

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(t => t.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            List<PollutionEnvironment> pollutionEnvironmentsUsed = new List<PollutionEnvironment>();
            foreach (var pollutionEnvironment in pollutionEnvironments)
            {
                foreach (var target in targets)
                {
                    if (pollutionEnvironment.Id == target.PollutionEnvironmentId)
                    {
                        pollutionEnvironmentsUsed.Add(pollutionEnvironment);
                        break;
                    }
                }
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(t => t.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == targetValue.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", targetValue.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == targetValue.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", targetValue.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == targetValue.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", targetValue.Year);

            targetValue.Target = targets.FirstOrDefault(t => t.Id == targetValue.TargetId);
            targetValue.TargetTerritory = targetTerritories.FirstOrDefault(t => t.Id == targetValue.TargetTerritoryId);

            return View(targetValue);
        }

        // POST: TargetValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TargetId,TargetTerritoryId,Year,TargetValueType,Value,AdditionalInformationKK,AdditionalInformationRU")] TargetValue targetValue,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            if (id != targetValue.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/TargetValues/{targetValue.Id}", targetValue);

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
                    return View(targetValue);
                }

                targetValue = await response.Content.ReadAsAsync<TargetValue>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter,
                        TargetIdFilter = ViewBag.TargetIdFilter,
                        MeasuredParameterUnitIdFilter = ViewBag.MeasuredParameterUnitIdFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter,
                        TargetTerritoryIdFilter = ViewBag.TargetTerritoryIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        TargetValueTypeFilter = ViewBag.TargetValueTypeFilter
                    });
            }

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(t => t.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            List<PollutionEnvironment> pollutionEnvironmentsUsed = new List<PollutionEnvironment>();
            foreach (var pollutionEnvironment in pollutionEnvironments)
            {
                foreach (var target in targets)
                {
                    if (pollutionEnvironment.Id == target.PollutionEnvironmentId)
                    {
                        pollutionEnvironmentsUsed.Add(pollutionEnvironment);
                        break;
                    }
                }
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(t => t.Name), "Id", "Name", targetValue.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == targetValue.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", targetValue.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == targetValue.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", targetValue.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == targetValue.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", targetValue.Year);

            return View(targetValue);
        }

        // GET: TargetValues/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            if (id == null)
            {
                return NotFound();
            }

            TargetValue targetValue = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TargetValues/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                targetValue = await response.Content.ReadAsAsync<TargetValue>();
            }
            if (targetValue == null)
            {
                return NotFound();
            }

            return View(targetValue);
        }

        // POST: TargetValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? YearFilter,
            bool? TargetValueTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.TargetValueTypeFilter = TargetValueTypeFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/TargetValues/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter,
                        TargetIdFilter = ViewBag.TargetIdFilter,
                        MeasuredParameterUnitIdFilter = ViewBag.MeasuredParameterUnitIdFilter,
                        TerritoryTypeIdFilter = ViewBag.TerritoryTypeIdFilter,
                        TargetTerritoryIdFilter = ViewBag.TargetTerritoryIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        TargetValueTypeFilter = ViewBag.TargetValueTypeFilter
                    });
        }

        //private bool TargetValueExists(int id)
        //{
        //    return _context.KazHydrometAirPost.Any(e => e.Id == id);
        //}

        [HttpPost]
        public async Task<JsonResult> GetTargetsByPollutionEnvironmentId(int PollutionEnvironmentId)
        {
            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            
            var targetsArray = targets
                .Where(t => t.PollutionEnvironmentId == PollutionEnvironmentId).ToArray().OrderBy(t => t.Name);
            JsonResult result = new JsonResult(targetsArray);
            return result;
        }

        [HttpPost]
        public async Task<JsonResult> GetTargetTerritoriesByTerritoryTypeId(int TerritoryTypeId)
        {
            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }

            var targetTerritoriesArray = targetTerritories
                .Where(t => t.TerritoryTypeId == TerritoryTypeId).ToArray().OrderBy(t => t.Name);
            JsonResult result = new JsonResult(targetTerritoriesArray);
            return result;
        }

        [HttpPost]
        public async Task<JsonResult> MeasuredParameterUnitIdByTargetId(int TargetId)
        {
            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }

            var target = targets
                .FirstOrDefault(t => t.Id == TargetId);
            JsonResult result = new JsonResult(target == null ? 0 : target.MeasuredParameterUnitId);
            return result;
        }
    }
}

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
    public class AActivitiesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public AActivitiesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: AActivities
        public async Task<IActionResult> Index(string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<AActivity> aActivities = new List<AActivity>();

            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            ViewBag.TargetIdFilter = TargetIdFilter;
            ViewBag.MeasuredParameterUnitIdFilter = MeasuredParameterUnitIdFilter;
            ViewBag.TerritoryTypeIdFilter = TerritoryTypeIdFilter;
            ViewBag.TargetTerritoryIdFilter = TargetTerritoryIdFilter;
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;

            ViewBag.PollutionEnvironmentIdSort = SortOrder == "PollutionEnvironmentId" ? "PollutionEnvironmentIdDesc" : "PollutionEnvironmentId";
            ViewBag.TargetIdSort = SortOrder == "TargetId" ? "TargetIdDesc" : "TargetId";
            ViewBag.MeasuredParameterUnitIdSort = SortOrder == "MeasuredParameterUnitId" ? "MeasuredParameterUnitIdDesc" : "MeasuredParameterUnitId";
            ViewBag.TerritoryTypeIdSort = SortOrder == "TerritoryTypeId" ? "TerritoryTypeIdDesc" : "TerritoryTypeId";
            ViewBag.TargetTerritoryIdSort = SortOrder == "TargetTerritoryId" ? "TargetTerritoryIdDesc" : "TargetTerritoryId";
            ViewBag.EventIdSort = SortOrder == "EventId" ? "EventIdDesc" : "EventId";
            ViewBag.ProjectSort = SortOrder == "Project" ? "ProjectDesc" : "Project";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ActivityTypeSort = SortOrder == "ActivityType" ? "ActivityTypeDesc" : "ActivityType";

            string url = "api/AActivities",
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
            if (EventIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EventId={EventIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"EventId={EventIdFilter}";
            }
            if (ProjectIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"ProjectId={ProjectIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"ProjectId={ProjectIdFilter}";
            }
            if (YearFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Year={YearFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Year={YearFilter}";
            }
            if (ActivityTypeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"ActivityType={ActivityTypeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"ActivityType={ActivityTypeFilter}";
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
                aActivities = await response.Content.ReadAsAsync<List<AActivity>>();
            }
            int aActivityCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                aActivityCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(aActivityCount / (decimal)PageSize) : 1;
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

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name");

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = new SelectList(targetValues.OrderBy(m => m.Value), "Id", "Value");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text");
            ViewBag.ActivityType = new List<SelectListItem>() {
                new SelectListItem() { Text=Resources.Controllers.SharedResources.Actual, Value="true"},
                new SelectListItem() { Text=Resources.Controllers.SharedResources.Planned, Value="false"}
            }.OrderBy(s => s.Text);

            return View(aActivities);
        }

        // GET: AActivities/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            if (id == null)
            {
                return NotFound();
            }

            AActivity aActivity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/AActivities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                aActivity = await response.Content.ReadAsAsync<AActivity>();
            }
            if (aActivity == null)
            {
                return NotFound();
            }

            return View(aActivity);
        }

        // GET: AActivities/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;

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

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name");

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = new SelectList(targetValues.OrderBy(m => m.Value), "Id", "Value");

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text");

            ViewBag.StartPeriod = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.EndPeriod = (DateTime.Now).ToString("yyyy-MM-dd");

            AActivity model = new AActivity
            {
                ActivityType = true
            };

            return View(model);
        }

        // POST: AActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartPeriod,EndPeriod,TargetValueId,TargetId,TargetTerritoryId,EventId,Year,ActivityType,ImplementationPercentage,Efficiency,AdditionalInformationKK,AdditionalInformationRU,ProjectId")] AActivity aActivity,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            if (ModelState.IsValid)
            {
                if (aActivity.ImplementationPercentage == null)
                {
                    aActivity.ImplementationPercentage = 100m;
                }
                if (aActivity.Efficiency == null)
                {
                    aActivity.Efficiency = 0m;
                }
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/AActivities", aActivity);

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
                    return View(aActivity);
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
                        EventIdFilter = ViewBag.EventIdFilter,
                        ProjectIdFilter = ViewBag.ProjectIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        ActivityTypeFilter = ViewBag.ActivityTypeFilter
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
            //ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);

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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(m => m.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == aActivity.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", aActivity.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == aActivity.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", aActivity.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == aActivity.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name", aActivity.EventId);

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", aActivity.ProjectId);

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = new SelectList(targetValues.OrderBy(m => m.Value), "Id", "Value", aActivity.TargetValueId);

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", aActivity.Year);

            ViewBag.StartPeriod = aActivity.StartPeriod.Value.ToString("yyyy-MM-dd");
            ViewBag.EndPeriod = aActivity.EndPeriod.Value.ToString("yyyy-MM-dd");

            return View(aActivity);
        }

        // GET: AActivities/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            AActivity aActivity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/AActivities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                aActivity = await response.Content.ReadAsAsync<AActivity>();
            }

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(t => t.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);

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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(t => t.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == aActivity.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", aActivity.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == aActivity.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", aActivity.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == aActivity.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name", aActivity.EventId);

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", aActivity.ProjectId);

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = new SelectList(targetValues.OrderBy(m => m.Value), "Id", "Value", aActivity.TargetValueId);

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", aActivity.Year);

            aActivity.Target = targets.FirstOrDefault(t => t.Id == aActivity.TargetId);
            aActivity.TargetTerritory = targetTerritories.FirstOrDefault(t => t.Id == aActivity.TargetTerritoryId);

            return View(aActivity);
        }

        // POST: AActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartPeriod,EndPeriod,TargetValueId,TargetId,TargetTerritoryId,EventId,Year,ActivityType,ImplementationPercentage,Efficiency,AdditionalInformationKK,AdditionalInformationRU,ProjectId")] AActivity aActivity,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            if (id != aActivity.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (aActivity.ImplementationPercentage == null)
                {
                    aActivity.ImplementationPercentage = 100m;
                }
                if (aActivity.Efficiency == null)
                {
                    aActivity.Efficiency = 0m;
                }
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/AActivities/{aActivity.Id}", aActivity);

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
                    return View(aActivity);
                }

                aActivity = await response.Content.ReadAsAsync<AActivity>();
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
                        EventIdFilter = ViewBag.EventIdFilter,
                        ProjectIdFilter = ViewBag.ProjectIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        ActivityTypeFilter = ViewBag.ActivityTypeFilter
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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(t => t.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);

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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironmentsUsed.OrderBy(t => t.Name), "Id", "Name", aActivity.Target.PollutionEnvironmentId);
            ViewBag.Targets = new SelectList(targets.Where(t => t.PollutionEnvironmentId == aActivity.Target.PollutionEnvironmentId).OrderBy(t => t.Name), "Id", "Name", aActivity.TargetId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", targets.FirstOrDefault(t => t.Id == aActivity.TargetId).MeasuredParameterUnitId);

            List<TerritoryType> territoryTypes = new List<TerritoryType>();
            string urlTerritoryTypes = "api/TerritoryTypes",
                routeTerritoryTypes = "";
            HttpResponseMessage responseTerritoryTypes = await _HttpApiClient.GetAsync(urlTerritoryTypes + routeTerritoryTypes);
            if (responseTerritoryTypes.IsSuccessStatusCode)
            {
                territoryTypes = await responseTerritoryTypes.Content.ReadAsAsync<List<TerritoryType>>();
            }
            ViewBag.TerritoryTypes = new SelectList(territoryTypes.OrderBy(t => t.Name), "Id", "Name", aActivity.TargetTerritory.TerritoryTypeId);

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.Where(t => t.TerritoryTypeId == aActivity.TargetTerritory.TerritoryTypeId).OrderBy(t => t.Name), "Id", "Name");

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name", aActivity.EventId);

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", aActivity.ProjectId);

            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }
            ViewBag.TargetValues = new SelectList(targetValues.OrderBy(m => m.Value), "Id", "Value", aActivity.TargetValueId);

            ViewBag.Year = new SelectList(Enumerable.Range(Constants.YearMin, Constants.YearMax - Constants.YearMin + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }), "Value", "Text", aActivity.Year);

            return View(aActivity);
        }

        // GET: AActivities/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            if (id == null)
            {
                return NotFound();
            }

            AActivity aActivity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/AActivities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                aActivity = await response.Content.ReadAsAsync<AActivity>();
            }
            if (aActivity == null)
            {
                return NotFound();
            }

            return View(aActivity);
        }

        // POST: AActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? PollutionEnvironmentIdFilter,
            int? TargetIdFilter,
            int? MeasuredParameterUnitIdFilter,
            int? TerritoryTypeIdFilter,
            int? TargetTerritoryIdFilter,
            int? EventIdFilter,
            int? ProjectIdFilter,
            int? YearFilter,
            bool? ActivityTypeFilter,
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
            ViewBag.EventIdFilter = EventIdFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            ViewBag.YearFilter = YearFilter;
            ViewBag.ActivityTypeFilter = ActivityTypeFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/AActivities/{id}");
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
                        EventIdFilter = ViewBag.EventIdFilter,
                        ProjectIdFilter = ViewBag.ProjectIdFilter,
                        YearFilter = ViewBag.YearFilter,
                        ActivityTypeFilter = ViewBag.ActivityTypeFilter
                    });
        }

        //private bool AActivityExists(int id)
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

        [HttpPost]
        public async Task<JsonResult> GetTargetValueTypeById(int TargetValueId)
        {
            List<TargetValue> targetValues = new List<TargetValue>();
            string urlTargetValues = "api/TargetValues",
                routeTargetValues = "";
            HttpResponseMessage responseTargetValues = await _HttpApiClient.GetAsync(urlTargetValues + routeTargetValues);
            if (responseTargetValues.IsSuccessStatusCode)
            {
                targetValues = await responseTargetValues.Content.ReadAsAsync<List<TargetValue>>();
            }

            var targetValueType = targetValues
                .Where(t => t.Id == TargetValueId).FirstOrDefault().TargetValueType;
            JsonResult result = new JsonResult(targetValueType);
            return result;
        }
    }
}

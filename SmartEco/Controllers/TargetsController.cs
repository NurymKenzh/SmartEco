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
    public class TargetsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public TargetsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Targets
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Target> targets = new List<Target>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;

            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.PollutionEnvironmentSort = SortOrder == "PollutionEnvironment" ? "PollutionEnvironmentDesc" : "PollutionEnvironment";

            string url = "api/Targets",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (!string.IsNullOrEmpty(NameKKFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameKK={NameKKFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameKK={NameKKFilter}";
            }
            if (!string.IsNullOrEmpty(NameRUFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameRU={NameRUFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameRU={NameRUFilter}";
            }
            if (!string.IsNullOrEmpty(NameENFilter))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameEN={NameENFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameEN={NameENFilter}";
            }
            if (PollutionEnvironmentIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PollutionEnvironmentId={PollutionEnvironmentIdFilter}";
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
                targets = await response.Content.ReadAsAsync<List<Target>>();
            }
            int targetsCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                targetsCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(targetsCount / (decimal)PageSize) : 1;
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

            return View(targets);
        }

        // GET: Targets/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Target target = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Targets/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                target = await response.Content.ReadAsAsync<Target>();
            }
            if (target == null)
            {
                return NotFound();
            }

            return View(target);
        }

        // GET: Targets/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name");

            Target model = new Target
            {
                TypeOfAchievement = false
            };

            return View(model);
        }

        // POST: Targets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PollutionEnvironmentId,NameKK,NameRU,NameEN,TypeOfAchievement,MeasuredParameterUnitId")] Target target,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Targets", target);

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
                    return View(target);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name");

            return View(target);
        }

        // GET: Targets/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;

            Target target = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Targets/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                target = await response.Content.ReadAsAsync<Target>();
            }

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name", target.PollutionEnvironmentId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", target.MeasuredParameterUnitId);

            return View(target);
        }

        // POST: Targets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PollutionEnvironmentId,NameKK,NameRU,NameEN,TypeOfAchievement,MeasuredParameterUnitId")] Target target,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id != target.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Targets/{target.Id}", target);

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
                    return View(target);
                }

                target = await response.Content.ReadAsAsync<Target>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
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
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name", target.PollutionEnvironmentId);

            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", target.MeasuredParameterUnitId);

            return View(target);
        }

        // GET: Targets/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Target target = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Targets/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                target = await response.Content.ReadAsAsync<Target>();
            }
            if (target == null)
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
            ViewBag.TargetValues = targetValues.Where(t => t.TargetId == id).AsQueryable();

            List<AActivity> aActivities = new List<AActivity>();
            string urlAActivities = "api/AActivities",
                routeAActivities = "";
            HttpResponseMessage responseAActivities = await _HttpApiClient.GetAsync(urlAActivities + routeAActivities);
            if (responseAActivities.IsSuccessStatusCode)
            {
                aActivities = await responseAActivities.Content.ReadAsAsync<List<AActivity>>();
            }
            ViewBag.AActivities = aActivities.Where(t => t.TargetId == id).AsQueryable();

            return View(target);
        }

        // POST: Targets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PollutionEnvironmentIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.PollutionEnvironmentIdFilter = PollutionEnvironmentIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/Targets/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        PollutionEnvironmentIdFilter = ViewBag.PollutionEnvironmentIdFilter
                    });
        }
    }
}

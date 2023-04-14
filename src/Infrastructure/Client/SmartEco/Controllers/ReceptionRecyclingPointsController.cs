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
    public class ReceptionRecyclingPointsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public ReceptionRecyclingPointsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: ReceptionRecyclingPoints
        public async Task<IActionResult> Index(string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<ReceptionRecyclingPoint> receptionRecyclingPoints = new List<ReceptionRecyclingPoint>();

            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;

            ViewBag.OrganizationSort = SortOrder == "Organization" ? "OrganizationDesc" : "Organization";
            ViewBag.TypesRawSort = SortOrder == "TypesRaw" ? "TypesRawDesc" : "TypesRaw";

            string url = "api/ReceptionRecyclingPoints",
                route = "",
                routeCount = "";
            if (OrganizationFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Organization={OrganizationFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Organization={OrganizationFilter}";
            }
            if (TypesRawFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"TypesRaw={TypesRawFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"TypesRaw={TypesRawFilter}";
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
                receptionRecyclingPoints = await response.Content.ReadAsAsync<List<ReceptionRecyclingPoint>>();
            }
            int receptionRecyclingPointCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                receptionRecyclingPointCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(receptionRecyclingPointCount / (decimal)PageSize) : 1;
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

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            return View(receptionRecyclingPoints);
        }

        // GET: ReceptionRecyclingPoints/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            if (id == null)
            {
                return NotFound();
            }

            ReceptionRecyclingPoint receptionRecyclingPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/ReceptionRecyclingPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                receptionRecyclingPoint = await response.Content.ReadAsAsync<ReceptionRecyclingPoint>();
            }
            if (receptionRecyclingPoint == null)
            {
                return NotFound();
            }

            return View(receptionRecyclingPoint);
        }

        // GET: ReceptionRecyclingPoints/Create
        public async Task<IActionResult> Create(string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            ReceptionRecyclingPoint model = new ReceptionRecyclingPoint();

            return View(model);
        }

        // POST: ReceptionRecyclingPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Organization,Address,TypesRaw,NorthLatitude,EastLongitude,ProjectId")] ReceptionRecyclingPoint receptionRecyclingPoint,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/ReceptionRecyclingPoints", receptionRecyclingPoint);

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
                    return View(receptionRecyclingPoint);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        OrganizationFilter = ViewBag.OrganizationFilter,
                        TypesRawFilter = ViewBag.TypesRawFilter
                    });
            }

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", receptionRecyclingPoint.ProjectId);

            return View(receptionRecyclingPoint);
        }

        // GET: ReceptionRecyclingPoints/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            ReceptionRecyclingPoint receptionRecyclingPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/ReceptionRecyclingPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                receptionRecyclingPoint = await response.Content.ReadAsAsync<ReceptionRecyclingPoint>();
            }

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", receptionRecyclingPoint.ProjectId);

            return View(receptionRecyclingPoint);
        }

        // POST: ReceptionRecyclingPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Organization,Address,TypesRaw,NorthLatitude,EastLongitude,ProjectId")] ReceptionRecyclingPoint receptionRecyclingPoint,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            if (id != receptionRecyclingPoint.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/ReceptionRecyclingPoints/{receptionRecyclingPoint.Id}", receptionRecyclingPoint);

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
                    return View(receptionRecyclingPoint);
                }

                receptionRecyclingPoint = await response.Content.ReadAsAsync<ReceptionRecyclingPoint>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        OrganizationFilter = ViewBag.OrganizationFilter,
                        TypesRawFilter = ViewBag.TypesRawFilter
                    });
            }

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", receptionRecyclingPoint.ProjectId);

            return View(receptionRecyclingPoint);
        }

        // GET: ReceptionRecyclingPoints/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            if (id == null)
            {
                return NotFound();
            }

            ReceptionRecyclingPoint receptionRecyclingPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/ReceptionRecyclingPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                receptionRecyclingPoint = await response.Content.ReadAsAsync<ReceptionRecyclingPoint>();
            }
            if (receptionRecyclingPoint == null)
            {
                return NotFound();
            }

            return View(receptionRecyclingPoint);
        }

        // POST: ReceptionRecyclingPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string OrganizationFilter,
            string TypesRawFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.OrganizationFilter = OrganizationFilter;
            ViewBag.TypesRawFilter = TypesRawFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/ReceptionRecyclingPoints/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        OrganizationFilter = ViewBag.OrganizationFilter,
                        TypesRawFilter = ViewBag.TypesRawFilter
                    });
        }
    }
}

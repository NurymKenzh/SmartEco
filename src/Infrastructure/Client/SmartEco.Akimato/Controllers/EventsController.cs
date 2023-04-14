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
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
{
    public class EventsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public EventsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Events
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Event> events = new List<Event>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;

            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.ProjectSort = SortOrder == "Project" ? "ProjectDesc" : "Project";

            string url = "api/Events",
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
            if (NameENFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameEN={NameENFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"NameEN={NameENFilter}";
            }
            if (ProjectIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"ProjectId={ProjectIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"ProjectId={ProjectIdFilter}";
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
                events = await response.Content.ReadAsAsync<List<Event>>();
            }
            int eventCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                eventCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(eventCount / (decimal)PageSize) : 1;
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

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Event eventt = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Events/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                eventt = await response.Content.ReadAsAsync<Event>();
            }
            if (eventt == null)
            {
                return NotFound();
            }

            return View(eventt);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameKK,NameRU,NameEN,ProjectId")] Event eventt,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Events", eventt);

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
                    return View(eventt);
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
                        ProjectIdFilter = ViewBag.ProjectIdFilter
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
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", eventt.ProjectId);

            return View(eventt);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            Event eventt = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Events/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                eventt = await response.Content.ReadAsAsync<Event>();
            }

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", eventt.ProjectId);

            return View(eventt);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameKK,NameRU,NameEN,ProjectId")] Event eventt,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            if (id != eventt.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/Events/{eventt.Id}", eventt);

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
                    return View(eventt);
                }

                eventt = await response.Content.ReadAsAsync<Event>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        ProjectIdFilter = ViewBag.ProjectIdFilter
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
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name", eventt.ProjectId);

            return View(eventt);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            Event eventt = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Events/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                eventt = await response.Content.ReadAsAsync<Event>();
            }
            if (eventt == null)
            {
                return NotFound();
            }

            return View(eventt);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? ProjectIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.ProjectIdFilter = ProjectIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/Events/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        ProjectIdFilter = ViewBag.ProjectIdFilter
                    });
        }
    }
}

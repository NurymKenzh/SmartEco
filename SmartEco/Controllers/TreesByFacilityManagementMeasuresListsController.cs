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
    public class TreesByFacilityManagementMeasuresListsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public TreesByFacilityManagementMeasuresListsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: TreesByFacilityManagementMeasuresLists
        public async Task<IActionResult> Index(string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<TreesByFacilityManagementMeasuresList> treesByFacilityManagementMeasuresLists = new List<TreesByFacilityManagementMeasuresList>();

            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;

            ViewBag.GreemPlantsPassportSort = SortOrder == "GreemPlantsPassport" ? "GreemPlantsPassportDesc" : "GreemPlantsPassport";
            ViewBag.PlantationsTypeSort = SortOrder == "PlantationsType" ? "PlantationsTypeDesc" : "PlantationsType";
            ViewBag.QuantitySort = SortOrder == "Quantity" ? "QuantityDesc" : "Quantity";

            string url = "api/TreesByFacilityManagementMeasuresLists",
                route = "",
                routeCount = "";
            if (GreemPlantsPassportIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"GreemPlantsPassportId={GreemPlantsPassportIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"GreemPlantsPassportId={GreemPlantsPassportIdFilter}";
            }
            if (PlantationsTypeIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PlantationsTypeId={PlantationsTypeIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PlantationsTypeId={PlantationsTypeIdFilter}";
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
                treesByFacilityManagementMeasuresLists = await response.Content.ReadAsAsync<List<TreesByFacilityManagementMeasuresList>>();
            }
            int treesByFacilityManagementMeasuresListCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                treesByFacilityManagementMeasuresListCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(treesByFacilityManagementMeasuresListCount / (decimal)PageSize) : 1;
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

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name");

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject");

            return View(treesByFacilityManagementMeasuresLists);
        }

        // GET: TreesByFacilityManagementMeasuresLists/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByFacilityManagementMeasuresLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByFacilityManagementMeasuresList = await response.Content.ReadAsAsync<TreesByFacilityManagementMeasuresList>();
            }
            if (treesByFacilityManagementMeasuresList == null)
            {
                return NotFound();
            }

            return View(treesByFacilityManagementMeasuresList);
        }

        // GET: TreesByFacilityManagementMeasuresLists/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name");

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject");

            return View();
        }

        // POST: TreesByFacilityManagementMeasuresLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GreemPlantsPassportId,PlantationsTypeId,BusinessEventsPlantationsTypeId,SanitaryPruning,CrownFormation,SanitaryFelling,MaintenanceWork,Quantity,BusinessEvents")] TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/TreesByFacilityManagementMeasuresLists", treesByFacilityManagementMeasuresList);

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
                    return View(treesByFacilityManagementMeasuresList);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreemPlantsPassportIdFilter = ViewBag.GreemPlantsPassportIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
                    });
            }

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByFacilityManagementMeasuresList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByFacilityManagementMeasuresList.GreemPlantsPassportId);

            return View(treesByFacilityManagementMeasuresList);
        }

        // GET: TreesByFacilityManagementMeasuresLists/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByFacilityManagementMeasuresLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByFacilityManagementMeasuresList = await response.Content.ReadAsAsync<TreesByFacilityManagementMeasuresList>();
            }

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByFacilityManagementMeasuresList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByFacilityManagementMeasuresList.GreemPlantsPassportId);

            return View(treesByFacilityManagementMeasuresList);
        }

        // POST: TreesByFacilityManagementMeasuresLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GreemPlantsPassportId,PlantationsTypeId,BusinessEventsPlantationsTypeId,SanitaryPruning,CrownFormation,SanitaryFelling,MaintenanceWork,Quantity,BusinessEvents")] TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id != treesByFacilityManagementMeasuresList.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/TreesByFacilityManagementMeasuresLists/{treesByFacilityManagementMeasuresList.Id}", treesByFacilityManagementMeasuresList);

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
                    return View(treesByFacilityManagementMeasuresList);
                }

                treesByFacilityManagementMeasuresList = await response.Content.ReadAsAsync<TreesByFacilityManagementMeasuresList>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreemPlantsPassportIdFilter = ViewBag.GreemPlantsPassportIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
                    });
            }

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByFacilityManagementMeasuresList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByFacilityManagementMeasuresList.GreemPlantsPassportId);

            return View(treesByFacilityManagementMeasuresList);
        }

        // GET: TreesByFacilityManagementMeasuresLists/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            TreesByFacilityManagementMeasuresList treesByFacilityManagementMeasuresList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByFacilityManagementMeasuresLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByFacilityManagementMeasuresList = await response.Content.ReadAsAsync<TreesByFacilityManagementMeasuresList>();
            }
            if (treesByFacilityManagementMeasuresList == null)
            {
                return NotFound();
            }

            return View(treesByFacilityManagementMeasuresList);
        }

        // POST: TreesByFacilityManagementMeasuresLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/TreesByFacilityManagementMeasuresLists/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GreemPlantsPassportIdFilter = ViewBag.GreemPlantsPassportIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
                    });
        }
    }
}

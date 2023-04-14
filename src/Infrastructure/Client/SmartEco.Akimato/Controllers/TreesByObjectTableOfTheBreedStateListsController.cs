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
    public class TreesByObjectTableOfTheBreedStateListsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public TreesByObjectTableOfTheBreedStateListsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: TreesByObjectTableOfTheBreedStateLists
        public async Task<IActionResult> Index(string SortOrder,
            int? GreemPlantsPassportIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<TreesByObjectTableOfTheBreedStateList> treesByObjectTableOfTheBreedStateLists = new List<TreesByObjectTableOfTheBreedStateList>();

            ViewBag.GreemPlantsPassportIdFilter = GreemPlantsPassportIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;

            ViewBag.GreemPlantsPassportSort = SortOrder == "GreemPlantsPassport" ? "GreemPlantsPassportDesc" : "GreemPlantsPassport";
            ViewBag.PlantationsTypeSort = SortOrder == "PlantationsType" ? "PlantationsTypeDesc" : "PlantationsType";
            ViewBag.QuantitySort = SortOrder == "Quantity" ? "QuantityDesc" : "Quantity";

            string url = "api/TreesByObjectTableOfTheBreedStateLists",
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
                treesByObjectTableOfTheBreedStateLists = await response.Content.ReadAsAsync<List<TreesByObjectTableOfTheBreedStateList>>();
            }
            int treesByObjectTableOfTheBreedStateListCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                treesByObjectTableOfTheBreedStateListCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(treesByObjectTableOfTheBreedStateListCount / (decimal)PageSize) : 1;
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

            return View(treesByObjectTableOfTheBreedStateLists);
        }

        // GET: TreesByObjectTableOfTheBreedStateLists/Details/5
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

            TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByObjectTableOfTheBreedStateLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByObjectTableOfTheBreedStateList = await response.Content.ReadAsAsync<TreesByObjectTableOfTheBreedStateList>();
            }
            if (treesByObjectTableOfTheBreedStateList == null)
            {
                return NotFound();
            }

            return View(treesByObjectTableOfTheBreedStateList);
        }

        // GET: TreesByObjectTableOfTheBreedStateLists/Create
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

            TreesByObjectTableOfTheBreedStateList model = new TreesByObjectTableOfTheBreedStateList();

            return View(model);
        }

        // POST: TreesByObjectTableOfTheBreedStateLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GreemPlantsPassportId,PlantationsTypeId,StateOfCSR15PlantationsTypeId,StateOfCSR15_1,StateOfCSR15_2,StateOfCSR15_3,StateOfCSR15_4,StateOfCSR15_5,Quantity,StateOfCSR15Type")] TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList,
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
                if (treesByObjectTableOfTheBreedStateList.StateOfCSR15Type)
                {
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15PlantationsTypeId = null;
                }
                else
                {
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_1 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_2 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_3 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_4 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_5 = null;
                }

                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/TreesByObjectTableOfTheBreedStateLists", treesByObjectTableOfTheBreedStateList);

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
                    return View(treesByObjectTableOfTheBreedStateList);
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
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByObjectTableOfTheBreedStateList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByObjectTableOfTheBreedStateList.GreemPlantsPassportId);

            return View(treesByObjectTableOfTheBreedStateList);
        }

        // GET: TreesByObjectTableOfTheBreedStateLists/Edit/5
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
            TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByObjectTableOfTheBreedStateLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByObjectTableOfTheBreedStateList = await response.Content.ReadAsAsync<TreesByObjectTableOfTheBreedStateList>();
            }

            if (treesByObjectTableOfTheBreedStateList.StateOfCSR15PlantationsTypeId == null)
            {
                treesByObjectTableOfTheBreedStateList.StateOfCSR15Type = true;
            }
            else
            {
                treesByObjectTableOfTheBreedStateList.StateOfCSR15Type = false;
            }

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByObjectTableOfTheBreedStateList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByObjectTableOfTheBreedStateList.GreemPlantsPassportId);

            return View(treesByObjectTableOfTheBreedStateList);
        }

        // POST: TreesByObjectTableOfTheBreedStateLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GreemPlantsPassportId,PlantationsTypeId,StateOfCSR15PlantationsTypeId,StateOfCSR15_1,StateOfCSR15_2,StateOfCSR15_3,StateOfCSR15_4,StateOfCSR15_5,Quantity,StateOfCSR15Type")] TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList,
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
            if (id != treesByObjectTableOfTheBreedStateList.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (treesByObjectTableOfTheBreedStateList.StateOfCSR15Type)
                {
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15PlantationsTypeId = null;
                }
                else
                {
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_1 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_2 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_3 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_4 = null;
                    treesByObjectTableOfTheBreedStateList.StateOfCSR15_5 = null;
                }

                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/TreesByObjectTableOfTheBreedStateLists/{treesByObjectTableOfTheBreedStateList.Id}", treesByObjectTableOfTheBreedStateList);

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
                    return View(treesByObjectTableOfTheBreedStateList);
                }

                treesByObjectTableOfTheBreedStateList = await response.Content.ReadAsAsync<TreesByObjectTableOfTheBreedStateList>();
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
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", treesByObjectTableOfTheBreedStateList.PlantationsTypeId);

            List<GreemPlantsPassport> greemPlantsPassports = new List<GreemPlantsPassport>();
            string urlGreemPlantsPassports = "api/GreemPlantsPassports",
                routeGreemPlantsPassports = "";
            HttpResponseMessage responseGreemPlantsPassports = await _HttpApiClient.GetAsync(urlGreemPlantsPassports + routeGreemPlantsPassports);
            if (responseGreemPlantsPassports.IsSuccessStatusCode)
            {
                greemPlantsPassports = await responseGreemPlantsPassports.Content.ReadAsAsync<List<GreemPlantsPassport>>();
            }
            ViewBag.GreemPlantsPassports = new SelectList(greemPlantsPassports.OrderBy(m => m.GreenObject), "Id", "GreenObject", treesByObjectTableOfTheBreedStateList.GreemPlantsPassportId);

            return View(treesByObjectTableOfTheBreedStateList);
        }

        // GET: TreesByObjectTableOfTheBreedStateLists/Delete/5
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

            TreesByObjectTableOfTheBreedStateList treesByObjectTableOfTheBreedStateList = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/TreesByObjectTableOfTheBreedStateLists/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                treesByObjectTableOfTheBreedStateList = await response.Content.ReadAsAsync<TreesByObjectTableOfTheBreedStateList>();
            }
            if (treesByObjectTableOfTheBreedStateList == null)
            {
                return NotFound();
            }

            return View(treesByObjectTableOfTheBreedStateList);
        }

        // POST: TreesByObjectTableOfTheBreedStateLists/Delete/5
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
                $"api/TreesByObjectTableOfTheBreedStateLists/{id}");
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

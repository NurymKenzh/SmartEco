using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartEco.Data;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class PlantationsStatesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PlantationsStatesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PlantationsStates
        public async Task<IActionResult> Index(string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PlantationsState> plantationsStates = new List<PlantationsState>();

            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;

            ViewBag.KATOSort = SortOrder == "KATO" ? "KATODesc" : "KATO";
            ViewBag.PlantationsStateTypeSort = SortOrder == "PlantationsStateType" ? "PlantationsStateTypeDesc" : "PlantationsStateType";

            string url = "api/PlantationsStates",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (KATOIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"KATOId={KATOIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"KATOId={KATOIdFilter}";
            }
            if (PlantationsStateTypeIdFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PlantationsStateTypeId={PlantationsStateTypeIdFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"PlantationsStateTypeId={PlantationsStateTypeIdFilter}";
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
                plantationsStates = await response.Content.ReadAsAsync<List<PlantationsState>>();
            }
            int plantationsStateCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                plantationsStateCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(plantationsStateCount / (decimal)PageSize) : 1;
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

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();
            string urlPlantationsStateTypes = "api/PlantationsStateTypes",
                routePlantationsStateTypes = "";
            HttpResponseMessage responsePlantationsStateTypes = await _HttpApiClient.GetAsync(urlPlantationsStateTypes + routePlantationsStateTypes);
            if (responsePlantationsStateTypes.IsSuccessStatusCode)
            {
                plantationsStateTypes = await responsePlantationsStateTypes.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            ViewBag.PlantationsStateTypes = new SelectList(plantationsStateTypes.OrderBy(m => m.Name), "Id", "Name");

            return View(plantationsStates);
        }

        // GET: PlantationsStates/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            PlantationsState plantationsState = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStates/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsState = await response.Content.ReadAsAsync<PlantationsState>();
            }
            if (plantationsState == null)
            {
                return NotFound();
            }

            return View(plantationsState);
        }

        // GET: PlantationsStates/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();
            string urlPlantationsStateTypes = "api/PlantationsStateTypes",
                routePlantationsStateTypes = "";
            HttpResponseMessage responsePlantationsStateTypes = await _HttpApiClient.GetAsync(urlPlantationsStateTypes + routePlantationsStateTypes);
            if (responsePlantationsStateTypes.IsSuccessStatusCode)
            {
                plantationsStateTypes = await responsePlantationsStateTypes.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            ViewBag.PlantationsStateTypes = new SelectList(plantationsStateTypes.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: PlantationsStates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KATOId,PlantationsStateTypeId,TreesNumber")] PlantationsState plantationsState,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/PlantationsStates", plantationsState);

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
                    return View(plantationsState);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsStateTypeIdFilter = ViewBag.PlantationsStateTypeIdFilter
                    });
            }

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", plantationsState.KATOId);

            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();
            string urlPlantationsStateTypes = "api/PlantationsStateTypes",
                routePlantationsStateTypes = "";
            HttpResponseMessage responsePlantationsStateTypes = await _HttpApiClient.GetAsync(urlPlantationsStateTypes + routePlantationsStateTypes);
            if (responsePlantationsStateTypes.IsSuccessStatusCode)
            {
                plantationsStateTypes = await responsePlantationsStateTypes.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            ViewBag.PlantationsStateTypes = new SelectList(plantationsStateTypes.OrderBy(m => m.Name), "Id", "Name", plantationsState.PlantationsStateTypeId);

            return View(plantationsState);
        }

        // GET: PlantationsStates/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            PlantationsState plantationsState = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStates/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsState = await response.Content.ReadAsAsync<PlantationsState>();
            }

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", plantationsState.KATOId);

            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();
            string urlPlantationsStateTypes = "api/PlantationsStateTypes",
                routePlantationsStateTypes = "";
            HttpResponseMessage responsePlantationsStateTypes = await _HttpApiClient.GetAsync(urlPlantationsStateTypes + routePlantationsStateTypes);
            if (responsePlantationsStateTypes.IsSuccessStatusCode)
            {
                plantationsStateTypes = await responsePlantationsStateTypes.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            ViewBag.PlantationsStateTypes = new SelectList(plantationsStateTypes.OrderBy(m => m.Name), "Id", "Name", plantationsState.PlantationsStateTypeId);

            return View(plantationsState);
        }

        // POST: PlantationsStates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KATOId,PlantationsStateTypeId,TreesNumber")] PlantationsState plantationsState,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            if (id != plantationsState.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/PlantationsStates/{plantationsState.Id}", plantationsState);

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
                    return View(plantationsState);
                }

                plantationsState = await response.Content.ReadAsAsync<PlantationsState>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsStateTypeIdFilter = ViewBag.PlantationsStateTypeIdFilter
                    });
            }

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", plantationsState.KATOId);

            List<PlantationsStateType> plantationsStateTypes = new List<PlantationsStateType>();
            string urlPlantationsStateTypes = "api/PlantationsStateTypes",
                routePlantationsStateTypes = "";
            HttpResponseMessage responsePlantationsStateTypes = await _HttpApiClient.GetAsync(urlPlantationsStateTypes + routePlantationsStateTypes);
            if (responsePlantationsStateTypes.IsSuccessStatusCode)
            {
                plantationsStateTypes = await responsePlantationsStateTypes.Content.ReadAsAsync<List<PlantationsStateType>>();
            }
            ViewBag.PlantationsStateTypes = new SelectList(plantationsStateTypes.OrderBy(m => m.Name), "Id", "Name", plantationsState.PlantationsStateTypeId);

            return View(plantationsState);
        }

        // GET: PlantationsStates/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            PlantationsState plantationsState = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PlantationsStates/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                plantationsState = await response.Content.ReadAsAsync<PlantationsState>();
            }
            if (plantationsState == null)
            {
                return NotFound();
            }

            return View(plantationsState);
        }

        // POST: PlantationsStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsStateTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsStateTypeIdFilter = PlantationsStateTypeIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/PlantationsStates/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsStateTypeIdFilter = ViewBag.PlantationsStateTypeIdFilter
                    });
        }

    }
}

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
    public class SpeciesDiversitiesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public SpeciesDiversitiesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: SpeciesDiversities
        public async Task<IActionResult> Index(string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<SpeciesDiversity> speciesDiversities = new List<SpeciesDiversity>();

            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;

            ViewBag.KATOSort = SortOrder == "KATO" ? "KATODesc" : "KATO";
            ViewBag.PlantationsTypeSort = SortOrder == "PlantationsType" ? "PlantationsTypeDesc" : "PlantationsType";

            string url = "api/SpeciesDiversities",
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
                speciesDiversities = await response.Content.ReadAsAsync<List<SpeciesDiversity>>();
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

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name");

            return View(speciesDiversities);
        }

        // GET: SpeciesDiversities/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SpeciesDiversity speciesDiversity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciesDiversities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciesDiversity = await response.Content.ReadAsAsync<SpeciesDiversity>();
            }
            if (speciesDiversity == null)
            {
                return NotFound();
            }

            return View(speciesDiversity);
        }

        // GET: SpeciesDiversities/Create
        public async Task<IActionResult> Create(string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name");

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: SpeciesDiversities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KATOId,PlantationsTypeId,TreesNumber")] SpeciesDiversity speciesDiversity,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/SpeciesDiversities", speciesDiversity);

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
                    return View(speciesDiversity);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
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
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", speciesDiversity.KATOId);

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", speciesDiversity.PlantationsTypeId);

            return View(speciesDiversity);
        }

        // GET: SpeciesDiversities/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            SpeciesDiversity speciesDiversity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciesDiversities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciesDiversity = await response.Content.ReadAsAsync<SpeciesDiversity>();
            }

            List<KATO> KATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                KATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", speciesDiversity.KATOId);

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", speciesDiversity.PlantationsTypeId);

            return View(speciesDiversity);
        }

        // POST: SpeciesDiversities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KATOId,PlantationsTypeId,TreesNumber")] SpeciesDiversity speciesDiversity,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id != speciesDiversity.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/SpeciesDiversities/{speciesDiversity.Id}", speciesDiversity);

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
                    return View(speciesDiversity);
                }

                speciesDiversity = await response.Content.ReadAsAsync<SpeciesDiversity>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
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
            ViewBag.KATOes = new SelectList(KATOes.Where(k => k.ParentEgovId == 17112).OrderBy(m => m.Name), "Id", "Name", speciesDiversity.KATOId);

            List<PlantationsType> plantationsTypes = new List<PlantationsType>();
            string urlPlantationsTypes = "api/PlantationsTypes",
                routePlantationsTypes = "";
            HttpResponseMessage responsePlantationsTypes = await _HttpApiClient.GetAsync(urlPlantationsTypes + routePlantationsTypes);
            if (responsePlantationsTypes.IsSuccessStatusCode)
            {
                plantationsTypes = await responsePlantationsTypes.Content.ReadAsAsync<List<PlantationsType>>();
            }
            ViewBag.PlantationsTypes = new SelectList(plantationsTypes.OrderBy(m => m.Name), "Id", "Name", speciesDiversity.PlantationsTypeId);

            return View(speciesDiversity);
        }

        // GET: SpeciesDiversities/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            if (id == null)
            {
                return NotFound();
            }

            SpeciesDiversity speciesDiversity = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/SpeciesDiversities/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                speciesDiversity = await response.Content.ReadAsAsync<SpeciesDiversity>();
            }
            if (speciesDiversity == null)
            {
                return NotFound();
            }

            return View(speciesDiversity);
        }

        // POST: SpeciesDiversities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? KATOIdFilter,
            int? PlantationsTypeIdFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.KATOIdFilter = KATOIdFilter;
            ViewBag.PlantationsTypeIdFilter = PlantationsTypeIdFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/SpeciesDiversities/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        KATOIdFilter = ViewBag.KATOIdFilter,
                        PlantationsTypeIdFilter = ViewBag.PlantationsTypeIdFilter
                    });
        }

    }
}

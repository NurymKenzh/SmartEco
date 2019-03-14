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
    public class LayersController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public LayersController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Layers
        public async Task<IActionResult> Index(string SortOrder,
            string GeoServerNameFilter,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Layer> layers = new List<Layer>();

            ViewBag.GeoServerNameFilter = GeoServerNameFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            ViewBag.GeoServerNameSort = SortOrder == "GeoServerName" ? "GeoServerNameDesc" : "GeoServerName";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";

            string url = "api/Layers",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (GeoServerNameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"GeoServerName={GeoServerNameFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"GeoServerName={GeoServerNameFilter}";
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
                layers = await response.Content.ReadAsAsync<List<Layer>>();
            }
            int layersCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                layersCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(layersCount / (decimal)PageSize) : 1;
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

            return View(layers);
        }

        // GET: Layers/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string GeoServerNameFilter,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GeoServerNameFilter = GeoServerNameFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id == null)
            {
                return NotFound();
            }

            Layer layer = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Layers/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                layer = await response.Content.ReadAsAsync<Layer>();
            }
            if (layer == null)
            {
                return NotFound();
            }

            return View(layer);
        }

        // GET: Layers/Create
        public async Task<IActionResult> Create(string SortOrder,
            string GeoServerNameFilter,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GeoServerNameFilter = GeoServerNameFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            //ViewData["KATOId"] = new SelectList(_context.Set<KATO>(), "Id", "Id");
            //ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id");
            //ViewData["PollutionEnvironmentId"] = new SelectList(_context.Set<PollutionEnvironment>(), "Id", "Id");
            List<KATO> kATOes = new List<KATO>();
            string urlKATOes = "api/KATOes",
                routeKATOes = "";
            HttpResponseMessage responseKATOes = await _HttpApiClient.GetAsync(urlKATOes + routeKATOes);
            if (responseKATOes.IsSuccessStatusCode)
            {
                kATOes = await responseKATOes.Content.ReadAsAsync<List<KATO>>();
            }
            ViewBag.KATOes = new SelectList(kATOes.OrderBy(k => k.NameRU), "Id", "NameRU");

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = new SelectList(measuredParameters.OrderBy(m => m.Name), "Id", "Name");

            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        // POST: Layers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GeoServerWorkspace,GeoServerName,NameKK,NameRU,NameEN,PollutionEnvironmentId,MeasuredParameterId,KATOId,Season,Hour")] Layer layer,
            string SortOrder,
            string GeoServerNameFilter,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.GeoServerNameFilter = GeoServerNameFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            //if (ModelState.IsValid)
            //{
            //    _context.Add(layer);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["KATOId"] = new SelectList(_context.Set<KATO>(), "Id", "Id", layer.KATOId);
            //ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", layer.MeasuredParameterId);
            //ViewData["PollutionEnvironmentId"] = new SelectList(_context.Set<PollutionEnvironment>(), "Id", "Id", layer.PollutionEnvironmentId);
            //return View(layer);
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/Layers", layer);

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
                    return View(layer);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        GeoServerNameFilter = ViewBag.GeoServerNameFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
            }
            return View(layer);
        }

        //// GET: Layers/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var layer = await _context.Layer.FindAsync(id);
        //    if (layer == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["KATOId"] = new SelectList(_context.Set<KATO>(), "Id", "Id", layer.KATOId);
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", layer.MeasuredParameterId);
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.Set<PollutionEnvironment>(), "Id", "Id", layer.PollutionEnvironmentId);
        //    return View(layer);
        //}

        //// POST: Layers/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,GeoServerWorkspace,GeoServerName,NameKK,NameRU,NameEN,PollutionEnvironmentId,MeasuredParameterId,KATOId,Season,Hour")] Layer layer)
        //{
        //    if (id != layer.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(layer);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!LayerExists(layer.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KATOId"] = new SelectList(_context.Set<KATO>(), "Id", "Id", layer.KATOId);
        //    ViewData["MeasuredParameterId"] = new SelectList(_context.Set<MeasuredParameter>(), "Id", "Id", layer.MeasuredParameterId);
        //    ViewData["PollutionEnvironmentId"] = new SelectList(_context.Set<PollutionEnvironment>(), "Id", "Id", layer.PollutionEnvironmentId);
        //    return View(layer);
        //}

        //// GET: Layers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var layer = await _context.Layer
        //        .Include(l => l.KATO)
        //        .Include(l => l.MeasuredParameter)
        //        .Include(l => l.PollutionEnvironment)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (layer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(layer);
        //}

        //// POST: Layers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var layer = await _context.Layer.FindAsync(id);
        //    _context.Layer.Remove(layer);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool LayerExists(int id)
        //{
        //    return _context.Layer.Any(e => e.Id == id);
        //}
    }
}

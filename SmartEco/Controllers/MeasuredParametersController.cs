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
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class MeasuredParametersController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public MeasuredParametersController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: MeasuredParameters
        public async Task<IActionResult> Index(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;

            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.EcomonCodeSort = SortOrder == "EcomonCode" ? "EcomonCodeDesc" : "EcomonCode";
            ViewBag.OceanusCodeSort = SortOrder == "OceanusCode" ? "OceanusCodeDesc" : "OceanusCode";
            ViewBag.KazhydrometCodeSort = SortOrder == "KazhydrometCode" ? "KazhydrometCodeDesc" : "KazhydrometCode";

            string url = "api/MeasuredParameters",
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
            if (EcomonCodeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EcomonCode={EcomonCodeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"EcomonCode={EcomonCodeFilter}";
            }
            if (OceanusCodeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"OceanusCode={OceanusCodeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"OceanusCode={OceanusCodeFilter}";
            }
            if (KazhydrometCodeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"KazhydrometCode={KazhydrometCodeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"KazhydrometCode={KazhydrometCodeFilter}";
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
                measuredParameters = await response.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            int measuredParametersCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                measuredParametersCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(measuredParametersCount / (decimal)PageSize) : 1;
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

            return View(measuredParameters);
        }

        // GET: MeasuredParameters/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            if (id == null)
            {
                return NotFound();
            }

            MeasuredParameter measuredParameter = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MeasuredParameters/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                measuredParameter = await response.Content.ReadAsAsync<MeasuredParameter>();
            }
            if (measuredParameter == null)
            {
                return NotFound();
            }

            return View(measuredParameter);
        }

        // GET: MeasuredParameters/Create
        public async Task<IActionResult> Create(string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name");
            return View();
        }

        // POST: MeasuredParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeasuredParameterUnitId,NameKK,NameRU,NameEN,MPCDailyAverage,MPCMaxSingle,EcomonCode,OceanusCode,KazhydrometCode")] MeasuredParameter measuredParameter,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/MeasuredParameters", measuredParameter);

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
                    return View(measuredParameter);
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
                        EcomonCodeFilter = ViewBag.EcomonCodeFilter,
                        OceanusCodeFilter = ViewBag.OceanusCodeFilter,
                        KazhydrometCodeFilter = ViewBag.KazhydrometCodeFilter
                    });
            }
            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name");
            return View(measuredParameter);
        }

        // GET: MeasuredParameters/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            MeasuredParameter measuredParameter = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MeasuredParameters/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                measuredParameter = await response.Content.ReadAsAsync<MeasuredParameter>();
            }
            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", measuredParameter.MeasuredParameterUnitId);
            return View(measuredParameter);
        }

        // POST: MeasuredParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeasuredParameterUnitId,NameKK,NameRU,NameEN,MPCDailyAverage,MPCMaxSingle,EcomonCode,OceanusCode,KazhydrometCode")] MeasuredParameter measuredParameter,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            if (id != measuredParameter.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/MeasuredParameters/{measuredParameter.Id}", measuredParameter);

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
                    return View(measuredParameter);
                }

                measuredParameter = await response.Content.ReadAsAsync<MeasuredParameter>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        EcomonCodeFilter = ViewBag.EcomonCodeFilter,
                        OceanusCodeFilter = ViewBag.OceanusCodeFilter,
                        KazhydrometCodeFilter = ViewBag.KazhydrometCodeFilter
                    });
            }
            List<MeasuredParameterUnit> measuredParameterUnits = new List<MeasuredParameterUnit>();
            string urlMeasuredParameterUnits = "api/MeasuredParameterUnits",
                routeMeasuredParameterUnits = "";
            HttpResponseMessage responseMeasuredParameterUnits = await _HttpApiClient.GetAsync(urlMeasuredParameterUnits + routeMeasuredParameterUnits);
            if (responseMeasuredParameterUnits.IsSuccessStatusCode)
            {
                measuredParameterUnits = await responseMeasuredParameterUnits.Content.ReadAsAsync<List<MeasuredParameterUnit>>();
            }
            ViewBag.MeasuredParameterUnits = new SelectList(measuredParameterUnits.OrderBy(m => m.Name), "Id", "Name", measuredParameter.MeasuredParameterUnitId);
            return View(measuredParameter);
        }

        // GET: MeasuredParameters/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            if (id == null)
            {
                return NotFound();
            }

            MeasuredParameter measuredParameter = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/MeasuredParameters/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                measuredParameter = await response.Content.ReadAsAsync<MeasuredParameter>();
            }
            if (measuredParameter == null)
            {
                return NotFound();
            }

            return View(measuredParameter);
        }

        // POST: MeasuredParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? EcomonCodeFilter,
            string OceanusCodeFilter,
            string KazhydrometCodeFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            ViewBag.EcomonCodeFilter = EcomonCodeFilter;
            ViewBag.OceanusCodeFilter = OceanusCodeFilter;
            ViewBag.KazhydrometCodeFilter = KazhydrometCodeFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/MeasuredParameters/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter,
                        EcomonCodeFilter = ViewBag.EcomonCodeFilter,
                        OceanusCodeFilter = ViewBag.OceanusCodeFilter,
                        KazhydrometCodeFilter = ViewBag.KazhydrometCodeFilter
                    });
        }

        //private bool MeasuredParameterExists(int id)
        //{
        //    return _context.MeasuredParameter.Any(e => e.Id == id);
        //}
    }
}

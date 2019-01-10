using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class EcomonMonitoringPointsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public EcomonMonitoringPointsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: EcomonMonitoringPoints
        public async Task<IActionResult> Index(string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<EcomonMonitoringPoint> ecomonMonitoringPoints = new List<EcomonMonitoringPoint>();

            ViewBag.NumberFilter = NumberFilter;

            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";

            string url = "api/EcomonMonitoringPoints",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (NumberFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={NumberFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Number={NumberFilter}";
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
                ecomonMonitoringPoints = await response.Content.ReadAsAsync<List<EcomonMonitoringPoint>>();
            }
            int ecomonMonitoringPointCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                ecomonMonitoringPointCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(ecomonMonitoringPointCount / (decimal)PageSize) : 1;
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

            return View(ecomonMonitoringPoints);
        }

        // GET: EcomonMonitoringPoints/Details/5
        public async Task<IActionResult> Details(int? id,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (id == null)
            {
                return NotFound();
            }

            EcomonMonitoringPoint ecomonMonitoringPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/EcomonMonitoringPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecomonMonitoringPoint = await response.Content.ReadAsAsync<EcomonMonitoringPoint>();
            }
            if (ecomonMonitoringPoint == null)
            {
                return NotFound();
            }

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }

            ViewBag.MeasuredParameters = new SelectList(measuredParameters.OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = (DateTime.Now).ToString("HH:mm:ss");
            return View(ecomonMonitoringPoint);
        }

        // GET: EcomonMonitoringPoints/Create
        public IActionResult Create(string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            return View();
        }

        // POST: EcomonMonitoringPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,NorthLatitude,EastLongitude")] EcomonMonitoringPoint ecomonMonitoringPoint,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/EcomonMonitoringPoints", ecomonMonitoringPoint);

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
                    return View(ecomonMonitoringPoint);
                }

                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
            }
            return View(ecomonMonitoringPoint);
        }

        // GET: EcomonMonitoringPoints/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            EcomonMonitoringPoint ecomonMonitoringPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/EcomonMonitoringPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecomonMonitoringPoint = await response.Content.ReadAsAsync<EcomonMonitoringPoint>();
            }
            return View(ecomonMonitoringPoint);
        }

        // POST: EcomonMonitoringPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,NorthLatitude,EastLongitude")] EcomonMonitoringPoint ecomonMonitoringPoint,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (id != ecomonMonitoringPoint.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/EcomonMonitoringPoints/{ecomonMonitoringPoint.Id}", ecomonMonitoringPoint);

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
                    return View(ecomonMonitoringPoint);
                }

                ecomonMonitoringPoint = await response.Content.ReadAsAsync<EcomonMonitoringPoint>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
            }
            return View(ecomonMonitoringPoint);
        }

        // GET: EcomonMonitoringPoints/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (id == null)
            {
                return NotFound();
            }

            EcomonMonitoringPoint ecomonMonitoringPoint = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/EcomonMonitoringPoints/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                ecomonMonitoringPoint = await response.Content.ReadAsAsync<EcomonMonitoringPoint>();
            }
            if (ecomonMonitoringPoint == null)
            {
                return NotFound();
            }

            return View(ecomonMonitoringPoint);
        }

        // POST: EcomonMonitoringPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/EcomonMonitoringPoints/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
        }

        //private bool EcomonMonitoringPointExists(int id)
        //{
        //    return _context.EcomonMonitoringPoint.Any(e => e.Id == id);
        //}

        [HttpPost]
        public async Task<IActionResult> GetMeasuredDatas(int MeasuredParameterId,
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            MeasuredData[] measureddatas = null;
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            string url = "api/MeasuredDatas",
                route = "";
            // MeasuredParameterId
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterId}";
            }
            // dateTimeFrom
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";
            }
            // dateTimeTo
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";
            }
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
            }
            measureddatas = measuredDatas.ToArray();
            return Json(new
            {
                measureddatas
            });
        }
    }
}

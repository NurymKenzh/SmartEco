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
    public class PollutionEnvironmentsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public PollutionEnvironmentsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: PollutionEnvironments
        public async Task<IActionResult> Index(string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();

            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;

            ViewBag.NameKKSort = NameKKSortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = NameRUSortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = NameENSortOrder == "NameEN" ? "NameENDesc" : "NameEN";

            string url = "api/PollutionEnvironments",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(NameKKSortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameKKSortOrder={NameKKSortOrder}";
            }
            if (!string.IsNullOrEmpty(NameRUSortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameRUSortOrder={NameRUSortOrder}";
            }
            if (!string.IsNullOrEmpty(NameENSortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NameENSortOrder={NameENSortOrder}";
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
                pollutionEnvironments = await response.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            int pollutionEnvironmentCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                pollutionEnvironmentCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(pollutionEnvironmentCount / (decimal)PageSize) : 1;
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

            return View(pollutionEnvironments);
        }

        // GET: PollutionEnvironments/Details/5
        public async Task<IActionResult> Details(int? id,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id == null)
            {
                return NotFound();
            }

            PollutionEnvironment pollutionEnvironment = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionEnvironments/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionEnvironment = await response.Content.ReadAsAsync<PollutionEnvironment>();
            }
            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            //List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            //string urlMeasuredParameters = "api/MeasuredParameters",
            //    routeMeasuredParameters = "";
            //HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            //if (responseMeasuredParameters.IsSuccessStatusCode)
            //{
            //    measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            //}

            return View(pollutionEnvironment);
        }

        // GET: PollutionEnvironments/Create
        public IActionResult Create(string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            return View();
        }

        // POST: PollutionEnvironments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameKK,NameRU,NameEN")] PollutionEnvironment pollutionEnvironment,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (ModelState.IsValid)
            {
                //int logNumber = pollutionEnvironment.Number;
                //decimal logNorthLatitude = pollutionEnvironment.NorthLatitude;
                //decimal logEastLongitude = pollutionEnvironment.EastLongitude;
                //DateTime logDateTimeStart = DateTime.Now;

                //string url = "api/Logs/AddNote",
                //route = "";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"Number={logNumber.ToString()}";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"DateTimeStart={logDateTimeStart.ToString()}";

                //HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                //HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                //    "api/PollutionEnvironments", pollutionEnvironment);

                //string OutputViewText = await response.Content.ReadAsStringAsync();
                //OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                //try
                //{
                //    response.EnsureSuccessStatusCode();
                //}
                //catch
                //{
                //    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                //    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                //    {
                //        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                //    }
                //    return View(pollutionEnvironment);
                //}

                return RedirectToAction(nameof(Index),
                    new
                    {
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        NameENSortOrder = ViewBag.NameENSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
            });
            }
            return View(pollutionEnvironment);
        }

        // GET: PollutionEnvironments/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            PollutionEnvironment pollutionEnvironment = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionEnvironments/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionEnvironment = await response.Content.ReadAsAsync<PollutionEnvironment>();
            }
            return View(pollutionEnvironment);
        }

        // POST: PollutionEnvironments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameKK,NameRU,NameEN")] PollutionEnvironment pollutionEnvironment,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id != pollutionEnvironment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //int logNumber = pollutionEnvironment.Number;
                //decimal logNorthLatitude = pollutionEnvironment.NorthLatitude;
                //decimal logEastLongitude = pollutionEnvironment.EastLongitude;
                //DateTime logDateTimeStart = DateTime.Now;

                //string url = "api/Logs/EditNote",
                //route = "";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"Number={logNumber.ToString()}";

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                //route += string.IsNullOrEmpty(route) ? "?" : "&";
                //route += $"DateTimeStart={logDateTimeStart.ToString()}";

                //HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                //HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                //    $"api/PollutionEnvironments/{pollutionEnvironment.Id}", pollutionEnvironment);

                //string OutputViewText = await response.Content.ReadAsStringAsync();
                //OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                //try
                //{
                //    response.EnsureSuccessStatusCode();
                //}
                //catch
                //{
                //    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                //    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                //    {
                //        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                //    }
                //    return View(pollutionEnvironment);
                //}

                //pollutionEnvironment = await response.Content.ReadAsAsync<PollutionEnvironment>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        NameENSortOrder = ViewBag.NameENSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
            }
            return View(pollutionEnvironment);
        }

        // GET: PollutionEnvironments/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            if (id == null)
            {
                return NotFound();
            }

            PollutionEnvironment pollutionEnvironment = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/PollutionEnvironments/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                pollutionEnvironment = await response.Content.ReadAsAsync<PollutionEnvironment>();
            }
            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            return View(pollutionEnvironment);
        }

        // POST: PollutionEnvironments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string NameENSortOrder,
            string NameKKFilter,
            string NameRUFilter,
            string NameENFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.NameENSortOrder = NameENSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            ViewBag.NameENFilter = NameENFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/PollutionEnvironments/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        NameENSortOrder = ViewBag.NameENSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter,
                        NameENFilter = ViewBag.NameENFilter
                    });
        }

        //private bool PollutionEnvironmentExists(int id)
        //{
        //    return _context.KazHydrometAirPost.Any(e => e.Id == id);
        //}
    }
}

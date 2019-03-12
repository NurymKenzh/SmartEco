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
    public class KazHydrometSoilPostsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public KazHydrometSoilPostsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: KazHydrometSoilPosts
        public async Task<IActionResult> Index(string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();

            ViewBag.NumberFilter = NumberFilter;

            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";

            string url = "api/KazHydrometSoilPosts",
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
                kazHydrometSoilPosts = await response.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            }
            int kazHydrometSoilPostCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                kazHydrometSoilPostCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(kazHydrometSoilPostCount / (decimal)PageSize) : 1;
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

            return View(kazHydrometSoilPosts);
        }

        // GET: KazHydrometSoilPosts/Details/5
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

            KazHydrometSoilPost kazHydrometSoilPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometSoilPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometSoilPost = await response.Content.ReadAsAsync<KazHydrometSoilPost>();
            }
            if (kazHydrometSoilPost == null)
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

            return View(kazHydrometSoilPost);
        }

        // GET: KazHydrometSoilPosts/Create
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

        // POST: KazHydrometSoilPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometSoilPost kazHydrometSoilPost,
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
                int logNumber = kazHydrometSoilPost.Number;
                decimal logNorthLatitude = kazHydrometSoilPost.NorthLatitude;
                decimal logEastLongitude = kazHydrometSoilPost.EastLongitude;
                DateTime logDateTimeStart = DateTime.Now;

                string url = "api/Logs/AddNote",
                route = "";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={logNumber.ToString()}";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeStart={logDateTimeStart.ToString()}";

                HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/KazHydrometSoilPosts", kazHydrometSoilPost);

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
                    return View(kazHydrometSoilPost);
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
            return View(kazHydrometSoilPost);
        }

        // GET: KazHydrometSoilPosts/Edit/5
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
            KazHydrometSoilPost kazHydrometSoilPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometSoilPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometSoilPost = await response.Content.ReadAsAsync<KazHydrometSoilPost>();
            }
            return View(kazHydrometSoilPost);
        }

        // POST: KazHydrometSoilPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometSoilPost kazHydrometSoilPost,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (id != kazHydrometSoilPost.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                int logNumber = kazHydrometSoilPost.Number;
                decimal logNorthLatitude = kazHydrometSoilPost.NorthLatitude;
                decimal logEastLongitude = kazHydrometSoilPost.EastLongitude;
                DateTime logDateTimeStart = DateTime.Now;

                string url = "api/Logs/EditNote",
                route = "";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Number={logNumber.ToString()}";

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"NorthLatitude={logNorthLatitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EastLongitude={logEastLongitude.ToString()}".Replace(',', '.');

                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeStart={logDateTimeStart.ToString()}";

                HttpResponseMessage responseLog = await _HttpApiClient.PostAsync(url + route, null);

                HttpResponseMessage response = await _HttpApiClient.PutAsJsonAsync(
                    $"api/KazHydrometSoilPosts/{kazHydrometSoilPost.Id}", kazHydrometSoilPost);

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
                    return View(kazHydrometSoilPost);
                }

                kazHydrometSoilPost = await response.Content.ReadAsAsync<KazHydrometSoilPost>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
            }
            return View(kazHydrometSoilPost);
        }

        // GET: KazHydrometSoilPosts/Delete/5
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

            KazHydrometSoilPost kazHydrometSoilPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometSoilPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometSoilPost = await response.Content.ReadAsAsync<KazHydrometSoilPost>();
            }
            if (kazHydrometSoilPost == null)
            {
                return NotFound();
            }

            return View(kazHydrometSoilPost);
        }

        // POST: KazHydrometSoilPosts/Delete/5
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
                $"api/KazHydrometSoilPosts/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
        }

        //private bool KazHydrometSoilPostExists(int id)
        //{
        //    return _context.KazHydrometSoilPost.Any(e => e.Id == id);
        //}
    }
}

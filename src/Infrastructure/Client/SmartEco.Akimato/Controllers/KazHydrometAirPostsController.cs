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
    public class KazHydrometAirPostsController : Controller
    {
        //private readonly ApplicationDbContext _context;

        //public KazHydrometAirPostsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: KazHydrometAirPosts
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.KazHydrometAirPost.ToListAsync());
        //}

        //// GET: KazHydrometAirPosts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kazHydrometAirPost = await _context.KazHydrometAirPost
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (kazHydrometAirPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kazHydrometAirPost);
        //}

        //// GET: KazHydrometAirPosts/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: KazHydrometAirPosts/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometAirPost kazHydrometAirPost)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(kazHydrometAirPost);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(kazHydrometAirPost);
        //}

        //// GET: KazHydrometAirPosts/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kazHydrometAirPost = await _context.KazHydrometAirPost.FindAsync(id);
        //    if (kazHydrometAirPost == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(kazHydrometAirPost);
        //}

        //// POST: KazHydrometAirPosts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometAirPost kazHydrometAirPost)
        //{
        //    if (id != kazHydrometAirPost.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(kazHydrometAirPost);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!KazHydrometAirPostExists(kazHydrometAirPost.Id))
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
        //    return View(kazHydrometAirPost);
        //}

        //// GET: KazHydrometAirPosts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kazHydrometAirPost = await _context.KazHydrometAirPost
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (kazHydrometAirPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kazHydrometAirPost);
        //}

        //// POST: KazHydrometAirPosts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var kazHydrometAirPost = await _context.KazHydrometAirPost.FindAsync(id);
        //    _context.KazHydrometAirPost.Remove(kazHydrometAirPost);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool KazHydrometAirPostExists(int id)
        //{
        //    return _context.KazHydrometAirPost.Any(e => e.Id == id);
        //}

        private readonly HttpApiClientController _HttpApiClient;

        public KazHydrometAirPostsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: KazHydrometAirPosts
        public async Task<IActionResult> Index(string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<KazHydrometAirPost> kazHydrometAirPosts = new List<KazHydrometAirPost>();

            ViewBag.NumberFilter = NumberFilter;

            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";

            string url = "api/KazHydrometAirPosts",
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
                kazHydrometAirPosts = await response.Content.ReadAsAsync<List<KazHydrometAirPost>>();
            }
            int kazHydrometAirPostCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                kazHydrometAirPostCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(kazHydrometAirPostCount / (decimal)PageSize) : 1;
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

            return View(kazHydrometAirPosts);
        }

        // GET: KazHydrometAirPosts/Details/5
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

            KazHydrometAirPost kazHydrometAirPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometAirPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometAirPost = await response.Content.ReadAsAsync<KazHydrometAirPost>();
            }
            if (kazHydrometAirPost == null)
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

            return View(kazHydrometAirPost);
        }

        // GET: KazHydrometAirPosts/Create
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

        // POST: KazHydrometAirPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometAirPost kazHydrometAirPost,
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
                //int logNumber = kazHydrometAirPost.Number;
                //decimal logNorthLatitude = kazHydrometAirPost.NorthLatitude;
                //decimal logEastLongitude = kazHydrometAirPost.EastLongitude;
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

                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/KazHydrometAirPosts", kazHydrometAirPost);

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
                    return View(kazHydrometAirPost);
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
            return View(kazHydrometAirPost);
        }

        // GET: KazHydrometAirPosts/Edit/5
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
            KazHydrometAirPost kazHydrometAirPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometAirPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometAirPost = await response.Content.ReadAsAsync<KazHydrometAirPost>();
            }
            return View(kazHydrometAirPost);
        }

        // POST: KazHydrometAirPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Name,AdditionalInformation,NorthLatitude,EastLongitude")] KazHydrometAirPost kazHydrometAirPost,
            string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberFilter = NumberFilter;
            if (id != kazHydrometAirPost.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                int logNumber = kazHydrometAirPost.Number;
                decimal logNorthLatitude = kazHydrometAirPost.NorthLatitude;
                decimal logEastLongitude = kazHydrometAirPost.EastLongitude;
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
                    $"api/KazHydrometAirPosts/{kazHydrometAirPost.Id}", kazHydrometAirPost);

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
                    return View(kazHydrometAirPost);
                }

                kazHydrometAirPost = await response.Content.ReadAsAsync<KazHydrometAirPost>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
            }
            return View(kazHydrometAirPost);
        }

        // GET: KazHydrometAirPosts/Delete/5
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

            KazHydrometAirPost kazHydrometAirPost = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KazHydrometAirPosts/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                kazHydrometAirPost = await response.Content.ReadAsAsync<KazHydrometAirPost>();
            }
            if (kazHydrometAirPost == null)
            {
                return NotFound();
            }

            return View(kazHydrometAirPost);
        }

        // POST: KazHydrometAirPosts/Delete/5
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
                $"api/KazHydrometAirPosts/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
        }

        //private bool KazHydrometAirPostExists(int id)
        //{
        //    return _context.KazHydrometAirPost.Any(e => e.Id == id);
        //}
    }
}

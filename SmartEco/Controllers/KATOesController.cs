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
    public class KATOesController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public KATOesController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: KATOes
        public async Task<IActionResult> Index(string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<KATO> KATOes = new List<KATO>();

            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;

            ViewBag.CodeSort = CodeSortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.LevelSort = LevelSortOrder == "Level" ? "LevelDesc" : "Level";
            ViewBag.NameKKSort = NameKKSortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = NameRUSortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            string url = "api/KATOes",
                route = "",
                routeCount = "";
            if (!string.IsNullOrEmpty(CodeSortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"CodeSortOrder={CodeSortOrder}";
            }
            if (!string.IsNullOrEmpty(LevelSortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"LevelSortOrder={LevelSortOrder}";
            }
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

            if (CodeFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Code={CodeFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Code={CodeFilter}";
            }
            if (LevelFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Level={LevelFilter}";
                routeCount += string.IsNullOrEmpty(routeCount) ? "?" : "&";
                routeCount += $"Level={LevelFilter}";
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
                KATOes = await response.Content.ReadAsAsync<List<KATO>>();
            }
            int KATOCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                KATOCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(KATOCount / (decimal)PageSize) : 1;
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

            return View(KATOes);
        }

        // GET: KATOes/Details/5
        public async Task<IActionResult> Details(int? id,
            string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            if (id == null)
            {
                return NotFound();
            }

            KATO KATO = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KATOes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                KATO = await response.Content.ReadAsAsync<KATO>();
            }
            if (KATO == null)
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

            return View(KATO);
        }

        // GET: KATOes/Create
        public IActionResult Create(string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            return View();
        }

        // POST: KATOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Level,AreaType,EgovId,ParentEgovId,NameKK,NameRU")] KATO KATO,
           string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            if (ModelState.IsValid)
            {
                //int logNumber = KATO.Number;
                //decimal logNorthLatitude = KATO.NorthLatitude;
                //decimal logEastLongitude = KATO.EastLongitude;
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
                //    "api/KATOes", KATO);

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
                //    return View(KATO);
                //}

                return RedirectToAction(nameof(Index),
                    new
                    {
                        CodeSortOrder = ViewBag.CodeSortOrder,
                        LevelSortOrder = ViewBag.LevelSortOrder,
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        CodeFilter = ViewBag.CodeFilter,
                        LevelFilter = ViewBag.LevelFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter
                    });
            }
            return View(KATO);
        }

        // GET: KATOes/Edit/5
        public async Task<IActionResult> Edit(int? id,
            string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            KATO KATO = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KATOes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                KATO = await response.Content.ReadAsAsync<KATO>();
            }
            return View(KATO);
        }

        // POST: KATOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Level,AreaType,EgovId,ParentEgovId,NameKK,NameRU")] KATO KATO,
            string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            if (id != KATO.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //int logNumber = KATO.Number;
                //decimal logNorthLatitude = KATO.NorthLatitude;
                //decimal logEastLongitude = KATO.EastLongitude;
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
                //    $"api/KATOes/{KATO.Id}", KATO);

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
                //    return View(KATO);
                //}

                //KATO = await response.Content.ReadAsAsync<KATO>();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        CodeSortOrder = ViewBag.CodeSortOrder,
                        LevelSortOrder = ViewBag.LevelSortOrder,
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        CodeFilter = ViewBag.CodeFilter,
                        LevelFilter = ViewBag.LevelFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter
                    });
            }
            return View(KATO);
        }

        // GET: KATOes/Delete/5
        public async Task<IActionResult> Delete(int? id,
            string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            if (id == null)
            {
                return NotFound();
            }

            KATO KATO = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/KATOes/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                KATO = await response.Content.ReadAsAsync<KATO>();
            }
            if (KATO == null)
            {
                return NotFound();
            }

            return View(KATO);
        }

        // POST: KATOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            string CodeSortOrder,
            string LevelSortOrder,
            string NameKKSortOrder,
            string NameRUSortOrder,
            string CodeFilter,
            int? LevelFilter,
            string NameKKFilter,
            string NameRUFilter,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.CodeSortOrder = CodeSortOrder;
            ViewBag.LevelSortOrder = LevelSortOrder;
            ViewBag.NameKKSortOrder = NameKKSortOrder;
            ViewBag.NameRUSortOrder = NameRUSortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.CodeFilter = CodeFilter;
            ViewBag.LevelFilter = LevelFilter;
            ViewBag.NameKKFilter = NameKKFilter;
            ViewBag.NameRUFilter = NameRUFilter;
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/KATOes/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        CodeSortOrder = ViewBag.CodeSortOrder,
                        LevelSortOrder = ViewBag.LevelSortOrder,
                        NameKKSortOrder = ViewBag.NameKKSortOrder,
                        NameRUSortOrder = ViewBag.NameRUSortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        CodeFilter = ViewBag.CodeFilter,
                        LevelFilter = ViewBag.LevelFilter,
                        NameKKFilter = ViewBag.NameKKFilter,
                        NameRUFilter = ViewBag.NameRUFilter
                    });
        }

        //private bool KATOExists(int id)
        //{
        //    return _context.KATO.Any(e => e.Id == id);
        //}
    }
}

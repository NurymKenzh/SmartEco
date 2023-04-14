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
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
{
    public class LogsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public LogsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: Logs
        public async Task<IActionResult> Index(string SortOrder,
            int? NumberFilter,
            int? PageSize,
            int? PageNumber)
        {
            List<Log> logs = new List<Log>();

            ViewBag.NumberFilter = NumberFilter;

            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";

            string url = "api/Logs",
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
                logs = await response.Content.ReadAsAsync<List<Log>>();
            }
            int logCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                logCount = await responseCount.Content.ReadAsAsync<int>();
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(logCount / (decimal)PageSize) : 1;
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

            return View(logs);
        }

        // GET: Logs/Details/5
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

            Log log = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Logs/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                log = await response.Content.ReadAsAsync<Log>();
            }
            if (log == null)
            {
                return NotFound();
            }
            return View(log);
        }

        // GET: Logs/Delete/5
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

            Log log = null;
            HttpResponseMessage response = await _HttpApiClient.GetAsync($"api/Logs/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                log = await response.Content.ReadAsAsync<Log>();
            }
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // POST: Logs/Delete/5
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
                $"api/Logs/{id}");
            return RedirectToAction(nameof(Index),
                    new
                    {
                        SortOrder = ViewBag.SortOrder,
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber,
                        NumberFilter = ViewBag.NumberFilter
                    });
        }

        //private bool LogExists(int id)
        //{
        //    return _context.Log.Any(e => e.Id == id);
        //}
    }
}

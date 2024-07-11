using Microsoft.AspNetCore.Mvc;
using SmartEco.Services.ASM;
using SmartEco.Services;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Uprza;
using System.Threading.Tasks;
using SmartEco.Models.ASM.Reports;
using SmartEco.Models.ASM.Requests.Reports;
using System.Net.Http;
using SmartEco.Models.ASM.Responses.Reports;
using System;
using System.Net.Mime;
using System.IO;

namespace SmartEco.Controllers.ASM.Reports
{
    public class ReportsEnterprisesController : Controller
    {
        private readonly string _urlReportsEnterprises = "api/ReportsEnterprises";
        private readonly SmartEcoApi _smartEcoApi;

        public ReportsEnterprisesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: ReportsEnterprise
        public async Task<IActionResult> Index(ReportFilter filter)
        {
            ViewBag.CreatedDateSort = nameof(filter.CreatedDateFilter).FilterSorting(filter.SortOrder);
            ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);

            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var reportEnterprisesViewModel = new ReportEnterpriseListViewModel();

            var reportsEnterpisesRequest = new ReportsEnterpisesRequest()
            {
                SortOrder = filter.SortOrder,
                CreatedDate = filter.CreatedDateFilter,
                Name = filter.NameFilter,
                KatoComplex = filter.KatoComplexFilter,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlReportsEnterprises, reportsEnterpisesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int reportsCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var reportEnterprisesResponse = await response.Content.ReadAsAsync<ReportsEnterprisesResponse>();
                reportEnterprisesViewModel.Items = reportEnterprisesResponse.ReportsEnterprises;
                reportsCount = reportEnterprisesResponse.Count;
            }

            reportEnterprisesViewModel.Pager = new Pager(reportsCount, pager.PageNumber, pager.PageSize);
            reportEnterprisesViewModel.Filter = filter;

            return View(reportEnterprisesViewModel);
        }

        // GET: ReportsEnterprises/CreateParameters
        public async Task<IActionResult> CreateParameters(ReportFilter filter)
        {
            var reportEnterpriseViewModel = new ReportEnterpriseViewModel()
            {
                Filter = filter
            };

            return View(reportEnterpriseViewModel);
        }

        // POST: ReportsEnterprises/CreateParameters
        [HttpPost]
        public async Task<IActionResult> CreateParameters(ReportEnterpriseViewModel reportViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = reportViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlReportsEnterprises}/Parameters", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error creating");
                    return View(reportViewModel);
                }

                return RedirectToAction(nameof(Index), reportViewModel.Filter);
            }

            return View(reportViewModel);
        }

        // GET: ReportsEnterprises/DeleteParameters/5
        public async Task<IActionResult> Delete(ReportFilterId filter)
        {
            var reportEnterpriseViewModel = new ReportEnterpriseViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlReportsEnterprises}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                reportEnterpriseViewModel.Item = await response.Content.ReadAsAsync<ReportEnterprise>();
            }

            reportEnterpriseViewModel.Filter = filter;
            return View(reportEnterpriseViewModel);
        }

        // POST: ReportsEnterprises/DeleteParameters/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ReportEnterpriseViewModel reportEnterpriseViewModel)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlReportsEnterprises}/{reportEnterpriseViewModel.Item.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            return RedirectToAction(nameof(Index), reportEnterpriseViewModel.Filter);
        }

        public async Task<IActionResult> Download(ReportFilterId filter)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlReportsEnterprises}/Download/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
                var reportDownload = await response.Content.ReadAsAsync<ReportDownloadResponse>();
                var resultBytes = reportDownload.FileData;
                if (resultBytes != null && resultBytes.Length > 0)
                {
                    return File(resultBytes, reportDownload.ContentType, reportDownload.FileName);
                }
                else
                {
                    return RedirectToAction(nameof(Index), filter);
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index), filter);
            }
        }
    }
}

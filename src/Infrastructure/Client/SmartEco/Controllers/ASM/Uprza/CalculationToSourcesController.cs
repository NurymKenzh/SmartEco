using Microsoft.AspNetCore.Mvc;
using SmartEco.Extensions.ASM;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using SmartEco.Services.ASM;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class CalculationToSourcesController : Controller
    {
        private readonly string _urlCalcToSrcs = "api/CalculationToSources";
        private readonly SmartEcoApi _smartEcoApi;

        public CalculationToSourcesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetSourcesByCalc(CalculationToSourcesFilter filter)
        {
            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var viewModel = new CalculationToSourcesInvolvedViewModel();

            var calcToSrcsRequest = new CalculationToSourcesRequest()
            {
                CalculationId = filter.CalculationId,
                EnterpriseIds = filter.EnterpriseIds,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalcToSrcs, calcToSrcsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int calcToSrcsCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var calcToSrcsResponse = await response.Content.ReadAsAsync<CalculationToSourcesResponse>();
                viewModel.Items = calcToSrcsResponse.Sources;
                viewModel.IsInvolvedAllSources = calcToSrcsResponse.IsInvolvedAllSorces;
                calcToSrcsCount = calcToSrcsResponse.Count;
            }

            viewModel.Pager = new Pager(calcToSrcsCount, pager.PageNumber, pager.PageSize);
            viewModel.Filter = filter;

            return PartialView("~/Views/Calculations/_AirPollutionSources.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CalculationToSource calcToSrc)
        {
            if (ModelState.IsValid)
            {
                var body = calcToSrc;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlCalcToSrcs, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CalculationToSource calcToSrc)
        {
            var calculationId = calcToSrc.CalculationId;
            var sourceId = calcToSrc.SourceId;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalcToSrcs}/{calculationId}/{sourceId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SelectAll(CalculationToSourcesFilter filter)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlCalcToSrcs}/SelectAll", filter);
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetSourcesByCalc), filter);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll(CalculationToSourcesFilter filter)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalcToSrcs}/{filter.CalculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetSourcesByCalc), filter);
        }
    }
}

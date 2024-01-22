using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Helpers.ASM;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Models.ASM.Responses.Uprza;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using SmartEco.Services.ASM;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class CalculationsController : Controller
    {
        private readonly string _urlCalculations = "api/Calculations";
        private readonly string _urlCalculationTypes = "api/CalculationTypes";
        private readonly string _urlCalculationStatuses = "api/CalculationStatuses";
        private readonly string _urlCalcToEnts = "api/CalculationToEnterprises";
        private readonly string _urlCalcToSrcs = "api/CalculationToSources";
        private readonly string _urlCalcSettings = "api/CalculationSettings";
        private readonly string _urlStateCalcs = "api/StateCalculations";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IUprzaService _uprzaService;

        public CalculationsController(SmartEcoApi smartEcoApi, IUprzaService uprzaService)
        {
            _smartEcoApi = smartEcoApi;
            _uprzaService = uprzaService;
        }

        // GET: Calculations
        public async Task<IActionResult> Index(CalculationFilter filter)
        {
            ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);

            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var calculationsViewModel = new CalculationListViewModel();

            var calculationsRequest = new CalculationsRequest()
            {
                SortOrder = filter.SortOrder,
                Name = filter.NameFilter,
                CalculationTypeId = filter.CalculationTypeIdFilter,
                CalculationStatusId = filter.CalculationStatusIdFilter,
                KatoComplex = filter.KatoComplexFilter,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalculations, calculationsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int calculationsCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var calculationsResponse = await response.Content.ReadAsAsync<CalculationsResponse>();
                calculationsViewModel.Items = calculationsResponse.Calculations;
                calculationsCount = calculationsResponse.Count;
            }

            calculationsViewModel.Pager = new Pager(calculationsCount, pager.PageNumber, pager.PageSize);
            calculationsViewModel.Filter = filter;
            calculationsViewModel.CalculationTypesSelectList = await GetCalculationTypesSelectList();
            calculationsViewModel.CalculationStatusesSelectList = await GetCalculationStatusesSelectList();

            return View(calculationsViewModel);
        }

        // GET: Calculations/Details/5
        public async Task<IActionResult> Details(CalculationFilterId filter)
        {
            if (filter.Id == null)
            {
                return NotFound();
            }

            var calculationDetailViewModel = new CalculationDetailViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalculations}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                calculationDetailViewModel.Item = await response.Content.ReadAsAsync<Calculation>();
            }
            if (calculationDetailViewModel.Item == null)
            {
                return NotFound();
            }

            calculationDetailViewModel.Filter = filter;
            calculationDetailViewModel.Enterprises = await GetEnterprisesByCalc(calculationDetailViewModel.Item.Id);
            var enterpriseIds = calculationDetailViewModel.Enterprises
                .Select(x => x.Id)
                .ToList();
            calculationDetailViewModel.CalcToSrcsViewModel = await GetSourcesByCalc(calculationDetailViewModel.Item.Id, enterpriseIds);
            calculationDetailViewModel.CalcSettingsViewModel = await GetSettingsByCalc(calculationDetailViewModel.Item.Id);

            var airPollutionsSelected = calculationDetailViewModel.CalcSettingsViewModel?.CalculationSetting?.AirPollutantIds;
            calculationDetailViewModel.CalcSettingsViewModel.AirPollutionsSelectList = 
                new MultiSelectList(calculationDetailViewModel.CalcToSrcsViewModel.AirPollutants.OrderBy(m => m.Name), "Id", "CodeName", airPollutionsSelected);

            return View(calculationDetailViewModel);
        }

        // GET: Calculations/Create
        public async Task<IActionResult> Create(CalculationFilter filter)
        {
            var calculationViewModel = new CalculationViewModel()
            {
                Filter = filter,
                CalculationTypesSelectList = await GetCalculationTypesSelectList()
            };

            return View(calculationViewModel);
        }

        // POST: Calculations/Create
        [HttpPost]
        public async Task<IActionResult> Create(CalculationViewModel calculationViewModel)
        {
            if (ModelState.IsValid)
            {
                calculationViewModel.Item.StatusId = (int)CalculationStatuses.New;
                var body = calculationViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlCalculations, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error creating");
                    return View(calculationViewModel);
                }

                calculationViewModel.Item = await response.Content.ReadAsAsync<Calculation>();
                return RedirectToAction(nameof(Index), calculationViewModel.Filter);
            }

            calculationViewModel.CalculationTypesSelectList = await GetCalculationTypesSelectList();
            return View(calculationViewModel);
        }

        // GET: Calculations/Edit/5
        public async Task<IActionResult> Edit(CalculationFilterId filter)
        {
            var calculationViewModel = new CalculationViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalculations}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                calculationViewModel.Item = await response.Content.ReadAsAsync<Calculation>();
            }

            calculationViewModel.Filter = filter;
            calculationViewModel.CalculationTypesSelectList = await GetCalculationTypesSelectList();
            return View(calculationViewModel);
        }

        // POST: Calculations/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(CalculationViewModel calculationViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = calculationViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlCalculations}/{calculationViewModel.Item.Id}", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error editing");
                    return View(calculationViewModel);
                }

                calculationViewModel.Item = await response.Content.ReadAsAsync<Calculation>();
                return RedirectToAction(nameof(Index), calculationViewModel.Filter);
            }
            return View(calculationViewModel);
        }

        // GET: Calculations/Delete/5
        public async Task<IActionResult> Delete(CalculationFilterId filter)
        {
            var calculationViewModel = new CalculationViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalculations}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                calculationViewModel.Item = await response.Content.ReadAsAsync<Calculation>();
            }

            calculationViewModel.Filter = filter;
            return View(calculationViewModel);
        }

        // POST: Calculations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(CalculationViewModel calculationViewModel)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalculations}/{calculationViewModel.Item.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            return RedirectToAction(nameof(Index), calculationViewModel.Filter);
        }

        [HttpPost]
        public async Task<IActionResult> RunCalculation(int calculationId)
        {
            if (calculationId == 0)
                return BadRequest();

            Calculation calculation = null;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalculations}/{calculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                calculation = await response.Content.ReadAsAsync<Calculation>();

            if (calculation == null)
                return NotFound();

            var enterprises = await GetEnterprisesByCalc(calculationId);
            var enterpriseIds = enterprises
                .Select(x => x.Id)
                .ToList();

            var calculationDetailViewModel = new CalculationDetailViewModel
            {
                Item = calculation,
                CalcToSrcsViewModel = await GetSourcesByCalc(calculationId, enterpriseIds),
                CalcSettingsViewModel = await GetSettingsByCalc(calculationId)
            };

            var uprzaRequest = UprzaHelper.MapToRequest(calculationDetailViewModel);
            var uprzaResponse = await _uprzaService.SendCalculation(uprzaRequest, calculation);
            if (uprzaResponse.ErrorMessage != null)
                return BadRequest($"{uprzaResponse.ErrorMessage} - {string.Join(';', uprzaResponse.Description)}");

            await CreateStateCalculation(calculationId, uprzaResponse);

            return Ok();
        }

        private async Task<SelectList> GetCalculationTypesSelectList()
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalculationTypes);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var calculationTypesResponse = await response.Content.ReadAsAsync<CalculationTypesResponse>();
                return new SelectList(calculationTypesResponse.CalculationTypes.OrderBy(m => m.Id), "Id", "Name");
            }
            return null;
        }

        private async Task<SelectList> GetCalculationStatusesSelectList()
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalculationStatuses);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var calculationStatusesResponse = await response.Content.ReadAsAsync<CalculationStatusesResponse>();
                return new SelectList(calculationStatusesResponse.CalculationStatuses.OrderBy(m => m.Id), "Id", "Name");
            }
            return null;
        }

        private async Task<List<Enterprise>> GetEnterprisesByCalc(int calculationId)
        {
            try
            {
                var calcToEntsRequest = new CalculationToEnterprisesRequest()
                {
                    CalculationId = calculationId
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalcToEnts, calcToEntsRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var calcToEntsResponse = await response.Content.ReadAsAsync<CalculationToEnterprisesResponse>();
                return calcToEntsResponse.CalcToEnts
                    .Select(c => c.Enterprise)
                    .ToList();
            }
            catch { return new List<Enterprise>(); }
        }

        private async Task<CalculationToSourcesInvolvedViewModel> GetSourcesByCalc(int calculationId, List<int> enterpriseIds)
        {
            try
            {
                var pager = new Pager(null);
                var viewModel = new CalculationToSourcesInvolvedViewModel();

                var calcToSrcsRequest = new CalculationToSourcesRequest()
                {
                    CalculationId = calculationId,
                    EnterpriseIds = enterpriseIds,
                    PageSize = pager.PageSize,
                    PageNumber = pager.PageNumber,
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalcToSrcs, calcToSrcsRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                int calcToSrcsCount = 0;
                var calcToSrcsResponse = await response.Content.ReadAsAsync<CalculationToSourcesResponse>();
                viewModel.Items = calcToSrcsResponse.Sources;
                calcToSrcsCount = calcToSrcsResponse.Count;

                viewModel.Pager = new Pager(calcToSrcsCount, pager.PageNumber, pager.PageSize);
                viewModel.Filter = new CalculationToSourcesFilter
                {
                    CalculationId = calculationId,
                    EnterpriseIds = enterpriseIds
                };
                viewModel.IsInvolvedAllSources = calcToSrcsResponse.IsInvolvedAllSorces;
                viewModel.AirPollutants = calcToSrcsResponse.AirPollutants;

                return viewModel;
            }
            catch { return new CalculationToSourcesInvolvedViewModel(); }
        }

        private async Task<CalculationSettingsViewModel> GetSettingsByCalc(int calculationId)
        {
            try
            {
                var calcSettingsRequest = new CalculationSettingsRequest()
                {
                    CalculationId = calculationId
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalcSettings, calcSettingsRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var calcSettingsResponse = await response.Content.ReadAsAsync<CalculationSettingsResponse>();
                return new CalculationSettingsViewModel
                {
                    CalculationPoints = calcSettingsResponse.CalcPoints,
                    CalculationRectangles = calcSettingsResponse.CalcRectangles,
                    CalculationSetting = calcSettingsResponse.CalcSetting,
                    StateCalculation = calcSettingsResponse.StateCalculation
                };
            }
            catch { return new CalculationSettingsViewModel(); }
        }

        private async Task CreateStateCalculation(int calculationId, StateCalculation uprzaResponse)
        {
            var body = uprzaResponse;
            var requestStateCalc = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlStateCalcs}/{calculationId}", body);
            await _smartEcoApi.Client.SendAsync(requestStateCalc);
        }
    }
}

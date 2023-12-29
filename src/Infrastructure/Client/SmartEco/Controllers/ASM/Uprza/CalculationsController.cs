using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Models.ASM.Responses.Uprza;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class CalculationsController : Controller
    {
        private readonly string _urlCalculations = "api/Calculations";
        private readonly string _urlCalculationTypes = "api/CalculationTypes";
        private readonly string _urlCalculationStatuses = "api/CalculationStatuses";
        private readonly string _urlCalcToEnts = "api/CalculationToEnterprises";
        private readonly SmartEcoApi _smartEcoApi;

        public CalculationsController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
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
    }
}

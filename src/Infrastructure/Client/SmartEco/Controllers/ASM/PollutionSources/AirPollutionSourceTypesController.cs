using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM.PollutionSources
{
    public class AirPollutionSourceTypesController : Controller
    {
        private readonly string _urlAirPollutionSourceTypes = "api/AirPollutionSourceTypes";
        private readonly SmartEcoApi _smartEcoApi;

        public AirPollutionSourceTypesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: AirPollutionSourceTypes
        public async Task<IActionResult> Index(AirPollutionSourceTypeFilter filter)
        {
            ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);

            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var airPollutionSourceTypesViewModel = new AirPollutionSourceTypeListViewModel();

            var airPollutionSourceTypesRequest = new AirPollutionSourceTypesRequest()
            {
                SortOrder = filter.SortOrder,
                Name = filter.NameFilter,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutionSourceTypes, airPollutionSourceTypesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int airPollutionSourceTypesCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var airPollutionSourceTypesResponse = await response.Content.ReadAsAsync<AirPollutionSourceTypesResponse>();
                airPollutionSourceTypesViewModel.Items = airPollutionSourceTypesResponse.AirPollutionSourceTypes;
                airPollutionSourceTypesCount = airPollutionSourceTypesResponse.Count;
            }

            airPollutionSourceTypesViewModel.Pager = new Pager(airPollutionSourceTypesCount, pager.PageNumber, pager.PageSize);
            airPollutionSourceTypesViewModel.Filter = filter;

            return View(airPollutionSourceTypesViewModel);
        }

        // GET: AirPollutionSourceTypes/Details/5
        public async Task<IActionResult> Details(AirPollutionSourceTypeFilterId filter)
        {
            if (filter.Id == null)
            {
                return NotFound();
            }

            var airPollutionSourceTypeViewModel = new AirPollutionSourceTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSourceTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                airPollutionSourceTypeViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSourceType>();
            }
            if (airPollutionSourceTypeViewModel == null)
            {
                return NotFound();
            }

            airPollutionSourceTypeViewModel.Filter = filter;
            return View(airPollutionSourceTypeViewModel);
        }

        // GET: AirPollutionSourceTypes/Create
        public IActionResult Create(AirPollutionSourceTypeFilter filter)
        {
            var airPollutionSourceTypeViewModel = new AirPollutionSourceTypeViewModel()
            {
                Filter = filter
            };
            return View(airPollutionSourceTypeViewModel);
        }

        // POST: AirPollutionSourceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirPollutionSourceTypeViewModel airPollutionSourceTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = airPollutionSourceTypeViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAirPollutionSourceTypes, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error creating");
                    return View(airPollutionSourceTypeViewModel);
                }

                airPollutionSourceTypeViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSourceType>();
                return RedirectToAction(nameof(Index), airPollutionSourceTypeViewModel.Filter);
            }

            return View(airPollutionSourceTypeViewModel);
        }

        // GET: AirPollutionSourceTypes/Edit/5
        public async Task<IActionResult> Edit(AirPollutionSourceTypeFilterId filter)
        {
            var airPollutionSourceTypeViewModel = new AirPollutionSourceTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSourceTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                airPollutionSourceTypeViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSourceType>();
            }

            airPollutionSourceTypeViewModel.Filter = filter;
            return View(airPollutionSourceTypeViewModel);
        }

        // POST: AirPollutionSourceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirPollutionSourceTypeViewModel airPollutionSourceTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = airPollutionSourceTypeViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlAirPollutionSourceTypes}/{airPollutionSourceTypeViewModel.Item.Id}", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error editing");
                    return View(airPollutionSourceTypeViewModel);
                }

                airPollutionSourceTypeViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSourceType>();
                return RedirectToAction(nameof(Index), airPollutionSourceTypeViewModel.Filter);
            }
            return View(airPollutionSourceTypeViewModel);
        }

        // GET: AirPollutionSourceTypes/Delete/5
        public async Task<IActionResult> Delete(AirPollutionSourceTypeFilterId filter)
        {
            var airPollutionSourceTypeViewModel = new AirPollutionSourceTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSourceTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                airPollutionSourceTypeViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSourceType>();
            }

            airPollutionSourceTypeViewModel.Filter = filter;
            return View(airPollutionSourceTypeViewModel);
        }

        // POST: AirPollutionSourceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(AirPollutionSourceTypeViewModel airPollutionSourceTypeViewModel)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlAirPollutionSourceTypes}/{airPollutionSourceTypeViewModel.Item.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            return RedirectToAction(nameof(Index), airPollutionSourceTypeViewModel.Filter);
        }
    }
}

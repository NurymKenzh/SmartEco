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
    public class AirPollutionSourcesController : Controller
    {
        private readonly string _urlAirPollutionSources = "api/AirPollutionSources";
        private readonly SmartEcoApi _smartEcoApi;

        public AirPollutionSourcesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetSources(AirPollutionSourceFilter filter)
        {
            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var viewModel = new AirPollutionSourceListViewModel();

            var airPollutionSourcesRequest = new AirPollutionSourcesRequest()
            {
                Name = filter.NameFilter,
                Number = filter.NumberFilter,
                Relation = filter.RelationFilter,
                EnterpriseId = filter.EnterpriseId,
                SortOrder = filter.SortOrder,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutionSources, airPollutionSourcesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int airPollutionSourcesCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var airPollutionSourcesResponse = await response.Content.ReadAsAsync<AirPollutionSourcesResponse>();
                viewModel.Items = airPollutionSourcesResponse.AirPollutionSources;
                airPollutionSourcesCount = airPollutionSourcesResponse.Count;
            }

            viewModel.Pager = new Pager(airPollutionSourcesCount, pager.PageNumber, pager.PageSize);
            viewModel.Filter = filter;

            viewModel.Filter.NameSort = nameof(viewModel.Filter.NameSort).Sorting(viewModel.Filter.SortOrder);
            viewModel.Filter.NumberSort = nameof(viewModel.Filter.NumberSort).Sorting(viewModel.Filter.SortOrder);
            viewModel.Filter.RelationSort = nameof(viewModel.Filter.RelationSort).Sorting(viewModel.Filter.SortOrder);

            return PartialView("~/Views/AirPollutionSources/_AirPollutionSourcesTable.cshtml", viewModel);
        }

        // GET: AirPollutionSources
        //public async Task<IActionResult> Index(AirPollutionSourceFilter filter)
        //{
        //    ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);
        //    ViewBag.NumberSort = nameof(filter.NumberFilter).FilterSorting(filter.SortOrder);
        //    ViewBag.RelationSort = nameof(filter.RelationFilter).FilterSorting(filter.SortOrder);

        //    var pager = new Pager(filter.PageNumber, filter.PageSize);
        //    var airPollutionSourcesViewModel = new AirPollutionSourceListViewModel();

        //    var airPollutionSourcesRequest = new AirPollutionSourcesRequest()
        //    {
        //        Name = filter.NameFilter,
        //        Number = filter.NumberFilter,
        //        Relation = filter.RelationFilter,
        //        EnterpriseId = filter.EnterpriseId,
        //        PageSize = pager.PageSize,
        //        PageNumber = pager.PageNumber,
        //    };
        //    var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutionSources, airPollutionSourcesRequest);
        //    var response = await _smartEcoApi.Client.SendAsync(request);

        //    int airPollutionSourcesCount = 0;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var airPollutionSourcesResponse = await response.Content.ReadAsAsync<AirPollutionSourcesResponse>();
        //        airPollutionSourcesViewModel.Items = airPollutionSourcesResponse.AirPollutionSources;
        //        airPollutionSourcesCount = airPollutionSourcesResponse.Count;
        //    }

        //    airPollutionSourcesViewModel.Pager = new Pager(airPollutionSourcesCount, pager.PageNumber, pager.PageSize);
        //    airPollutionSourcesViewModel.Filter = filter;

        //    return PartialView("~/Views/Enterprises/_AirPollutionSources.cshtml", airPollutionSourcesViewModel);
        //}

        // GET: AirPollutionSources/Details/5
        //public async Task<IActionResult> Details(AirPollutionSourceFilterId filter)
        //{
        //    if (filter.Id == null)
        //    {
        //        return NotFound();
        //    }

        //    var airPollutionSourceViewModel = new AirPollutionSourceViewModel();
        //    var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/{filter.Id}");
        //    var response = await _smartEcoApi.Client.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        airPollutionSourceViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSource>();
        //    }
        //    if (airPollutionSourceViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    airPollutionSourceViewModel.Filter = filter;
        //    return View(airPollutionSourceViewModel);
        //}

        // GET: AirPollutionSources/Create
        //public IActionResult Create(BaseFilter filter)
        //{
        //    var airPollutionSourceViewModel = new AirPollutionSourceViewModel()
        //    {
        //        Filter = filter
        //    };
        //    return View(airPollutionSourceViewModel);
        //}

        // POST: AirPollutionSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(AirPollutionSourceViewModel airPollutionSourceViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var body = airPollutionSourceViewModel.Item;
        //        var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAirPollutionSources, body);
        //        var response = await _smartEcoApi.Client.SendAsync(request);

        //        try
        //        {
        //            response.EnsureSuccessStatusCode();
        //        }
        //        catch
        //        {
        //            ModelState.AddModelError(string.Empty, "Error creating");
        //            return View(airPollutionSourceViewModel);
        //        }

        //        airPollutionSourceViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSource>();
        //        return RedirectToAction(nameof(Index), airPollutionSourceViewModel.Filter);
        //    }

        //    return View(airPollutionSourceViewModel);
        //}

        // GET: AirPollutionSources/Edit/5
        //public async Task<IActionResult> Edit(AirPollutionSourceFilterId filter)
        //{
        //    var airPollutionSourceViewModel = new AirPollutionSourceViewModel();
        //    var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/{filter.Id}");
        //    var response = await _smartEcoApi.Client.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        airPollutionSourceViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSource>();
        //    }

        //    airPollutionSourceViewModel.Filter = filter;
        //    return View(airPollutionSourceViewModel);
        //}

        // POST: AirPollutionSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(AirPollutionSourceViewModel airPollutionSourceViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var body = airPollutionSourceViewModel.Item;
        //        var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlAirPollutionSources}/{airPollutionSourceViewModel.Item.Id}", body);
        //        var response = await _smartEcoApi.Client.SendAsync(request);

        //        try
        //        {
        //            response.EnsureSuccessStatusCode();
        //        }
        //        catch
        //        {
        //            ModelState.AddModelError(string.Empty, "Error editing");
        //            return View(airPollutionSourceViewModel);
        //        }

        //        airPollutionSourceViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSource>();
        //        return RedirectToAction(nameof(Index), airPollutionSourceViewModel.Filter);
        //    }
        //    return View(airPollutionSourceViewModel);
        //}

        // GET: AirPollutionSources/Delete/5
        //public async Task<IActionResult> Delete(AirPollutionSourceFilterId filter)
        //{
        //    var airPollutionSourceViewModel = new AirPollutionSourceViewModel();
        //    var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/{filter.Id}");
        //    var response = await _smartEcoApi.Client.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        airPollutionSourceViewModel.Item = await response.Content.ReadAsAsync<AirPollutionSource>();
        //    }

        //    airPollutionSourceViewModel.Filter = filter;
        //    return View(airPollutionSourceViewModel);
        //}

        // POST: AirPollutionSources/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(AirPollutionSourceViewModel airPollutionSourceViewModel)
        //{
        //    var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlAirPollutionSources}/{airPollutionSourceViewModel.Item.Id}");
        //    var response = await _smartEcoApi.Client.SendAsync(request);

        //    return RedirectToAction(nameof(Index), airPollutionSourceViewModel.Filter);
        //}
    }
}

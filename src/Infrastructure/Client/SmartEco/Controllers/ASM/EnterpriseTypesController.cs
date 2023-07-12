using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM
{
    public class EnterpriseTypesController : Controller
    {
        private readonly string _urlEnterpriseTypes = "api/EnterpriseTypes";
        private readonly SmartEcoApi _smartEcoApi;

        public EnterpriseTypesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: EnterpriseTypes
        public async Task<IActionResult> Index(EnterpriseTypeFilter filter)
        {
            ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);

            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var enterpriseTypesViewModel = new EnterpriseTypeListViewModel();

            var enterpriseTypesRequest = new EnterpriseTypesRequest()
            {
                SortOrder = filter.SortOrder,
                Name = filter.NameFilter,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var bodyContent = JsonConvert.SerializeObject(enterpriseTypesRequest);
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlEnterpriseTypes, bodyContent);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int enterpriseTypesCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var enterpriseTypesResponse = await response.Content.ReadAsAsync<EnterpriseTypesResponse>();
                enterpriseTypesViewModel.Items = enterpriseTypesResponse.EnterpriseTypes;
                enterpriseTypesCount = enterpriseTypesResponse.Count;
            }

            enterpriseTypesViewModel.Pager = new Pager(enterpriseTypesCount, pager.PageNumber, pager.PageSize);
            enterpriseTypesViewModel.Filter = filter;

            return View(enterpriseTypesViewModel);
        }

        // GET: EnterpriseTypes/Details/5
        public async Task<IActionResult> Details(EnterpriseTypeFilterId filter)
        {
            if (filter.Id == null)
            {
                return NotFound();
            }

            var enterpriseTypeViewModel = new EnterpriseTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterpriseTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseTypeViewModel.Item = await response.Content.ReadAsAsync<EnterpriseType>();
            }
            if (enterpriseTypeViewModel == null)
            {
                return NotFound();
            }

            enterpriseTypeViewModel.Filter = filter;
            return View(enterpriseTypeViewModel);
        }

        // GET: EnterpriseTypes/Create
        public IActionResult Create(EnterpriseTypeFilter filter)
        {
            var enterpriseTypeViewModel = new EnterpriseTypeViewModel()
            {
                Filter = filter
            };
            return View(enterpriseTypeViewModel);
        }

        // POST: EnterpriseTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnterpriseTypeViewModel enterpriseTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var bodyContent = JsonConvert.SerializeObject(enterpriseTypeViewModel.Item);
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlEnterpriseTypes, bodyContent);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error creating");
                    return View(enterpriseTypeViewModel);
                }

                enterpriseTypeViewModel.Item = await response.Content.ReadAsAsync<EnterpriseType>();
                return RedirectToAction(nameof(Index), enterpriseTypeViewModel.Filter);
            }

            return View(enterpriseTypeViewModel);
        }

        // GET: EnterpriseTypes/Edit/5
        public async Task<IActionResult> Edit(EnterpriseTypeFilterId filter)
        {
            var enterpriseTypeViewModel = new EnterpriseTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterpriseTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseTypeViewModel.Item = await response.Content.ReadAsAsync<EnterpriseType>();
            }

            enterpriseTypeViewModel.Filter = filter;
            return View(enterpriseTypeViewModel);
        }

        // POST: EnterpriseTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EnterpriseTypeViewModel enterpriseTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var bodyContent = JsonConvert.SerializeObject(enterpriseTypeViewModel.Item);
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlEnterpriseTypes}/{enterpriseTypeViewModel.Item.Id}", bodyContent);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error editing");
                    return View(enterpriseTypeViewModel);
                }

                enterpriseTypeViewModel.Item = await response.Content.ReadAsAsync<EnterpriseType>();
                return RedirectToAction(nameof(Index), enterpriseTypeViewModel.Filter);
            }
            return View(enterpriseTypeViewModel);
        }

        // GET: EnterpriseTypes/Delete/5
        public async Task<IActionResult> Delete(EnterpriseTypeFilterId filter)
        {
            var enterpriseTypeViewModel = new EnterpriseTypeViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterpriseTypes}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseTypeViewModel.Item = await response.Content.ReadAsAsync<EnterpriseType>();
            }

            enterpriseTypeViewModel.Filter = filter;
            return View(enterpriseTypeViewModel);
        }

        // POST: EnterpriseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(EnterpriseTypeViewModel enterpriseTypeViewModel)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlEnterpriseTypes}/{enterpriseTypeViewModel.Item.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            return RedirectToAction(nameof(Index), enterpriseTypeViewModel.Filter);
        }
    }
}

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM
{
    public class EnterprisesController : Controller
    {
        private readonly string _urlEnterprises = "api/Enterprises";
        private readonly string _urlEnterpriseTypes = "api/EnterpriseTypes";
        private readonly string _urlKatoByCode = "api/KATOes/GetByCode";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly StatGovKzApi _statGovKzApi;

        public EnterprisesController(SmartEcoApi smartEcoApi, StatGovKzApi statGovKzApi)
        {
            _smartEcoApi = smartEcoApi;
            _statGovKzApi = statGovKzApi;
        }

        // GET: Enterprises
        public async Task<IActionResult> Index(EnterpriseFilter filter)
        {
            ViewBag.NameSort = nameof(filter.NameFilter).FilterSorting(filter.SortOrder);

            var pager = new Pager(filter.PageNumber, filter.PageSize);
            var enterprisesViewModel = new EnterpriseListViewModel();

            var enterprisesRequest = new EnterprisesRequest()
            {
                SortOrder = filter.SortOrder,
                Bin = filter.BinFilter,
                Name = filter.NameFilter,
                KatoComplex = filter.KatoComplexFilter,
                EnterpriseTypeId = filter.EnterpriseTypeIdFilter,
                PageSize = pager.PageSize,
                PageNumber = pager.PageNumber,
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlEnterprises, enterprisesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            int enterprisesCount = 0;
            if (response.IsSuccessStatusCode)
            {
                var enterprisesResponse = await response.Content.ReadAsAsync<EnterprisesResponse>();
                enterprisesViewModel.Items = enterprisesResponse.Enterprises;
                enterprisesCount = enterprisesResponse.Count;
            }

            enterprisesViewModel.Pager = new Pager(enterprisesCount, pager.PageNumber, pager.PageSize);
            enterprisesViewModel.Filter = filter;
            enterprisesViewModel.EnterpriseTypesSelectList = await GetEnterpriseTypesSelectList();

            return View(enterprisesViewModel);
        }

        // GET: Enterprises/Details/5
        public async Task<IActionResult> Details(EnterpriseFilterId filter)
        {
            if (filter.Id == null)
            {
                return NotFound();
            }

            var enterpriseViewModel = new EnterpriseViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterprises}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
            }
            if (enterpriseViewModel == null)
            {
                return NotFound();
            }

            enterpriseViewModel.Filter = filter;
            return View(enterpriseViewModel);
        }

        // GET: Enterprises/Create
        public async Task<IActionResult> Create(EnterpriseFilter filter)
        {
            var enterpriseViewModel = new EnterpriseViewModel()
            {
                Filter = filter,
                EnterpriseTypesSelectList = await GetEnterpriseTypesSelectList()
            };

            return View(enterpriseViewModel);
        }

        // POST: Enterprises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnterpriseViewModel enterpriseViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = enterpriseViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlEnterprises, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error creating");
                    return View(enterpriseViewModel);
                }

                enterpriseViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
                return RedirectToAction(nameof(Index), enterpriseViewModel.Filter);
            }

            return View(enterpriseViewModel);
        }

        // GET: Enterprises/Edit/5
        public async Task<IActionResult> Edit(EnterpriseFilterId filter)
        {
            var enterpriseViewModel = new EnterpriseViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterprises}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
            }

            enterpriseViewModel.Filter = filter;
            enterpriseViewModel.EnterpriseTypesSelectList = await GetEnterpriseTypesSelectList();
            return View(enterpriseViewModel);
        }

        // POST: Enterprises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EnterpriseViewModel enterpriseViewModel)
        {
            if (ModelState.IsValid)
            {
                var body = enterpriseViewModel.Item;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlEnterprises}/{enterpriseViewModel.Item.Id}", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error editing");
                    return View(enterpriseViewModel);
                }

                enterpriseViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
                return RedirectToAction(nameof(Index), enterpriseViewModel.Filter);
            }
            return View(enterpriseViewModel);
        }

        // GET: Enterprises/Delete/5
        public async Task<IActionResult> Delete(EnterpriseFilterId filter)
        {
            var enterpriseViewModel = new EnterpriseViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterprises}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
            }

            enterpriseViewModel.Filter = filter;
            enterpriseViewModel.EnterpriseTypesSelectList = await GetEnterpriseTypesSelectList();
            return View(enterpriseViewModel);
        }

        // POST: Enterprises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(EnterpriseViewModel enterpriseViewModel)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlEnterprises}/{enterpriseViewModel.Item.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);

            return RedirectToAction(nameof(Index), enterpriseViewModel.Filter);
        }

        [HttpPost]
        public async Task<JuridicalAccountResponse> GetEnterpriseFromStatGovKz(long Bin)
        {
            var response = await _statGovKzApi.GetEnterpriseByBin(Bin);
            if (!response.Success || response.Obj is null)
            {
                response.Description = "Ничего не найдено";
            }
            else
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlKatoByCode}/{response.Obj.KatoCode}");
                var responseKato = await _smartEcoApi.Client.SendAsync(request);
                if (responseKato.IsSuccessStatusCode)
                {
                    var kato = await responseKato.Content.ReadAsAsync<KATO>();
                    response.Obj.KatoId = kato.Id;
                    response.Obj.KatoComplex = kato.KatoComplexName;
                }
            }

            return response;
        }

        private async Task<SelectList> GetEnterpriseTypesSelectList()
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlEnterpriseTypes);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var enterpriseTypesResponse = await response.Content.ReadAsAsync<EnterpriseTypesResponse>();
                return new SelectList(enterpriseTypesResponse.EnterpriseTypes.OrderBy(m => m.Name), "Id", "Name");
            }
            return null;
        }
    }
}

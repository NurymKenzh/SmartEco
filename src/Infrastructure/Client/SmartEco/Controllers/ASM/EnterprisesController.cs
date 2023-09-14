using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM
{
    public class EnterprisesController : Controller
    {
        private readonly string _urlEnterprises = "api/Enterprises";
        private readonly string _urlEnterpriseTypes = "api/EnterpriseTypes";
        private readonly string _urlIndSiteEnterprises = "api/IndSiteEnterprises";
        private readonly string _urlWorkshops = "api/Workshops";
        private readonly string _urlAreas = "api/Areas";
        private readonly string _urlAirPollutionSources = "api/AirPollutionSources";
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

            var enterpriseDetailViewModel = new EnterpriseDetailViewModel();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlEnterprises}/{filter.Id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                enterpriseDetailViewModel.Item = await response.Content.ReadAsAsync<Enterprise>();
            }
            if (enterpriseDetailViewModel == null)
            {
                return NotFound();
            }

            enterpriseDetailViewModel.Filter = filter;
            enterpriseDetailViewModel.TreeNodes = await GetTreeNodes(filter.Id.Value);
            enterpriseDetailViewModel.IndSiteEnterprises = await GetIndSiteEnterprises(enterpriseDetailViewModel.Item.Id);
            enterpriseDetailViewModel.AirPollutionSourceListViewModel = await GetAirPollutionSources(enterpriseDetailViewModel.Item.Id);

            return View(enterpriseDetailViewModel);
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
        public async Task<JuridicalAccountResponse> GetEnterpriseFromStatGovKz(string Bin)
        {
            var response = await _statGovKzApi.GetEnterpriseByBin(Bin);
            if (!response.Success || response.Obj is null)
            {
                if (response.Description is null)
                    response.Description = "Ничего не найдено";
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

        private async Task<AirPollutionSourceListViewModel> GetAirPollutionSources(int enterpriseId)
        {
            try
            {
                var pager = new Pager(null);
                var viewModel = new AirPollutionSourceListViewModel();

                var airPollutionSourcesRequest = new AirPollutionSourcesRequest()
                {
                    EnterpriseId = enterpriseId,
                    PageSize = pager.PageSize,
                    PageNumber = pager.PageNumber,
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutionSources, airPollutionSourcesRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                int airPollutionSourcesCount = 0;
                var airPollutionSourcesResponse = await response.Content.ReadAsAsync<AirPollutionSourcesResponse>();
                viewModel.Items = airPollutionSourcesResponse.AirPollutionSources;
                airPollutionSourcesCount = airPollutionSourcesResponse.Count;

                viewModel.Pager = new Pager(airPollutionSourcesCount, pager.PageNumber, pager.PageSize);
                //airPollutionSourcesViewModel.Filter = filter;
                viewModel.Filter = new AirPollutionSourceFilter() 
                {  
                    EnterpriseId = enterpriseId
                };
                viewModel.Filter.NameSort = nameof(viewModel.Filter.NameSort).Sorting(viewModel.Filter.SortOrder);
                viewModel.Filter.NumberSort = nameof(viewModel.Filter.NumberSort).Sorting(viewModel.Filter.SortOrder);
                viewModel.Filter.RelationSort = nameof(viewModel.Filter.RelationSort).Sorting(viewModel.Filter.SortOrder);

                return viewModel;
            }
            catch { return new AirPollutionSourceListViewModel(); }
        }

        #region Composing tree nodes
        private async Task<TreeNodes> GetTreeNodes(int enterpriseId)
        {
            var indSiteEnterprises = await GetIndSiteEnterprises(enterpriseId);
            var workshops = await GetWorkshops(enterpriseId);
            var areas = await GetAreas(enterpriseId);
            return ComposingTreeNodes(indSiteEnterprises, workshops, areas);
        }

        private TreeNodes ComposingTreeNodes(List<IndSiteEnterprise> indSiteEnterprises, List<Workshop> workshops, List<Area> areas)
        {
            var nodes = new TreeNodes();
            foreach (var indSiteEnterprise in indSiteEnterprises)
            {
                var indSiteEnterpriseId = $"indSiteEnterprise_{indSiteEnterprise.Id}";
                var attributes = new { indSiteEnterprise.MinSizeSanitaryZone };
                nodes.Data.Add(AddDataNode(indSiteEnterpriseId, indSiteEnterprise.Name, "/images/ASM/Icons/IndSiteEnterprise.png", null, attributes));

                foreach (var workshop in workshops.Where(w => w.IndSiteEnterpriseId == indSiteEnterprise.Id))
                {
                    var workshopId = $"workshop_{workshop.Id}";
                    nodes.Data.Add(AddDataNode(workshopId, workshop.Name, "/images/ASM/Icons/Workshop.png", indSiteEnterpriseId));

                    foreach (var area in areas.Where(a => a.WorkshopId == workshop.Id))
                    {
                        var areaId = $"area_{area.Id}";
                        nodes.Data.Add(AddDataNode(areaId, area.Name, "/images/ASM/Icons/Area.png", workshopId));
                    }
                }
            }
            return nodes;
        }

        private DataNode AddDataNode(string id, string text, string icon = null, string parent = null, object a_attr = null)
            => new DataNode
            {
                Id = id,
                Parent = parent ?? "#",
                Text = text,
                Icon = icon,
                A_attr = a_attr
            };

        private async Task<List<IndSiteEnterprise>> GetIndSiteEnterprises(int enterpriseId)
        {
            try
            {
                var indSiteEnterprisesRequest = new IndSiteEnterprisesRequest()
                {
                    EnterpriseId = enterpriseId
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlIndSiteEnterprises, indSiteEnterprisesRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var indSiteEnterprisesResponse = await response.Content.ReadAsAsync<List<IndSiteEnterprise>>();
                return indSiteEnterprisesResponse;
            }
            catch { return new List<IndSiteEnterprise>(); }
        }

        private async Task<List<Workshop>> GetWorkshops(int enterpriseId)
        {
            try
            {
                var workshopsRequest = new WorkshopsRequest()
                {
                    EnterpriseId = enterpriseId
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlWorkshops, workshopsRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var workshopsResponse = await response.Content.ReadAsAsync<List<Workshop>>();
                return workshopsResponse;
            }
            catch { return new List<Workshop>(); }
        }

        private async Task<List<Area>> GetAreas(int enterpriseId)
        {
            try
            {
                var areasRequest = new AreasRequest()
                {
                    EnterpriseId = enterpriseId
                };
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAreas, areasRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var areasResponse = await response.Content.ReadAsAsync<List<Area>>();
                return areasResponse;
            }
            catch { return new List<Area>(); }
        }
        #endregion Composing tree nodes
    }
}

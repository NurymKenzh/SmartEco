using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartEco.Models.ASM;
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
        private readonly string _urlAirPollutionSourceTypes = "api/AirPollutionSourceTypes";
        private readonly string _urlIndSiteEnterprises = "api/IndSiteEnterprises";
        private readonly string _urlWorkshops = "api/Workshops";
        private readonly string _urlAreas = "api/Areas";
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

            viewModel.DropdownTypes = await GetAirPollutionSourceTypes();
            viewModel.DropdownIndSite = await GetIndSiteEnterprises(filter.EnterpriseId);
            viewModel.DropdownWorkShop = await GetWorkshops(filter.EnterpriseId);
            viewModel.DropdownArea = await GetAreas(filter.EnterpriseId);

            var relationBackgrounds = await GetRelationBackgrounds();
            viewModel.Items.ForEach(source => source.SourceInfo.DropdownBackgrounds = relationBackgrounds);

            return PartialView("~/Views/AirPollutionSources/_AirPollutionSources.cshtml", viewModel);
        }

        // POST: AirPollutionSources/Create
        [HttpPost]
        public async Task<IActionResult> Create(int enterpriseId, int pageSize)
        {
            try
            {
                var viewModel = new AirPollutionSourceListViewModel();
                var sourceLastNumberResponse = await GetLastNumber(enterpriseId);

                //Building a new item
                var sourceTypes = await GetAirPollutionSourceTypes();
                var indSiteEnterprises = await GetIndSiteEnterprises(enterpriseId);

                var airPollutionSource = CreateSourceObject(sourceLastNumberResponse.Number, sourceTypes.First());
                airPollutionSource.SourceInfo = CreateSourceInfoObject();

                if (indSiteEnterprises.Any())
                    airPollutionSource.SourceIndSite = new AirPollutionSourceIndSite() { IndSiteEnterpriseId = indSiteEnterprises.First().Id };
                else
                    return BadRequest();

                //Create new item
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAirPollutionSources, airPollutionSource);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                //Create pager for calculating number last page
                //Need for move to last page after creating new item
                var pager = new Pager(sourceLastNumberResponse.Count + 1, null, pageSize);
                var filter = new AirPollutionSourceFilter()
                {
                    EnterpriseId = enterpriseId,
                    PageNumber = pager.TotalPages,
                    PageSize = pageSize
                };
                return RedirectToAction(nameof(GetSources), filter);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: AirPollutionSources/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(AirPollutionSource airPollutionSource)
        {
            if (ModelState.IsValid)
            {
                var body = airPollutionSource;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlAirPollutionSources}/{airPollutionSource.Id}", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    if (response.StatusCode is HttpStatusCode.Conflict)
                        ModelState.AddModelError(nameof(airPollutionSource.Number), "Номер должен быть уникальным");
                    else
                        ModelState.AddModelError(string.Empty, "Error editing");
                    return BadRequest(ModelState);
                }

                return Ok();
            }
            return BadRequest(ModelState);
        }

        // POST: AirPollutionSources/Copy
        [HttpPost]
        public async Task<IActionResult> Copy(int enterpriseId, int pageSize, AirPollutionSource airPollutionSource)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sourceLastNumberResponse = await GetLastNumber(enterpriseId);

                    //Create new item
                    airPollutionSource = ChangeSourceForCopy(airPollutionSource, sourceLastNumberResponse.Number);
                    var requestCreate = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAirPollutionSources, airPollutionSource);
                    var responseCreate = await _smartEcoApi.Client.SendAsync(requestCreate);
                    responseCreate.EnsureSuccessStatusCode();

                    //Create pager for calculating number last page
                    //Need for move to last page after creating new item
                    var pager = new Pager(sourceLastNumberResponse.Count + 1, null, pageSize);
                    var filter = new AirPollutionSourceFilter()
                    {
                        EnterpriseId = enterpriseId,
                        PageNumber = pager.TotalPages,
                        PageSize = pageSize
                    };
                    return RedirectToAction(nameof(GetSources), filter);
                }
                catch { return BadRequest(); }
            }
            return BadRequest(ModelState);
        }

        // POST: AirPollutionSources/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id, AirPollutionSourceFilter filter)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlAirPollutionSources}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var sourceLastNumberResponse = await GetLastNumber(filter.EnterpriseId);
            //Create pager for calculating number last page
            //Need for move to last page after creating new item
            var pager = new Pager(sourceLastNumberResponse.Count, null, filter.PageSize);
            filter.PageNumber = filter.PageNumber > pager.TotalPages ? pager.TotalPages : filter.PageNumber;

            return RedirectToAction(nameof(GetSources), filter);
        }

        [HttpPost]
        public IActionResult ValidInfo(AirPollutionSourceInfo airPollutionSourceInfo)
        {
            if (ModelState.IsValid)
                return Ok();

            return BadRequest(ModelState);
        }

        private async Task<AirPollutinSourceLastNumberResponse> GetLastNumber(int? enterpriseId)
        {
            //Get last number for create new item
            //Get count items for calculate total pages
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/GetLastNumber/{enterpriseId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AirPollutinSourceLastNumberResponse>();
        }

        private AirPollutionSource CreateSourceObject(int lastNumber, AirPollutionSourceType type)
            => new AirPollutionSource()
            {
                Number = (++lastNumber).ToString().PadLeft(4, '0'),
                Name = "ИЗА",
                IsActive = true,
                TypeId = type.Id
            };

        private AirPollutionSourceInfo CreateSourceInfoObject()
            => new AirPollutionSourceInfo()
            {
                TerrainCoefficient = 1,
                AngleDeflection = 0,
                AngleRotation = 0,
                Hight = 0,
                Diameter = 0,
                RelationBackgroundId = 1
            };

        private AirPollutionSource ChangeSourceForCopy(AirPollutionSource source, int lastNumber)
        {
            source.Number = (++lastNumber).ToString().PadLeft(4, '0');
            source.Id = 0;
            source.SourceInfo.SourceId = 0;

            if (source.Relation is SourceRelations.IndSite)
                source.SourceIndSite.AirPollutionSourceId = 0;
            else if (source.Relation is SourceRelations.Workshop)
                source.SourceWorkshop.AirPollutionSourceId = 0;
            else
                source.SourceArea.AirPollutionSourceId = 0;

            return source;

        }

        private async Task<List<AirPollutionSourceType>> GetAirPollutionSourceTypes()
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutionSourceTypes);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var airPollutionSourceTypesResponse = await response.Content.ReadAsAsync<AirPollutionSourceTypesResponse>();

                return airPollutionSourceTypesResponse.AirPollutionSourceTypes ?? new List<AirPollutionSourceType>();
            }
            catch { return new List<AirPollutionSourceType>(); }
        }

        private async Task<List<RelationBackground>> GetRelationBackgrounds()
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/GetRelationBackgrounds");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var relationBackgroundsResponse = await response.Content.ReadAsAsync<List<RelationBackground>>();

                return relationBackgroundsResponse ?? new List<RelationBackground>();
            }
            catch { return new List<RelationBackground>(); }
        }

        private async Task<List<IndSiteEnterprise>> GetIndSiteEnterprises(int? enterpriseId)
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

        private async Task<List<Workshop>> GetWorkshops(int? enterpriseId)
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

        private async Task<List<Area>> GetAreas(int? enterpriseId)
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
    }
}

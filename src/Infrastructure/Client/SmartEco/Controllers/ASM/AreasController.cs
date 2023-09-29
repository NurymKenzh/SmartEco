using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM
{
    public class AreasController : Controller
    {
        private readonly string _urlAreas = "api/Areas";
        private readonly SmartEcoApi _smartEcoApi;
        public AreasController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: Areas
        [HttpGet]
        public async Task<List<Area>> List(int? EnterpriseId)
        {
            var areasRequest = new AreasRequest()
            {
                EnterpriseId = EnterpriseId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAreas, areasRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var areas = await response.Content.ReadAsAsync<List<Area>>();
                return areas ?? new List<Area>();
            }

            return new List<Area>();
        }

        // GET: Areas/Details/5
        [HttpGet(Name = nameof(Details))]
        public async Task<Area> Details(int? id)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAreas}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var area = await response.Content.ReadAsAsync<Area>();
                return area ?? new Area();
            }

            return new Area();
        }

        // POST: Areas/Create
        [HttpPost(Name = nameof(Create))]
        public async Task<IActionResult> Create(Area area)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = area;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAreas, body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var enterpriseResponse = await response.Content.ReadAsAsync<EnterpriseResponse>();
                    return RedirectToAction("Details", "Enterprises", new { id = enterpriseResponse.Id });
                }
                catch
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        // POST: Areas/Edit/5
        [HttpPost(Name = nameof(Edit))]
        public async Task<IActionResult> Edit(Area area)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = area;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlAreas}/{area.Id}", body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var enterpriseResponse = await response.Content.ReadAsAsync<EnterpriseResponse>();
                    return RedirectToAction("Details", "Enterprises", new { id = enterpriseResponse.Id });
                }
                catch
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        // POST: Areas/Delete/5
        [HttpPost(Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int? id, int enterpriseId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlAreas}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                var enterpriseResponse = await response.Content.ReadAsAsync<EnterpriseResponse>();
                return RedirectToAction("Details", "Enterprises", new { id = enterpriseResponse.Id });
            }
            catch
            {
                if (response.StatusCode is HttpStatusCode.Forbidden)
                    return RedirectToAction("Details", "Enterprises", new EnterpriseFilterId { Id = enterpriseId, IsCannotDelete = true });

                return BadRequest();
            }
        }
    }
}

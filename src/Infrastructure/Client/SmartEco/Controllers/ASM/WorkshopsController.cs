using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM;
using SmartEco.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using System.Net;

namespace SmartEco.Controllers.ASM
{
    public class WorkshopsController : Controller
    {
        private readonly string _urlWorkshops = "api/Workshops";
        private readonly SmartEcoApi _smartEcoApi;

        public WorkshopsController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: Workshops
        [HttpGet]
        public async Task<List<Workshop>> List(int? EnterpriseId)
        {
            var workshopsRequest = new WorkshopsRequest()
            {
                EnterpriseId = EnterpriseId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlWorkshops, workshopsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var workshops = await response.Content.ReadAsAsync<List<Workshop>>();
                return workshops ?? new List<Workshop>();
            }

            return new List<Workshop>();
        }

        // GET: Workshops/Details/5
        [HttpGet(Name = nameof(Details))]
        public async Task<Workshop> Details(int? id)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlWorkshops}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var workshop = await response.Content.ReadAsAsync<Workshop>();
                return workshop ?? new Workshop();
            }

            return new Workshop();
        }

        // POST: Workshops/Create
        [HttpPost(Name = nameof(Create))]
        public async Task<IActionResult> Create(Workshop workshop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = workshop;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlWorkshops, body);
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

        // POST: Workshops/Edit/5
        [HttpPost(Name = nameof(Edit))]
        public async Task<IActionResult> Edit(Workshop workshop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = workshop;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlWorkshops}/{workshop.Id}", body);
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

        // POST: Workshops/Delete/5
        [HttpPost(Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int? id, int enterpriseId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlWorkshops}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                var enterpriseResponse = await response.Content.ReadAsAsync<EnterpriseResponse>();
                return RedirectToAction("Details", "Enterprises", new EnterpriseFilterId { Id = enterpriseResponse.Id });
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

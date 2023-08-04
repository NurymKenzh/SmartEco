using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Requests;
using SmartEco.Services;

namespace SmartEco.Controllers.ASM
{
    public class IndSiteEnterprisesController : Controller
    {
        private readonly string _urlIndSiteEnterprises = "api/IndSiteEnterprises";
        private readonly SmartEcoApi _smartEcoApi;

        public IndSiteEnterprisesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        // GET: IndSiteEnterprises
        [HttpGet]
        public async Task<List<IndSiteEnterprise>> List(int? EnterpriseId)
        {
            var indSiteEnterprisesRequest = new IndSiteEnterprisesRequest()
            {
                EnterpriseId = EnterpriseId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlIndSiteEnterprises, indSiteEnterprisesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var indSiteEnterprises = await response.Content.ReadAsAsync<List<IndSiteEnterprise>>();
                return indSiteEnterprises ?? new List<IndSiteEnterprise>();
            }

            return new List<IndSiteEnterprise>();
        }

        // GET: IndSiteEnterprises/Details/5
        [HttpGet(Name = nameof(Details))]
        public async Task<IndSiteEnterprise> Details(int? id)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlIndSiteEnterprises}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var indSiteEnterprise = await response.Content.ReadAsAsync<IndSiteEnterprise>();
                return indSiteEnterprise ?? new IndSiteEnterprise();
            }

            return new IndSiteEnterprise();
        }

        // POST: IndSiteEnterprises/Create
        [HttpPost(Name = nameof(Create))]
        public async Task<IActionResult> Create(IndSiteEnterprise indSiteEnterprise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = indSiteEnterprise;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlIndSiteEnterprises, body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    return RedirectToAction("Details", "Enterprises", new { id = indSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        // POST: IndSiteEnterprises/Edit/5
        [HttpPost(Name = nameof(Edit))]
        public async Task<IActionResult> Edit(IndSiteEnterprise indSiteEnterprise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = indSiteEnterprise;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlIndSiteEnterprises}/{indSiteEnterprise.Id}", body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    return RedirectToAction("Details", "Enterprises", new { id = indSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        // POST: IndSiteEnterprises/Delete/5
        [HttpPost(Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int? id, int enterpriseId)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlIndSiteEnterprises}/{id}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Details", "Enterprises", new { id = enterpriseId });
            }
            catch
            {

                return BadRequest();
            }
        }
    }
}

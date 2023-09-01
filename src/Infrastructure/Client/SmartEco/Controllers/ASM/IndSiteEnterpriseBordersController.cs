using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM;
using System.Threading.Tasks;
using SmartEco.Services;
using System.Net.Http;
using System.Collections.Generic;
using SmartEco.Models.ASM.Requests;

namespace SmartEco.Controllers.ASM
{
    public class IndSiteEnterpriseBordersController : Controller
    {
        private readonly string _urlIndSiteEnterpriseBorders = "api/IndSiteEnterpriseBorders";
        private readonly SmartEcoApi _smartEcoApi;

        public IndSiteEnterpriseBordersController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int IndSiteEnterpriseId, int EnterpriseId)
        {
            var viewModel = new IndSiteEnterpriseBorderListViewModel();

            var bordersRequest = new IndSiteEnterpriseBordersRequest()
            {
                IndSiteEnterpriseId = IndSiteEnterpriseId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlIndSiteEnterpriseBorders, bordersRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var borders = await response.Content.ReadAsAsync<List<IndSiteEnterpriseBorder>>();
                viewModel.Items = borders;
            }
            viewModel.IndSiteEnterpriseId = IndSiteEnterpriseId;
            viewModel.EnterpriseId = EnterpriseId;

            return View(viewModel);
        }

        public IActionResult Create(int IndSiteEnterpriseId)
        {
            var viewModel = new IndSiteEnterpriseBorder()
            {
                IndSiteEnterpriseId = IndSiteEnterpriseId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndSiteEnterpriseBorder indSiteEnterpriseBorder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = indSiteEnterpriseBorder;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlIndSiteEnterpriseBorders, body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var borderResponse = await response.Content.ReadAsAsync<IndSiteEnterpriseBorder>();
                    return RedirectToAction("Details", "Enterprises", new { id = borderResponse.IndSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return View(indSiteEnterpriseBorder);
                }
            }

            return View(indSiteEnterpriseBorder);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlIndSiteEnterpriseBorders}/{id}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var indSiteEnterpriseBorder = await response.Content.ReadAsAsync<IndSiteEnterpriseBorder>();
                return View(indSiteEnterpriseBorder);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IndSiteEnterpriseBorder indSiteEnterpriseBorder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = indSiteEnterpriseBorder;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlIndSiteEnterpriseBorders}/{indSiteEnterpriseBorder.Id}", body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var borderResponse = await response.Content.ReadAsAsync<IndSiteEnterpriseBorder>();
                    return RedirectToAction("Details", "Enterprises", new { id = borderResponse.IndSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return View(indSiteEnterpriseBorder);
                }
            }
            return View(indSiteEnterpriseBorder);
        }

        [HttpPost(Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int? id, int IndSiteEnterpriseId, int EnterpriseId)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlIndSiteEnterpriseBorders}/{id}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Details", "Enterprises", new { id = EnterpriseId });
            }
            catch
            {

                return BadRequest();
            }
        }
    }
}

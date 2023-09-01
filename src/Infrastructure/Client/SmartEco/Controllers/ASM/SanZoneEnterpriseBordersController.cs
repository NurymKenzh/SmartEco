using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM;
using System.Threading.Tasks;
using SmartEco.Services;
using System.Net.Http;
using System.Collections.Generic;
using SmartEco.Models.ASM.Requests;

namespace SmartEco.Controllers.ASM
{
    public class SanZoneEnterpriseBordersController : Controller
    {
        private readonly string _urlSanZoneEnterpriseBorders = "api/SanZoneEnterpriseBorders";
        private readonly SmartEcoApi _smartEcoApi;

        public SanZoneEnterpriseBordersController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int IndSiteEnterpriseId, int EnterpriseId)
        {
            var viewModel = new SanZoneEnterpriseBorderListViewModel();

            var bordersRequest = new SanZoneEnterpriseBordersRequest()
            {
                IndSiteEnterpriseId = IndSiteEnterpriseId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlSanZoneEnterpriseBorders, bordersRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var borders = await response.Content.ReadAsAsync<List<SanZoneEnterpriseBorder>>();
                viewModel.Items = borders;
            }
            viewModel.IndSiteEnterpriseId = IndSiteEnterpriseId;
            viewModel.EnterpriseId = EnterpriseId;

            return View(viewModel);
        }

        public IActionResult Create(int IndSiteEnterpriseId)
        {
            var viewModel = new SanZoneEnterpriseBorder()
            {
                IndSiteEnterpriseId = IndSiteEnterpriseId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanZoneEnterpriseBorder sanZoneEnterpriseBorder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = sanZoneEnterpriseBorder;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlSanZoneEnterpriseBorders, body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var borderResponse = await response.Content.ReadAsAsync<SanZoneEnterpriseBorder>();
                    return RedirectToAction("Details", "Enterprises", new { id = borderResponse.IndSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return View(sanZoneEnterpriseBorder);
                }
            }

            return View(sanZoneEnterpriseBorder);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlSanZoneEnterpriseBorders}/{id}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var sanZoneEnterpriseBorder = await response.Content.ReadAsAsync<SanZoneEnterpriseBorder>();
                return View(sanZoneEnterpriseBorder);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SanZoneEnterpriseBorder sanZoneEnterpriseBorder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = sanZoneEnterpriseBorder;
                    var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlSanZoneEnterpriseBorders}/{sanZoneEnterpriseBorder.Id}", body);
                    var response = await _smartEcoApi.Client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                    var borderResponse = await response.Content.ReadAsAsync<SanZoneEnterpriseBorder>();
                    return RedirectToAction("Details", "Enterprises", new { id = borderResponse.IndSiteEnterprise.EnterpriseId });
                }
                catch
                {
                    return View(sanZoneEnterpriseBorder);
                }
            }
            return View(sanZoneEnterpriseBorder);
        }

        [HttpPost(Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int? id, int IndSiteEnterpriseId, int EnterpriseId)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlSanZoneEnterpriseBorders}/{id}");
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

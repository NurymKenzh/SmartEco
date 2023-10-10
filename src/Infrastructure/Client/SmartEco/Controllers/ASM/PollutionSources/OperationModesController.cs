using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.PollutionSources
{
    public class OperationModesController : Controller
    {
        private readonly string _urlOperationModes = "api/OperationModes";
        private readonly SmartEcoApi _smartEcoApi;

        public OperationModesController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetModes(OperationModesRequest operationModesRequest)
        {
            var operationModes = new List<OperationMode>();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlOperationModes, operationModesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var operationModesResponse = await response.Content.ReadAsAsync<List<OperationMode>>();
                operationModes = operationModesResponse;
            }

            return PartialView("~/Views/AirPollutionSources/_OperationModesTable.cshtml", operationModes);
        }

        // POST: OperationModes/Create
        [HttpPost]
        public async Task<IActionResult> Create(int sourceId)
        {
            try
            {
                //Building a new item
                var operationMode = CreateModeObject(sourceId);
                operationMode.GasAirMixture = new GasAirMixture();

                //Create new item
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlOperationModes, operationMode);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(GetModes), new OperationModesRequest { SourceId = sourceId });
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: OperationModes/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(OperationMode operationMode)
        {
            if (ModelState.IsValid)
            {
                var body = operationMode;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlOperationModes}/{operationMode.Id}", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error editing");
                    return BadRequest(ModelState);
                }

                return Ok();
            }
            return BadRequest(ModelState);
        }

        // POST: AirPollutionSources/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int sourceId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlOperationModes}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetModes), new OperationModesRequest { SourceId = sourceId });
        }

        private OperationMode CreateModeObject(int sourceId)
            => new OperationMode()
            {
                Name = "Режим работы",
                WorkedTime = 0,
                SourceId = sourceId
            };
    }
}

using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.PollutionSources
{
    public class AirEmissionsController : Controller
    {
        private readonly string _urlAirEmissions = "api/AirEmissions";
        private readonly string _urlAirPollutants = "api/AirPollutants";
        private readonly SmartEcoApi _smartEcoApi;

        public AirEmissionsController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmissions(AirEmissionsRequest airEmissionsRequest)
        {
            var airEmissions = new List<AirEmission>();
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirEmissions, airEmissionsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var airEmissionsResponse = await response.Content.ReadAsAsync<List<AirEmission>>();
                airEmissions = airEmissionsResponse;
            }

            return PartialView("~/Views/AirPollutionSources/_AirEmissionsTable.cshtml", airEmissions);
        }

        // POST: AirEmissions/Create
        [HttpPost]
        public async Task<IActionResult> Create(int operationModeId)
        {
            try
            {
                //Building a new item
                var airEmission = await CreateEmissionObject(operationModeId);

                //Create new item
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlAirEmissions, airEmission);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(GetEmissions), new AirEmissionsRequest { OperationModeId = operationModeId });
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: AirEmissions/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(AirEmission airEmission)
        {
            if (ModelState.IsValid)
            {
                var body = airEmission;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlAirEmissions}/{airEmission.Id}", body);
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
        public async Task<IActionResult> Delete(int id, int operationModeId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlAirEmissions}/{id}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetEmissions), new AirEmissionsRequest { OperationModeId = operationModeId });
        }

        private async Task<AirEmission> CreateEmissionObject(int operationModeId)
            => new AirEmission()
            {
                PollutantId = await GetPollutantFirstId(),
                OperationModeId = operationModeId,
                SettlingCoef = 1,
                EnteredDate = DateTime.Now
            };

        private async Task<int> GetPollutantFirstId()
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutants}/GetFirst");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var airPollutant = await response.Content.ReadAsAsync<AirPollutant>();
            return airPollutant.Id;
        }
    }
}

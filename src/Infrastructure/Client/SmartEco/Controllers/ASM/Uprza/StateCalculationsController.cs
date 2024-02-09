using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using SmartEco.Services.ASM;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class StateCalculationsController : Controller
    {
        private readonly string _urlStateCalcs = "api/StateCalculations";
        private readonly string _urlCalculations = "api/Calculations";
        private readonly string _urlAirPollutants = "api/AirPollutants";
        private readonly string _urlResultEmissions = "api/ResultEmissions";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IUprzaService _uprzaService;

        public StateCalculationsController(SmartEcoApi smartEcoApi, IUprzaService uprzaService)
        {
            _smartEcoApi = smartEcoApi;
            _uprzaService = uprzaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetState(int calculationId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlStateCalcs}/{calculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var stateCalcResponse = await response.Content.ReadAsAsync<StateCalculation>();

            return PartialView("~/Views/Calculations/_StateCalculationTable.cshtml", stateCalcResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int calculationId, int jobId)
        {
            var calculation = await GetCalculation(calculationId);
            if (calculation is null)
                return NotFound();

            var stateCalc = await _uprzaService.GetStatusCalculation(jobId, calculation);
            if(stateCalc is null)
                return RedirectToAction(nameof(GetState), new { calculationId });

            var body = stateCalc;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlStateCalcs}/{calculationId}", body);
            await _smartEcoApi.Client.SendAsync(request);

            //For setting result emissions for view features on map
            if (stateCalc.Calculation.StatusId == (int)CalculationStatuses.Done)
            {
                //To get using pollutants in UPRZA calculation
                var uprzaPollutantsResp = await _uprzaService.GetCalculationPollutants(jobId);
                if (uprzaPollutantsResp is null)
                    return RedirectToAction(nameof(GetState), new { calculationId });

                //To get only codes from using pollutants
                var uprzaPollutantCodes = uprzaPollutantsResp.CalculationPollutants
                    .Select(p => p.Code)
                    .ToList();

                //To get air pollutants from DB
                //To create result emissions
                var airPollutants = await GetPollutantsByCodes(uprzaPollutantCodes);
                var resultEmissions = new List<ResultEmission>();
                foreach(var airPollutant in airPollutants)
                {
                    var pollutantCode = airPollutant.Code;
                    var uprzaResultEmission = await _uprzaService.GetResultEmission(jobId, pollutantCode);
                    resultEmissions.Add(new ResultEmission
                    {
                        CalculationId = calculationId,
                        AirPollutantId = airPollutants.Single(p => p.Code == pollutantCode).Id,
                        FeatureCollection = uprzaResultEmission
                    });
                }
                await CreateResultEmissions(calculationId, resultEmissions);
            }

            return RedirectToAction(nameof(GetState), new { calculationId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int calculationId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlStateCalcs}/{calculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetState), new { calculationId });
        }

        private async Task<Calculation> GetCalculation(int calculationId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalculations}/{calculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsAsync<Calculation>();
        }

        private async Task<List<AirPollutant>> GetPollutantsByCodes(List<int> pollutantCodes)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutants}/ByCodes", pollutantCodes);
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsAsync<List<AirPollutant>>();
        }

        private async Task<IActionResult> CreateResultEmissions(int calculationId, List<ResultEmission> resultEmissions)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlResultEmissions}/{calculationId}", resultEmissions);
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return Ok();
        }
    }
}

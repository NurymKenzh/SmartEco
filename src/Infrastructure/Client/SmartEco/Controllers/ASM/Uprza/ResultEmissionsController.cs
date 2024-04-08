using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class ResultEmissionsController : Controller
    {
        private readonly string _urlResultEmissions = "api/ResultEmissions";
        private readonly string _urlIndSiteEnterprises = "api/IndSiteEnterprises";
        private readonly string _urlAirPollutionSources = "api/AirPollutionSources";
        private readonly SmartEcoApi _smartEcoApi;

        public ResultEmissionsController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> DispersionPollutants(int calcId)
        {
            var resultEmissionViewModel = new ResultEmissionViewModel
            {
                CalculationId = calcId,
                AirPollutantsSelectList = await GetAirPollutantsSelectList(calcId),
                IndSiteEnterprises = await GetIndSiteEnterprises(calcId),
                AirPollutionSources = await GetAirPollutionSources(calcId)
            };
            return View(resultEmissionViewModel);
        }

        [HttpGet]
        public async Task<ResultEmission> GetEmissionByCode(int calculationId, int pollutantCode)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlResultEmissions}/{calculationId}/{pollutantCode}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<ResultEmission>();
            }
            catch { return null; }
        }

        private async Task<SelectList> GetAirPollutantsSelectList(int calculationId)
        {
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlResultEmissions}/{calculationId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var airPollutants = await response.Content.ReadAsAsync<List<AirPollutant>>();
                return new SelectList(airPollutants.OrderBy(m => m.Code), "Code", "CodeName");
            }
            return null;
        }

        private async Task<List<IndSiteEnterprise>> GetIndSiteEnterprises(int calculationId)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlIndSiteEnterprises}/GetByCalculation/{calculationId}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<IndSiteEnterprise>>();
            }
            catch { return new List<IndSiteEnterprise>(); }
        }

        private async Task<List<AirPollutionSource>> GetAirPollutionSources(int calculationId)
        {
            try
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlAirPollutionSources}/GetByCalculation/{calculationId}");
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<AirPollutionSource>>();
            }
            catch { return new List<AirPollutionSource>(); }
        }
    }
}

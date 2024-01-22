using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using SmartEco.Services.ASM;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class StateCalculationsController : Controller
    {
        private readonly string _urlStateCalcs = "api/StateCalculations";
        private readonly string _urlCalculations = "api/Calculations";
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
            var request = _smartEcoApi.CreateRequest(HttpMethod.Put, $"{_urlStateCalcs}/{calculationId}", body);
            await _smartEcoApi.Client.SendAsync(request);

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
    }
}

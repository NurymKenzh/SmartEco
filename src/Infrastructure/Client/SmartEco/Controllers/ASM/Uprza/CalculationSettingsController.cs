using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Helpers.ASM;
using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class CalculationSettingsController : Controller
    {
        private readonly string _urlCalcSettings = "api/CalculationSettings";
        private readonly string _urlCalcToSrcs = "api/CalculationToSources";
        private readonly SmartEcoApi _smartEcoApi;

        public CalculationSettingsController(SmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAirPollutants(int calculationId, List<int> enterpriseIds, List<int> airPollutionsSelected)
        {
            var airPollutants = new List<AirPollutant>();
            var calcToSrcsRequest = new CalculationToSourcesRequest()
            {
                CalculationId = calculationId,
                EnterpriseIds = enterpriseIds
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalcToSrcs}/AirPollutants", calcToSrcsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                airPollutants = await response.Content.ReadAsAsync<List<AirPollutant>>();

            var selectList = new MultiSelectList(airPollutants.OrderBy(m => m.Name), "Id", "CodeName", airPollutionsSelected);
            return PartialView("~/Views/Calculations/_CalculationSettingAirPollutants.cshtml", selectList);
        }

        [HttpPost]
        public async Task<IActionResult> SetCalculationSetting(CalculationSetting calcSetting)
        {
            var message = "Настройки не валидны. Проверьте значения параметров.";
            if (ModelState.IsValid && UprzaHelper.IsSettingValid(calcSetting, out message))
            {
                var body = calcSetting;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlCalcSettings, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                    return Ok();
                }
                catch
                {
                    message = "Не удалось применить настройки. Ошибка соединения";
                }
            }

            var errorPair = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToList()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.FirstOrDefault()?.ErrorMessage)
                .FirstOrDefault();
            if (errorPair.Key != null)
                message = $"{errorPair.Key.Split('.').LastOrDefault()} - {errorPair.Value}";

            return BadRequest(message);
        }

        #region Points
        [HttpGet]
        public async Task<IActionResult> GetCalculationPoints(int calculationId)
        {
            var calcPointsRequest = new CalculationSettingsRequest()
            {
                CalculationId = calculationId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalcSettings}/Points", calcPointsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var calcPointsResponse = await response.Content.ReadAsAsync<CalculationSettingsResponse>();

            return PartialView("~/Views/Calculations/_CalculationPointsTable.cshtml",
                calcPointsResponse.CalcPoints);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCalculationPoint(CalculationPoint calcPoint)
        {
            if (ModelState.IsValid)
            {
                var body = calcPoint;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlCalcSettings}/Point", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                    return RedirectToAction(nameof(GetCalculationPoints), new { calculationId = calcPoint.CalculationId });
                }
                catch
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCalculationPoint(CalculationPoint calcPoint)
        {
            var calculationId = calcPoint.CalculationId;
            var number = calcPoint.Number;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalcSettings}/Point/{calculationId}/{number}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetCalculationPoints), new { calculationId = calcPoint.CalculationId });
        }
        #endregion Points

        #region Rectangle
        [HttpGet]
        public async Task<IActionResult> GetCalculationRectangles(int calculationId)
        {
            var calcRectanglesRequest = new CalculationSettingsRequest()
            {
                CalculationId = calculationId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, $"{_urlCalcSettings}/Rectangles", calcRectanglesRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var calcRectanglesResponse = await response.Content.ReadAsAsync<CalculationSettingsResponse>();

            return PartialView("~/Views/Calculations/_CalculationRectanglesTable.cshtml",
                calcRectanglesResponse.CalcRectangles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCalculationRectangle(CalculationRectangle calcRectangle)
        {
            if (ModelState.IsValid)
            {
                var body = calcRectangle;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, $"{_urlCalcSettings}/Rectangle", body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                    return RedirectToAction(nameof(GetCalculationRectangles), new { calculationId = calcRectangle.CalculationId });
                }
                catch
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCalculationRectangle(CalculationRectangle calcRectangle)
        {
            var calculationId = calcRectangle.CalculationId;
            var number = calcRectangle.Number;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalcSettings}/Rectangle/{calculationId}/{number}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetCalculationRectangles), new { calculationId = calcRectangle.CalculationId });
        }
        #endregion Rectangle
    }
}

using Microsoft.AspNetCore.Mvc;
using SmartEco.Extensions.ASM;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Requests;
using SmartEco.Models.ASM.Responses;
using SmartEco.Models.ASM.Uprza;
using SmartEco.Services;
using SmartEco.Services.ASM;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.Uprza
{
    public class CalculationToEnterprisesController : Controller
    {
        private readonly string _urlCalcToEnts = "api/CalculationToEnterprises";
        private readonly string _urlEnterprises = "api/Enterprises";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IKatoService _katoService;

        public CalculationToEnterprisesController(SmartEcoApi smartEcoApi, IKatoService katoService)
        {
            _smartEcoApi = smartEcoApi;
            _katoService = katoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnterprisesByCalc(int calculationId)
        {
            var calcToEntsRequest = new CalculationToEnterprisesRequest()
            {
                CalculationId = calculationId
            };
            var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlCalcToEnts, calcToEntsRequest);
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var calcToEntsResponse = await response.Content.ReadAsAsync<CalculationToEnterprisesResponse>();

            return PartialView("~/Views/Calculations/_EnterprisesTable.cshtml",
                calcToEntsResponse.CalcToEnts
                .Select(c => c.Enterprise)
                .ToList());
        }

        [HttpGet]
        public async Task<List<Enterprise>> GetEnterprisesByKato(string enterpriseBinName, string calcKatoCode, int calcTypeId, List<int> enterpriseIds)
        {
            if (IsOnlyOneAllowed(calcTypeId, enterpriseIds.Count))
                return new List<Enterprise>();

            var enterprises = await GetEnterprises(enterpriseBinName);
            var katoCatalogs = await _katoService.GetKatoCatalogs();
            var katoDownHierarchy = katoCatalogs.MapToChildrenHierarchy();
            var rootKatoDownHierarchy = katoDownHierarchy.GetRootKatoMatch(calcKatoCode).Single(); //get unique root KATO

            //Get KATO, that equals calculation KATO-code
            rootKatoDownHierarchy = rootKatoDownHierarchy
                .RecursiveDeep(k => k.Children)
                .Single(kato => kato.Code == calcKatoCode);

            //Get all list KATO with children for filtering enterprises
            var katoDownHierarchyCodes = rootKatoDownHierarchy
                .RecursiveDeep(k => k.Children)
                .GroupBy(kato => new { kato.Code })
                .Select(group => group.First().Code)
                .ToList();

            //Filtering enterprises, that contain necessary KATO-codes and excludes those already used
            return enterprises
                .Where(e => katoDownHierarchyCodes.Contains(e.Kato.Code) && !enterpriseIds.Contains(e.Id))
                .ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CalculationToEnterprise calcToEnt)
        {
            if (ModelState.IsValid)
            {
                var body = calcToEnt;
                var request = _smartEcoApi.CreateRequest(HttpMethod.Post, _urlCalcToEnts, body);
                var response = await _smartEcoApi.Client.SendAsync(request);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    return BadRequest();
                }
            }

            return RedirectToAction(nameof(GetEnterprisesByCalc), new { calculationId = calcToEnt.CalculationId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CalculationToEnterprise calcToEnt)
        {
            var calculationId = calcToEnt.CalculationId;
            var enterpriseId = calcToEnt.EnterpriseId;
            var request = _smartEcoApi.CreateRequest(HttpMethod.Delete, $"{_urlCalcToEnts}/{calculationId}/{enterpriseId}");
            var response = await _smartEcoApi.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(GetEnterprisesByCalc), new { calculationId });
        }

        private static bool IsOnlyOneAllowed(int calcTypeId, int countEnterprises)
            => calcTypeId == (int)CalculationTypes.MpeProject && countEnterprises >= 1;

        private async Task<List<Enterprise>> GetEnterprises(string enterpriseBinName)
        {
            try
            {
                var enterprisesRequest = new EnterprisesRequest();
                if (enterpriseBinName.All(char.IsDigit))
                    enterprisesRequest.Bin = enterpriseBinName;
                else
                    enterprisesRequest.Name = enterpriseBinName;

                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlEnterprises, enterprisesRequest);
                var response = await _smartEcoApi.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var enterprisesResponse = await response.Content.ReadAsAsync<EnterprisesResponse>();
                return enterprisesResponse.Enterprises;
            }
            catch { return new List<Enterprise>(); }
        }
    }
}

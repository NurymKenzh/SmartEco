using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SmartEco.Extensions.ASM;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;
using SmartEco.Services.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM
{
    public class KatoCatalogsController : Controller
    {
        private readonly IKatoService _katoService;

        public KatoCatalogsController(IKatoService katoService)
        {
            _katoService = katoService;
        }

        [HttpGet]
        public async Task<List<KatoCatalog>> GetKatoCatalogs(string katoCodeName)
        {
            katoCodeName = katoCodeName.ToLower().Trim();
            if (string.IsNullOrEmpty(katoCodeName))
                return null;

            var katoCatalogs = await _katoService.GetKatoCatalogs();
            var katoEnterprises = await _katoService.GetKatoEnterprises();
            var katoUpHierarchy = katoCatalogs.MapToParentHierarchy();
            var katoDownHierarchy = katoCatalogs.MapToChildrenHierarchy();

            var codes = katoEnterprises.GetCodes();
            var rootKatoDownHierarchy = katoDownHierarchy.GetRootKatoMatch(katoCodeName);
            var rootKatoChilrenDownHierarchy = rootKatoDownHierarchy.GetRootKatoChildren(codes);
            katoUpHierarchy = katoUpHierarchy.GetKatoContainChildren(rootKatoChilrenDownHierarchy);
            var katoParents = katoUpHierarchy.GetParents().ToList();
            return katoParents
                .GroupBy(kato => new { kato.Code })
                .Select(group => group.First())
                .Where(kato => kato.Code.StartsWith(katoCodeName) || kato.Name.ToLower().StartsWith(katoCodeName))
                .OrderBy(kato => kato.Code)
                .ToList();
        }

        [HttpGet]
        public async Task<KatoCatalog> GetKatoCatalog(string katoCodeName)
        {
            if (string.IsNullOrEmpty(katoCodeName))
                return null;

            var katoCatalogs = await _katoService.GetKatoCatalogs();
            return katoCatalogs
                .SingleOrDefault(kato => kato.CodeName.Equals(katoCodeName));
        }
    }
}

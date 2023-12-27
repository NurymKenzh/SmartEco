using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SmartEco.Extensions.ASM;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Responses;
using SmartEco.Services;
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
        private readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(20);
        private readonly string _cacheKeyKatoCatalog = "KatoCatalog";
        private readonly string _cacheKeyKatoEnterprises = "KatoEnterprises";

        private readonly string _urlKatoCatalogs = "api/KatoCatalogs";
        private readonly string _urlEnterprises = "api/Enterprises";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IDistributedCache _cache;

        public KatoCatalogsController(SmartEcoApi smartEcoApi, IDistributedCache cache)
        {
            _smartEcoApi = smartEcoApi;
            _cache = cache;
        }

        [HttpGet]
        public async Task<List<KatoCatalog>> GetKatoCatalogs(string katoCodeName)
        {
            katoCodeName = katoCodeName.ToLower().Trim();
            if (string.IsNullOrEmpty(katoCodeName))
                return null;

            var katoCatalogs = await GetKatoCatalogs();
            var katoEnterprises = await GetKatoEnterprises();
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

            var katoCatalogs = await GetKatoCatalogs();
            return katoCatalogs
                .SingleOrDefault(kato => kato.CodeName.Equals(katoCodeName));
        }

        private async Task<List<KatoCatalog>> GetKatoCatalogs()
        {
            var katoCatalogs = new List<KatoCatalog>();
            var katoCatalogsBytes = await _cache.GetAsync(_cacheKeyKatoCatalog);
            if (katoCatalogsBytes is null)
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlKatoCatalogs);
                var response = await _smartEcoApi.Client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    katoCatalogs = await response.Content.ReadAsAsync<List<KatoCatalog>>();
                    SetCacheKatoes(_cacheKeyKatoCatalog, katoCatalogs);
                }
            }
            else
            {
                var json = Encoding.UTF8.GetString(katoCatalogsBytes);
                katoCatalogs = JsonConvert.DeserializeObject<List<KatoCatalog>>(json);
            }
            return katoCatalogs;
        }

        private async Task<List<KatoEnterprise>> GetKatoEnterprises()
        {
            var katoEnterprises = new List<KatoEnterprise>();
            var katoEnterprisesBytes = await _cache.GetAsync(_cacheKeyKatoEnterprises);
            if (katoEnterprisesBytes is null)
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlEnterprises);
                var response = await _smartEcoApi.Client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var enterprisesResponse = await response.Content.ReadAsAsync<EnterprisesResponse>();
                    katoEnterprises = enterprisesResponse.Enterprises
                        .Select(enterprise => enterprise.Kato)
                        .GroupBy(kato => new
                        {
                            kato.Code,
                            kato.Address
                        })
                        .Select(group => group.First())
                        .ToList();
                    SetCacheKatoes(_cacheKeyKatoEnterprises, katoEnterprises);
                }
            }
            else
            {
                var json = Encoding.UTF8.GetString(katoEnterprisesBytes);
                katoEnterprises = JsonConvert.DeserializeObject<List<KatoEnterprise>>(json);
            }
            return katoEnterprises;
        }

        private void SetCacheKatoes<T>(string cacheKey, IList<T> katoes)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(_cacheTime);
            var json = JsonConvert.SerializeObject(katoes);
            var katoCatalogsBytes = Encoding.UTF8.GetBytes(json);
            _cache.Set(cacheKey, katoCatalogsBytes, options);
        }
    }
}

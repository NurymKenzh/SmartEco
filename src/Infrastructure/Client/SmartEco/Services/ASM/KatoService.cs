using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SmartEco.Models.ASM;
using SmartEco.Models.ASM.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Services.ASM
{
    public class KatoService : IKatoService
    {
        private readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(20);
        private readonly string _cacheKeyKatoCatalog = "KatoCatalog";
        private readonly string _cacheKeyKatoEnterprises = "KatoEnterprises";

        private readonly string _urlKatoCatalogs = "api/KatoCatalogs";
        private readonly string _urlEnterprises = "api/Enterprises";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IDistributedCache _cache;

        public KatoService(SmartEcoApi smartEcoApi, IDistributedCache cache)
        {
            _smartEcoApi = smartEcoApi;
            _cache = cache;
        }

        public async Task<List<KatoCatalog>> GetKatoCatalogs()
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

        public async Task<List<KatoEnterprise>> GetKatoEnterprises()
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

    public interface IKatoService
    {
        Task<List<KatoCatalog>> GetKatoCatalogs();
        Task<List<KatoEnterprise>> GetKatoEnterprises();
    }
}

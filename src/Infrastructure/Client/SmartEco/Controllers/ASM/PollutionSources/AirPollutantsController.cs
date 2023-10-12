using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SmartEco.Models.ASM.PollutionSources;
using SmartEco.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Controllers.ASM.PollutionSources
{
    public class AirPollutantsController : Controller
    {
        private readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(20);
        private readonly string _cacheKeyPollutans = "Pollutants";
        private readonly string _urlAirPollutants = "api/AirPollutants";
        private readonly SmartEcoApi _smartEcoApi;
        private readonly IDistributedCache _cache;

        public AirPollutantsController(SmartEcoApi smartEcoApi, IDistributedCache cache)
        {
            _smartEcoApi = smartEcoApi;
            _cache = cache;
        }

        [HttpGet]
        public async Task<List<AirPollutant>> GetPollutants(string pollutantCodeName)
        {
            if (string.IsNullOrEmpty(pollutantCodeName))
                return null;

            var airPollutants = await GetPollutants();
            return airPollutants
                .Where(p => p.CodeFour.StartsWith(pollutantCodeName)
                    || p.Name.StartsWith(pollutantCodeName)
                    || p.CodeName.StartsWith(pollutantCodeName))
                .ToList();
        }

        [HttpGet]
        public async Task<AirPollutant> GetPollutantId(string pollutantCodeName)
        {
            if (string.IsNullOrEmpty(pollutantCodeName))
                return null;

            var airPollutants = await GetPollutants();
            return airPollutants
                .Where(p => p.CodeName.Equals(pollutantCodeName))
                .FirstOrDefault();
        }

        private async Task<List<AirPollutant>> GetPollutants()
        {
            var airPollutants = new List<AirPollutant>();
            var airPollutantsBytes = await _cache.GetAsync(_cacheKeyPollutans);
            if (airPollutantsBytes is null)
            {
                var request = _smartEcoApi.CreateRequest(HttpMethod.Get, _urlAirPollutants);
                var response = await _smartEcoApi.Client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    airPollutants = await response.Content.ReadAsAsync<List<AirPollutant>>();
                    SetCachePollutants(airPollutants);
                }
            }
            else
            {
                var json = Encoding.UTF8.GetString(airPollutantsBytes);
                airPollutants = JsonConvert.DeserializeObject<List<AirPollutant>>(json);
            }
            return airPollutants;
        }

        private void SetCachePollutants(List<AirPollutant> airPollutants)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(_cacheTime);
            var json = JsonConvert.SerializeObject(airPollutants);
            var airPollutantsBytes = Encoding.UTF8.GetBytes(json);
            _cache.Set(_cacheKeyPollutans, airPollutantsBytes, options);
        }
    }
}

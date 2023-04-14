using Clarity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clarity.Services
{
    public class ClarityApi
    {
        private readonly Uri _clarityUri = new Uri("https://clarity-data-api.clarity.io");
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            { "x-api-key", "X2KiJnasqMzQdsy9xsKdceARyADtVVidMqgCQnnH" }
        };

        public async Task<List<ClarityMeasurement>> GetMeasurements(string timeFrequency, DateTime time)
        {
            using (var client = CreateHttpClient())
            {
                List<ClarityMeasurement> clarityMeasurements = new List<ClarityMeasurement>();
                var response = await client.GetAsync($"/v1/measurements?&outputFrequency={timeFrequency}&startTime={time:s}Z&endTime={time:s}Z");

                if (response.IsSuccessStatusCode)
                {
                    var contentResponse = await response.Content.ReadAsStringAsync();
                    clarityMeasurements = JsonConvert.DeserializeObject<List<ClarityMeasurement>>(contentResponse);
                }
                return clarityMeasurements;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = _clarityUri;
            foreach (var header in _headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            return client;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SmartEco.Web.Services.Providers
{
    public class BaseHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;
        public BaseHttpClient(HttpClient httpClient, IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
        {
            _httpClient = httpClient;
            _jsonSettings = jsonOptions.Value.SerializerSettings;
        }

        protected async Task<T> SendRequest<T>(object body, HttpMethod httpMethod, string methodUrl, Dictionary<string, string> headers = null)
        {
            using var request = CreateRequest(body, httpMethod, methodUrl, headers);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var contentResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(contentResponse);

                if (response.StatusCode is HttpStatusCode.Unauthorized)
                {
                    if (TryParseJson(contentResponse, out T result))
                        return result;
                }

                return default;
            }
            catch
            {
                return default;
            }
        }

        protected async Task<HttpResponseMessage> Execute(object body, HttpMethod httpMethod, string methodUrl, Dictionary<string, string> headers = null)
        {
            using var request = CreateRequest(body, httpMethod, methodUrl, headers);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private HttpRequestMessage CreateRequest(object body, HttpMethod httpMethod, string methodUrl, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(httpMethod, methodUrl);
            if (headers is not null)
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Key, header.Value);
            }

            request.Content = new StringContent(JsonConvert.SerializeObject(body, _jsonSettings), Encoding.UTF8, "application/json");
            return request;
        }

        private static bool TryParseJson<T>(string json, out T jObject)
        {
            try
            {
                jObject = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                jObject = default;
                return false;
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System;

namespace SmartEco.Services
{
    public class SmartEcoApi
    {
        public HttpClient Client { get; private set; }

        public SmartEcoApi(HttpClient client)
        {
            Client = client;
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpRequestMessage CreateRequest(HttpMethod httpMethod, string uriRelative, string body = null)
        {
            var uri = new Uri(Client.BaseAddress, new Uri(uriRelative, UriKind.Relative));
            var httpRequestMessage = new HttpRequestMessage(httpMethod, uri);
            if (body != null)
                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return httpRequestMessage;
        }
    }

    public sealed class AuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public AuthenticationHttpClientHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = _accessor.HttpContext.Session.GetString("Token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

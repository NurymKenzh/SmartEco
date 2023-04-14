using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartEco.Akimato.Controllers
{
    public class HttpApiClientController : HttpClient
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public HttpApiClientController(IHttpContextAccessor HttpContextAccessor)
        {
            bool server = Convert.ToBoolean(Startup.Configuration["Server"]);
            string APIUrl = server ? Startup.Configuration["APIUrlServer"] : Startup.Configuration["APIUrlDebug"];
            BaseAddress = new Uri(APIUrl);
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _HttpContextAccessor = HttpContextAccessor;
            string token = _HttpContextAccessor.HttpContext.Session.GetString("Token");
            DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }
    }
}
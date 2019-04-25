using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SmartEco.Controllers
{
    public class HttpApiClientController : HttpClient
    {
        public HttpApiClientController()
        {
            bool server = Convert.ToBoolean(Startup.Configuration["Server"]);
            string APIUrl = server ? Startup.Configuration["APIUrlServer"] : Startup.Configuration["APIUrlDebug"];
            BaseAddress = new Uri(APIUrl);
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
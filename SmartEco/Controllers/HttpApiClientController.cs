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
            BaseAddress = new Uri(Startup.Configuration["APIUrl"]);
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
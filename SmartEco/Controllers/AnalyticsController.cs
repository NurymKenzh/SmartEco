using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public AnalyticsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EcoserviceMonitoringPosts()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "KaragandaRegion"))
            {
                return Redirect("/");
            }

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString())
                .OrderBy(m => m.Name)
                .ToList();
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            string urlMeasuredParameters = "api/MeasuredParameters";
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters);
            measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            measuredParameters = measuredParameters
                .Where(m => !string.IsNullOrEmpty(m.OceanusCode))
                .OrderBy(m => m.Name)
                .ToList();
            ViewBag.MeasuredParameters = measuredParameters.ToArray();

            int MPCExceedPastMinutes = Convert.ToInt32(Startup.Configuration["MPCExceedPastMinutes"]),
                InactivePastMinutes = Convert.ToInt32(Startup.Configuration["InactivePastMinutes"]);
            ViewBag.PastMinutes = Math.Max(MPCExceedPastMinutes, InactivePastMinutes);

            return View();
        }
    }
}
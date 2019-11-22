using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> ComparisonMeasuredParameters()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "KaragandaRegion"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            
            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString())
                .OrderBy(m => m.Name)
                .ToList();
            ViewBag.EcoserviceAirMonitoringPosts = new SelectList(ecoserviceAirMonitoringPosts.OrderBy(m => m.Name), "Id", "AdditionalInformation"); ;
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");

            return View();
        }

        public async Task<IActionResult> GetMeasuredParameters(int MonitoringPostId)
        {
            List<MonitoringPostMeasuredParameters> monitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameters>();
            string urlMonitoringPostMeasuredParameters = "api/MonitoringPosts/getMonitoringPostMeasuredParametersForMap";
            string routeMonitoringPostMeasuredParameters = "";
            routeMonitoringPostMeasuredParameters += string.IsNullOrEmpty(routeMonitoringPostMeasuredParameters) ? "?" : "&";
            routeMonitoringPostMeasuredParameters += $"MonitoringPostId={MonitoringPostId.ToString()}";
            HttpResponseMessage responseMPMP = await _HttpApiClient.PostAsync(urlMonitoringPostMeasuredParameters + routeMonitoringPostMeasuredParameters, null);
            if (responseMPMP.IsSuccessStatusCode)
            {
                monitoringPostMeasuredParameters = await responseMPMP.Content.ReadAsAsync<List<MonitoringPostMeasuredParameters>>();
            }
            var result = monitoringPostMeasuredParameters.OrderBy(m => m.MeasuredParameter.Name).ToList();

            return Json(
                result
            );
        }

        public async Task<IActionResult> GetMeasuredParametersForUnit(int MeasuredParameterId)
        {
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            var measuredParameter = measuredParameters.Where(m => m.Id == MeasuredParameterId).FirstOrDefault();
            var result = measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MeasuredParameterUnitId == measuredParameter.MeasuredParameterUnitId).OrderBy(m => m.Name);

            return Json(
                result
            );
        }

        public async Task<IActionResult> GetMeasuredDatas(
            int? MonitoringPostId,
            int? MeasuredParameterId,
            DateTime DateTimeFrom,
            DateTime DateTimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            MeasuredData[] measureddatas = null;
            string url = "api/MeasuredDatas",
                route = "";
            // SortOrder=DateTime
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder=DateTime";
            }
            // MonitoringPostId
            if (MonitoringPostId != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringPostId={MonitoringPostId}";
            }
            // MeasuredParameterId
            if (MeasuredParameterId != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MeasuredParameterId={MeasuredParameterId}";
            }
            // AveragedFilter
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Averaged={Averaged}";
            }
            // dateTimeFrom
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={DateTimeFrom.ToString(dateTimeFormatInfo)}";
            }
            // dateTimeTo
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={DateTimeTo.ToString(dateTimeFormatInfo)}";
            }
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
            }
            measureddatas = measuredDatas.OrderByDescending(m => m.DateTime).ToArray();
            return Json(
                measureddatas
            );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
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
            ViewBag.EcoserviceAirMonitoringPosts = new SelectList(ecoserviceAirMonitoringPosts.OrderBy(m => m.Name), "Id", "AdditionalInformation");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            return View();
        }

        public IActionResult PostAnalytics()
        {
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAnalytics(
            DateTime DateFrom,
            DateTime DateTo,
            DateTime TimeFrom,
            DateTime TimeTo)
        {
            string url = "api/Analytics",
                route = "";
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            if (dateTimeFrom != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";
            }
            if (dateTimeTo != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";
            }

            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"Server={Startup.Configuration["Server"]}";
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            //if (response.IsSuccessStatusCode)
            //{
            //}

            ViewBag.ExcelSent = "Excel-файл отправлен на Ваш E-mail";
            ViewBag.DateFrom = DateFrom.ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = DateTo.ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");
            return View();
        }

        public IActionResult PostAnalyticsAlmaty()
        {
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAnalyticsAlmaty(
            DateTime DateFrom,
            DateTime DateTo,
            DateTime TimeFrom,
            DateTime TimeTo,
            List<string> MonitoringPostsId)
        {
            string url = "api/Analytics/ExcelFormationAlmaty",
                route = "";
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
            if (dateTimeFrom != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";
            }
            if (dateTimeTo != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";
            }
            if (MonitoringPostsId.Count != 0)
            {
                foreach (var monitoringPostId in MonitoringPostsId)
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MonitoringPostsId={monitoringPostId}".Replace(',', '.');
                }
            }

            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"Server={Startup.Configuration["Server"]}";
            HttpResponseMessage response = await _HttpApiClient.PostAsync(url + route, null);
            //if (response.IsSuccessStatusCode)
            //{
            //}

            ViewBag.ExcelSent = "Excel-файл отправлен на Ваш E-mail";
            ViewBag.DateFrom = DateFrom.ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = DateTo.ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");
            return View();
        }

        public async Task<IActionResult> GetMonitoringPosts()
        {
            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .OrderBy(m => m.Name)
                .ToList();
            var result = ecoserviceAirMonitoringPosts;

            return Json(
                result
            );
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
            DateTime DateFrom,
            DateTime TimeFrom,
            DateTime DateTo,
            DateTime TimeTo,
            bool? Averaged = true)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            MeasuredData[] measureddatas = null;
            string url = "api/MeasuredDatas",
                route = "";
            DateTime dateTimeFrom = DateFrom.Date + TimeFrom.TimeOfDay,
                dateTimeTo = DateTo.Date + TimeTo.TimeOfDay;
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
                route += $"DateTimeFrom={dateTimeFrom.ToString(dateTimeFormatInfo)}";
            }
            // dateTimeTo
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={dateTimeTo.ToString(dateTimeFormatInfo)}";
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

        public async Task<IActionResult> Stats()
        {
            int[] postsIds = new int[] { 52, 53, 43, 45, 47, 49, 50, 51, 48, 46 };
            string report = $"Период с 2019-06-01 по 2019-11-26\r\n";

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            measuredParameters = measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).ToList();

            foreach (int postId in postsIds)
            {
                report += $"Пост Id\t{postId}\r\n";
                DateTime startDT = new DateTime(2019, 6, 1);
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                string url = "api/MeasuredDatas",
                    route = "";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringPostId={postId}";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Averaged={true}";
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={startDT.ToString(dateTimeFormatInfo)}";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={DateTime.Now.ToString(dateTimeFormatInfo)}";
                HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
                if (response.IsSuccessStatusCode)
                {
                    measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measuredDatas = measuredDatas.OrderBy(m => m.DateTime).ToList();
                report += $"Средние значения:\r\n";
                foreach (MeasuredParameter measuredParameter in measuredParameters)
                {
                    report += $"{measuredParameter.Name}\t";
                    decimal? value = measuredDatas
                        .Where(m => m.MeasuredParameterId == measuredParameter.Id)
                        .Average(m => m.Value);
                    report += $"{value?.ToString().Replace(',','.')}\r\n";
                }
                int count = measuredDatas.Count() * 20;
                report += $"Количество данных:\t{count.ToString()}\r\n";
                //foreach (MeasuredParameter measuredParameter in measuredParameters)
                //{
                //    report += $"{measuredParameter.Name}\t";
                //    int count = measuredDatas
                //        .Count(m => m.MeasuredParameterId == measuredParameter.Id);
                //    report += $"{count.ToString()}\r\n";
                //}
                report += $"Превышения ПДК:\r\n";
                report += $"Пост\tПараметр\tДата, время\tЗначение\tПДК\r\n";
                foreach (MeasuredData measuredData in measuredDatas)
                //for (int i = 0; i <= measuredDatas.Count(); i++)
                {
                    if(measuredData.MeasuredParameter.MPCMaxSingle!=null)
                    {
                        if (measuredData.Value >= measuredData.MeasuredParameter.MPCMaxSingle)
                        {
                            report += $"{measuredData.MonitoringPost.Name} ({measuredData.MonitoringPost.AdditionalInformation})\t";
                            report += $"{measuredData.MeasuredParameter.Name}\t";
                            report += $"{measuredData.DateTime?.ToString()}\t";
                            report += $"{measuredData.Value?.ToString().Replace(',', '.')}\t";
                            report += $"{measuredData.MeasuredParameter.MPCMaxSingle?.ToString().Replace(',', '.')}\r\n";
                        }
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> Data()
        {
            int[] postsIds = new int[] { 52, 53, 43, 45, 47, 49, 50, 51, 48, 46 };
            string fileName = @"E:\Documents\New\Data.txt";
            System.IO.File.AppendAllText(fileName, "Дата\tПост\tПараметр\tЗначение\r\n");
            string data = "";

            foreach (int postId in postsIds)
            {
                DateTime startDT = new DateTime(2019, 6, 1);
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                string url = "api/MeasuredDatas",
                    route = "";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringPostId={postId}";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Averaged={true}";
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={startDT.ToString(dateTimeFormatInfo)}";
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={DateTime.Now.ToString(dateTimeFormatInfo)}";
                HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
                if (response.IsSuccessStatusCode)
                {
                    measuredDatas = await response.Content.ReadAsAsync<List<MeasuredData>>();
                }
                measuredDatas = measuredDatas.OrderBy(m => m.DateTime).ToList();
                foreach(MeasuredData measuredData in measuredDatas)
                {
                    //System.IO.File.AppendAllText(fileName, $"{measuredData.DateTime?.ToString()}\t" +
                    //    $"{measuredData.MonitoringPost.Name} ({measuredData.MonitoringPost.AdditionalInformation})\t" +
                    //    $"{measuredData.MeasuredParameter.Name}\t" +
                    //    $"{measuredData.Value?.ToString().Replace(',', '.')}\r\n");
                    data += $"{measuredData.DateTime?.ToString()}\t" +
                        $"{measuredData.MonitoringPost.Name} ({measuredData.MonitoringPost.AdditionalInformation})\t" +
                        $"{measuredData.MeasuredParameter.Name}\t" +
                        $"{measuredData.Value?.ToString().Replace(',', '.')}\r\n";
                }
            }
            System.IO.File.AppendAllText(fileName, data);

            return View();
        }

        public async Task<IActionResult> AllMonitoringPosts()
        {
            string url = "api/Analytics/GetAllMonitoringPosts";
            List<PostData> postDatas = new List<PostData>();
            HttpResponseMessage responsePostDatass = await _HttpApiClient.GetAsync(url);
            postDatas = await responsePostDatass.Content.ReadAsAsync<List<PostData>>();

            return View(postDatas);
        }
    }
}
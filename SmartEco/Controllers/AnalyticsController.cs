using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SmartEco.Models;
using SmartEco.Models.AMS;

namespace SmartEco.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;
        private readonly IHostingEnvironment _appEnvironment;

        public AnalyticsController(HttpApiClientController HttpApiClient, IHostingEnvironment appEnvironment)
        {
            _HttpApiClient = HttpApiClient;
            _appEnvironment = appEnvironment;
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

        public async Task<IActionResult> GetMonitoringPosts(string projectName)
        {
            var result = await GetAllMonitoringPosts(projectName);

            return Json(
                result
            );
        }

        public async Task<List<MonitoringPost>> GetAllMonitoringPosts(string projectName)
        {
            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == projectName
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .OrderBy(m => m.Name)
                .ToList();
            
            return ecoserviceAirMonitoringPosts;
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

        public IActionResult CalcAirPollutantModel()
        {
            List<SelectListItem> crossroads = new List<SelectListItem> {
                new SelectListItem() { Text="MAIN ST. AND LOCAL ST. INTERSECTION", Value="1"}
            };
            ViewBag.Crossroads = crossroads;
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            return View();
        }

        public class Camera
        {
            public int Id;
            public string Name;
            public decimal CoordX;
            public decimal CoordY;
            public decimal CoordZ;
        }

        public class Link
        {
            public int Id;
            public string Name;
            public int IQ;
            public string Type;
            public decimal CoordX1;
            public decimal CoordY1;
            public decimal CoordX2;
            public decimal CoordY2;
            public decimal HL;
            public decimal WL;
            public decimal? VPHL;
            public decimal? EFL;
            public int? NLANES;
        }

        public class LinkQueue
        {
            public int Id;
            public int LinkId;
            public int CAVG;
            public int RAVG;
            public decimal YFAC;
            public int IV;
            public decimal IDLFAC;
            public int SFR;
            public int ST;
            public int AT;
        }

        [HttpPost]
        public async Task<IActionResult> CalcAirPollutantModel(
            int CrossroadId,
            DateTime DateFrom,
            DateTime DateTo,
            DateTime TimeFrom,
            DateTime TimeTo,
            decimal atim,
            decimal zo,
            decimal vs,
            decimal vd,
            int nr,
            decimal scal,
            int iopt,
            List<string> cameraName,
            List<decimal> cameraCoordX,
            List<decimal> cameraCoordY,
            List<decimal> cameraCoordZ,
            int nl,
            int nm,
            string mode,
            List<string> linkName,
            List<int> linkIQ,
            List<string> linkType,
            List<decimal> linkCoordX1,
            List<decimal> linkCoordY1,
            List<decimal> linkCoordX2,
            List<decimal> linkCoordY2,
            List<decimal> vphl,
            List<decimal> efl,
            List<decimal> hl,
            List<decimal> wl,
            List<int> nlanes,
            List<int> cavg,
            List<int> ravg,
            List<decimal> yfac,
            List<int> iv,
            List<decimal> idfac,
            List<int> sfr,
            List<int> st,
            List<int> at,
            decimal u,
            decimal brg,
            int clas,
            decimal mixh,
            decimal amb,
            string variable,
            int degr,
            int vai1,
            int vai2)
        {
            string fileDat = Path.Combine(_appEnvironment.WebRootPath, "file.dat");
            string fileBat = Path.Combine(_appEnvironment.WebRootPath, "file.bat");
            string fileOut = Path.Combine(_appEnvironment.WebRootPath, "file.out");
            using (var sw = new StreamWriter(fileDat))
            {
                //var atim = 60; //переменная, представляющая среднее время(мин)
                //var zo = 175; //переменная, представляющая шероховатость поверхности(см)
                //var vs = 0; //переменная, представляющая скорость оседания(см/с)
                //var vd = 0; //реальная скорость осаждения(см/с)
                //var nr = 8; //  целое число, представляющее количество рецепторов (камер)
                //var scal = 0.3048; // коэффициент пересчета шкалы (0.3048 - футы, 1.0 - метры)
                //var iopt = 1; // целое число, метрическое преобразование вывода (1 - футы, 0 - метры)
                var idebug = 1; // целое число, опция отладки

                sw.WriteLine($"'MODEL AIR POLLUTANT CONCENTRATIONS' {atim.ToString(CultureInfo.GetCultureInfo("en-US"))} {zo.ToString(CultureInfo.GetCultureInfo("en-US"))} {vs.ToString(CultureInfo.GetCultureInfo("en-US"))} {vd.ToString(CultureInfo.GetCultureInfo("en-US"))} {nr} {scal.ToString(CultureInfo.GetCultureInfo("en-US"))} {iopt} {idebug}");

                //List<Camera> cameras = new List<Camera>
                //{
                //    new Camera{Id = 1, Name = "REC 1 (SE CORNER)", CoordX = 45, CoordY = -35, CoordZ = 6},
                //    new Camera{Id = 2, Name = "REC 2 (SW CORNER)", CoordX = -45, CoordY = -35, CoordZ = 6},
                //    new Camera{Id = 3, Name = "REC 3 (NW CORNER)", CoordX = -45, CoordY = 35, CoordZ = 6},
                //    new Camera{Id = 4, Name = "REC 4 (NE CORNER)", CoordX = 45, CoordY = 35, CoordZ = 6},
                //    new Camera{Id = 5, Name = "REC 5 (E MID-MAIN)", CoordX = 45, CoordY = -150, CoordZ = 6},
                //    new Camera{Id = 6, Name = "REC 6 (W MID-MAIN)", CoordX = -45, CoordY = -150, CoordZ = 6},
                //    new Camera{Id = 7, Name = "REC 7 (N MID-LOCAL)", CoordX = -150, CoordY = 35, CoordZ = 6},
                //    new Camera{Id = 8, Name = "REC 8 (S MID-LOCAL)", CoordX = -150, CoordY = -35, CoordZ = 6}
                //};

                List<Camera> cameras = new List<Camera>();
                for(int i = 0; i < cameraName.Count; i++)
                {
                    cameras.Add(new Camera { Id = i, Name = cameraName[i], CoordX = cameraCoordX[i], CoordY = cameraCoordY[i], CoordZ = cameraCoordY[i] });
                }

                foreach (var camera in cameras)
                {
                    sw.WriteLine($"'{camera.Name}' {camera.CoordX.ToString(CultureInfo.GetCultureInfo("en-US"))} {camera.CoordY.ToString(CultureInfo.GetCultureInfo("en-US"))} {camera.CoordZ.ToString(CultureInfo.GetCultureInfo("en-US"))}");
                }

                var run = "MAIN ST. AND LOCAL ST.INTERSECTION"; //переменная, обозначающая название текущего цикла (перекрёстка)
                //var nl = 9; //целое число, указывающее количество ссылок
                //var nm = 1; //целое число, обозначающее количество метровых условий
                var print2 = 0; //целое число, 0 - короткий формат, 1 - таблицы матрицы рецептор-ссылка
                //var mode = "C"; //символьная переменная («C» для CO или «P» для твердых частиц)

                sw.WriteLine($"'{run}' {nl} {nm} {print2} '{mode}'");

                //List<Link> links = new List<Link>
                //{
                //    new Link{Id = 1, Name = "Main St.NB Appr.", IQ = 1, Type = "AG", CoordX1 = 10, CoordY1 = -1000, CoordX2 = 10, CoordY2 = 0, VPHL = 1500, EFL = 41.6m, HL = 0, WL = 40},
                //    new Link{Id = 2, Name = "Main St.NB Queue", IQ = 2, Type = "AG", CoordX1 = 10, CoordY1 = -10, CoordX2 = 10, CoordY2 = -1000, HL = 0, WL = 20, NLANES = 2 },
                //    new Link{Id = 3, Name = "Main St.NB Dep.", IQ = 1, Type = "AG", CoordX1 = 10, CoordY1 = 0, CoordX2 = 10, CoordY2 = 1000, VPHL = 1500, EFL = 41.6m, HL = 0, WL = 40},
                //    new Link{Id = 4, Name = "Main St.SB Appr.", IQ = 1, Type = "AG", CoordX1 = -10, CoordY1 = 1000, CoordX2 = -10, CoordY2 = 0, VPHL = 1200, EFL = 41.6m, HL = 0, WL = 40},
                //    new Link{Id = 5, Name = "Main St.SB Queue", IQ = 2, Type = "AG", CoordX1 = -10, CoordY1 = 10, CoordX2 = -10, CoordY2 = 1000, HL = 0, WL = 20, NLANES = 2 },
                //    new Link{Id = 6, Name = "Main St.SB Dep.", IQ = 1, Type = "AG", CoordX1 = -10, CoordY1 = 0, CoordX2 = -10, CoordY2 = -1000, VPHL = 1200, EFL = 41.6m, HL = 0, WL = 40},
                //    new Link{Id = 7, Name = "Local St.Appr.Lnk.", IQ = 1, Type = "AG", CoordX1 = -1000, CoordY1 = 0, CoordX2 = 0, CoordY2 = 0, VPHL = 1000, EFL = 41.6m, HL = 0, WL = 40},
                //    new Link{Id = 8, Name = "Local St.Queue Lnk.", IQ = 2, Type = "AG", CoordX1 = -20, CoordY1 = 0, CoordX2 = -1000, CoordY2 = 0, HL = 0, WL = 20, NLANES = 2 },
                //    new Link{Id = 9, Name = "Main St.NB Appr.", IQ = 1, Type = "AG", CoordX1 = 0, CoordY1 = 0, CoordX2 = 1000, CoordY2 = 0, VPHL = 1000, EFL = 41.6m, HL = 0, WL = 40}
                //};

                //List<LinkQueue> linkQueues = new List<LinkQueue>
                //{
                //    new LinkQueue{Id = 1, LinkId = 2, CAVG = 90, RAVG = 40, YFAC = 3.0m, IV = 1500, IDLFAC = 735.00m, SFR = 0, ST = 0, AT = 0 },
                //    new LinkQueue{Id = 2, LinkId = 5, CAVG = 90, RAVG = 40, YFAC = 3.0m, IV = 1200, IDLFAC = 735.00m, SFR = 0, ST = 0, AT = 0 },
                //    new LinkQueue{Id = 3, LinkId = 8, CAVG = 90, RAVG = 50, YFAC = 3.0m, IV = 1000, IDLFAC = 735.00m, SFR = 0, ST = 0, AT = 0 }
                //};

                //foreach (var link in links)
                //{
                //    sw.WriteLine($"{link.IQ}");
                //    if (link.IQ == 1)
                //    {
                //        sw.WriteLine($"'{link.Name}' '{link.Type}' {link.CoordX1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordX2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.VPHL.Value.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.EFL.Value.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.HL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.WL.ToString(CultureInfo.GetCultureInfo("en-US"))}");
                //    }
                //    else
                //    {
                //        sw.WriteLine($"'{link.Name}' '{link.Type}' {link.CoordX1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordX2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.HL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.WL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.NLANES}");
                //        var linkQueue = linkQueues.Where(l => l.LinkId == link.Id).FirstOrDefault();
                //        if (linkQueue != null)
                //        {
                //            sw.WriteLine($"{linkQueue.CAVG} {linkQueue.RAVG} {linkQueue.YFAC.ToString(CultureInfo.GetCultureInfo("en-US"))} {linkQueue.IV} {linkQueue.IDLFAC.ToString(CultureInfo.GetCultureInfo("en-US"))} {linkQueue.SFR} {linkQueue.ST} {linkQueue.AT}");
                //        }
                //    }
                //}

                List<Link> links = new List<Link>
                {
                    new Link{Id = 0, Name = linkName[0], IQ = linkIQ[0], Type = linkType[0], CoordX1 = linkCoordX1[0], CoordY1 = linkCoordY1[0], CoordX2 = linkCoordX2[0], CoordY2 = linkCoordY2[0], VPHL = vphl[0], EFL = efl[0], HL = hl[0], WL = wl[0]},
                    new Link{Id = 1, Name = linkName[1], IQ = linkIQ[1], Type = linkType[1], CoordX1 = linkCoordX1[1], CoordY1 = linkCoordY1[1], CoordX2 = linkCoordX2[1], CoordY2 = linkCoordY2[1], HL = hl[1], WL = wl[1], NLANES = nlanes[0] }
                };

                List<LinkQueue> linkQueues = new List<LinkQueue>
                {
                    new LinkQueue{Id = 0, LinkId = 1, CAVG = cavg[0], RAVG = ravg[0], YFAC = yfac[0], IV = iv[0], IDLFAC = idfac[0], SFR = sfr[0], ST = st[0], AT = at[0] }
                };

                foreach (var link in links)
                {
                    sw.WriteLine($"{link.IQ}");
                    if (link.IQ == 1)
                    {
                        sw.WriteLine($"'{link.Name}' '{link.Type}' {link.CoordX1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordX2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.VPHL.Value.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.EFL.Value.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.HL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.WL.ToString(CultureInfo.GetCultureInfo("en-US"))}");
                    }
                    else
                    {
                        sw.WriteLine($"'{link.Name}' '{link.Type}' {link.CoordX1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY1.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordX2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.CoordY2.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.HL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.WL.ToString(CultureInfo.GetCultureInfo("en-US"))} {link.NLANES}");
                        var linkQueue = linkQueues.Where(l => l.LinkId == link.Id).FirstOrDefault();
                        if (linkQueue != null)
                        {
                            sw.WriteLine($"{linkQueue.CAVG} {linkQueue.RAVG} {linkQueue.YFAC.ToString(CultureInfo.GetCultureInfo("en-US"))} {linkQueue.IV} {linkQueue.IDLFAC.ToString(CultureInfo.GetCultureInfo("en-US"))} {linkQueue.SFR} {linkQueue.ST} {linkQueue.AT}");
                        }
                    }
                }

                //var u = 1.0; //действительная переменная, указывающая скорость ветра(м/с)
                //var brg = 0; // действительная переменная, указывающая направление ветра (угол)
                //var clas = 4; //целое число, обозначающее класс стабильности
                //var mixh = 1000; //действительная переменная, представляющая высоту смешивания(м)
                //var amb = 0; // действительное значение, указывающее фоновую концентрацию окружающей среды(ppm)
                //var variable = "Y"; // «Y» - изменение направления ветра, «N» - только одно направление ветра
                //var degr = 10; // целое число, метрическое преобразование вывода (1 - футы, 0 - метры)
                //var vai1 = 0; // целое число, обозначающее нижнюю границу диапазона вариации
                //var vai2 = 36; // целое число, которое указывает верхнюю границу диапазона изменения

                sw.WriteLine($"{u.ToString(CultureInfo.GetCultureInfo("en-US"))} {brg.ToString(CultureInfo.GetCultureInfo("en-US"))} {clas} {mixh.ToString(CultureInfo.GetCultureInfo("en-US"))} {amb.ToString(CultureInfo.GetCultureInfo("en-US"))} '{variable}' {degr} {vai1} {vai2}");
            }

            using (var sw = new StreamWriter(fileBat))
            {
                sw.WriteLine($"CAL3QHC file.dat file.out");
            }

            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;


                process.StartInfo.WorkingDirectory = _appEnvironment.WebRootPath;
                process.StartInfo.FileName = fileBat;
                process.Start();

                string output = "",
                    error = "";
                while (!process.StandardOutput.EndOfStream)
                {
                    output += process.StandardOutput.ReadLine();
                }
                while (!process.StandardError.EndOfStream)
                {
                    error += process.StandardError.ReadLine();
                }

                process.WaitForExit();
                System.IO.File.Delete(fileBat);
                System.IO.File.Delete(fileDat);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }

            List<string> matrix = new List<string>();

            using (StreamReader sr = new StreamReader(fileOut, System.Text.Encoding.Default))
            {
                string line;
                while (!sr.ReadLine().Contains("(PPM)"))
                {
                }
                do
                {
                    line = sr.ReadLine();
                    matrix.Add(line);
                }
                while (line != null);
                ViewBag.Matrix = matrix;
            }
            System.IO.File.Delete(fileOut);

            List<SelectListItem> crossroads = new List<SelectListItem> {
                new SelectListItem() { Text="MAIN ST. AND LOCAL ST. INTERSECTION", Value="1"}
            };
            ViewBag.Crossroads = crossroads;
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            return View();
        }

        public async Task<IActionResult> ComparisonTargetValue()
        {
            List<PollutionEnvironment> pollutionEnvironments = new List<PollutionEnvironment>();
            string urlPollutionEnvironments = "api/PollutionEnvironments",
                routePollutionEnvironments = "";
            HttpResponseMessage responsePollutionEnvironments = await _HttpApiClient.GetAsync(urlPollutionEnvironments + routePollutionEnvironments);
            if (responsePollutionEnvironments.IsSuccessStatusCode)
            {
                pollutionEnvironments = await responsePollutionEnvironments.Content.ReadAsAsync<List<PollutionEnvironment>>();
            }
            ViewBag.PollutionEnvironments = new SelectList(pollutionEnvironments.OrderBy(m => m.Name), "Id", "Name");

            List<Target> targets = new List<Target>();
            string urlTargets = "api/Targets",
                routeTargets = "";
            HttpResponseMessage responseTargets = await _HttpApiClient.GetAsync(urlTargets + routeTargets);
            if (responseTargets.IsSuccessStatusCode)
            {
                targets = await responseTargets.Content.ReadAsAsync<List<Target>>();
            }
            ViewBag.Targets = new SelectList(targets.OrderBy(m => m.Name), "Id", "Name");

            List<TargetTerritory> targetTerritories = new List<TargetTerritory>();
            string urlTargetTerritories = "api/TargetTerritories",
                routeTargetTerritories = "";
            HttpResponseMessage responseTargetTerritories = await _HttpApiClient.GetAsync(urlTargetTerritories + routeTargetTerritories);
            if (responseTargetTerritories.IsSuccessStatusCode)
            {
                targetTerritories = await responseTargetTerritories.Content.ReadAsAsync<List<TargetTerritory>>();
            }
            ViewBag.TargetTerritories = new SelectList(targetTerritories.OrderBy(m => m.Name), "Id", "Name");

            List<Event> events = new List<Event>();
            string urlEvents = "api/Events",
                routeEvents = "";
            HttpResponseMessage responseEvents = await _HttpApiClient.GetAsync(urlEvents + routeEvents);
            if (responseEvents.IsSuccessStatusCode)
            {
                events = await responseEvents.Content.ReadAsAsync<List<Event>>();
            }
            ViewBag.Events = new SelectList(events.OrderBy(m => m.Name), "Id", "Name");

            List<Project> projects = new List<Project>();
            string urlProjects = "api/Projects",
                routeProjects = "";
            HttpResponseMessage responseProjects = await _HttpApiClient.GetAsync(urlProjects + routeProjects);
            if (responseProjects.IsSuccessStatusCode)
            {
                projects = await responseProjects.Content.ReadAsAsync<List<Project>>();
            }
            ViewBag.Projects = new SelectList(projects.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        public async Task<IActionResult> GetAActivity(
            int PollutionEnvironmentId,
            int TargetId,
            int TargetTerritoryId,
            int? EventId,
            int? ProjectId)
        {
            List<AActivity> aActivities = new List<AActivity>();
            string urlAActivities = "api/AActivities",
                routeAActivities = "";
            HttpResponseMessage responseAActivities = await _HttpApiClient.GetAsync(urlAActivities + routeAActivities);
            if (responseAActivities.IsSuccessStatusCode)
            {
                aActivities = await responseAActivities.Content.ReadAsAsync<List<AActivity>>();
            }
            if(ProjectId != null)
            {
                aActivities = aActivities.Where(a => a.Target.PollutionEnvironmentId == PollutionEnvironmentId
                && a.TargetId == TargetId && a.TargetTerritoryId == TargetTerritoryId && (a.EventId == EventId || EventId == null) && a.ProjectId == ProjectId).OrderBy(a => a.Year).ToList();
            }
            else
            {
                aActivities = aActivities.Where(a => a.Target.PollutionEnvironmentId == PollutionEnvironmentId
                && a.TargetId == TargetId && a.TargetTerritoryId == TargetTerritoryId && (a.EventId == EventId || EventId == null)).OrderBy(a => a.Year).ToList();
            }
            var result = aActivities.OrderBy(m => m.Name);

            return Json(
                result
            );
        }

        public async Task<IActionResult> ZhanatasEcoserviceMonitoringPosts()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Zhanatas"))
            {
                return Redirect("/");
            }

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            List<MonitoringPost> zhanatasEcoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString() && m.Project.Name == "Zhanatas")
                .OrderBy(m => m.Name)
                .ToList();
            ViewBag.ZhanatasEcoserviceAirMonitoringPosts = zhanatasEcoserviceAirMonitoringPosts.ToArray();

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

        public async Task<IActionResult> PostAnalyticsZhanatas()
        {
            ViewBag.DateFrom = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = DateTime.Today.ToString("HH:mm:ss");
            ViewBag.DateTo = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");
            var monitoringPosts = await GetAllMonitoringPosts("Zhanatas");
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters/GetSameMeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = new SelectList(measuredParameters.OrderBy(m => m.Name), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAnalyticsZhanatas(
            DateTime DateFrom,
            DateTime DateTo,
            DateTime TimeFrom,
            DateTime TimeTo,
            List<int> MonitoringPostsId,
            List<int> MeasuredParametersId)
        {
            string url = "api/Analytics/ExcelFormationZhanatas",
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
            if (MeasuredParametersId.Count != 0)
            {
                foreach (var measuredParametersId in MeasuredParametersId)
                {
                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"MeasuredParametersId={measuredParametersId}".Replace(',', '.');
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
            ViewBag.TimeFrom = DateTime.Today.ToString("HH:mm:ss");
            ViewBag.DateTo = DateTo.ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");
            var monitoringPosts = await GetAllMonitoringPosts("Zhanatas");
            ViewBag.MonitoringPosts = new SelectList(monitoringPosts.OrderBy(m => m.Name), "Id", "Name");
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters/GetSameMeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            ViewBag.MeasuredParameters = new SelectList(measuredParameters.OrderBy(m => m.Name), "Id", "Name");
            return View();
        }

        //public async Task<IActionResult> AnalyticAMS()
        //{
        //    string url = "api/Analytics/GetAnalyticAMS";
        //    List<MonitoringData> monitoringDatas = new List<MonitoringData>();
        //    HttpResponseMessage responseMonitoringDatas = await _HttpApiClient.GetAsync(url);
        //    monitoringDatas = await responseMonitoringDatas.Content.ReadAsAsync<List<MonitoringData>>();

        //    return View(monitoringDatas);
        //}

        //[HttpPost]
        public async Task<IActionResult> AnalyticAMS(string SortOrder,
            string EnterpriseNameFilter,
            string CityFilter,
            string SourceAirPollutionNameFilter,
            string SourceEmissionNameFilter,
            string MonitoringParameterNameFilter,
            decimal? ValueFilter,
            decimal? MPCMaxSingleFilter,
            DateTime? DateTimeFromFilter,
            DateTime? DateTimeToFilter,
            int? PageSize,
            int? PageNumber)
        {
            MonitoringDataViewModel monitoringDataViewModel = new MonitoringDataViewModel();
            List<MonitoringData> monitoringDatas = new List<MonitoringData>();
            int monitoringDataCount = 0;

            ViewBag.EnterpriseNameFilter = EnterpriseNameFilter;
            ViewBag.CityFilter = CityFilter;
            ViewBag.SourceAirPollutionNameFilter = SourceAirPollutionNameFilter;
            ViewBag.SourceEmissionNameFilter = SourceEmissionNameFilter;
            ViewBag.MonitoringParameterNameFilter = MonitoringParameterNameFilter;
            ViewBag.ValueFilter = ValueFilter;
            ViewBag.MPCMaxSingleFilter = MPCMaxSingleFilter;
            ViewBag.DateTimeFromFilter = (DateTimeFromFilter)?.ToString("yyyy-MM-dd"); ;
            ViewBag.DateTimeToFilter = (DateTimeToFilter)?.ToString("yyyy-MM-dd");

            ViewBag.EnterpriseNameSort = SortOrder == "EnterpriseName" ? "EnterpriseNameDesc" : "EnterpriseName";
            ViewBag.CitySort = SortOrder == "City" ? "CityDesc" : "City";
            ViewBag.SourceAirPollutionNameSort = SortOrder == "SourceAirPollutionName" ? "SourceAirPollutionNameDesc" : "SourceAirPollutionName";
            ViewBag.SourceEmissionNameSort = SortOrder == "SourceEmissionName" ? "SourceEmissionNameDesc" : "SourceEmissionName";
            ViewBag.MonitoringParameterNameSort = SortOrder == "MonitoringParameterName" ? "MonitoringParameterNameDesc" : "MonitoringParameterName";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";
            ViewBag.MPCMaxSingleSort = SortOrder == "MPCMaxSingle" ? "MPCMaxSingleDesc" : "MPCMaxSingle";
            ViewBag.DateTimeSort = SortOrder == "DateTime" ? "DateTimeDesc" : "DateTime";

            string url = "api/Analytics/GetAnalyticAMS",
                route = "";
            if (!string.IsNullOrEmpty(SortOrder))
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SortOrder={SortOrder}";
            }
            if (EnterpriseNameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"EnterpriseName={EnterpriseNameFilter}";
            }
            if (CityFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"City={CityFilter}";
            }
            if (SourceAirPollutionNameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SourceAirPollutionName={SourceAirPollutionNameFilter}";
            }
            if (SourceEmissionNameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"SourceEmissionName={SourceEmissionNameFilter}";
            }
            if (MonitoringParameterNameFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MonitoringParameterName={MonitoringParameterNameFilter}";
            }
            if (ValueFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"Value={ValueFilter}".Replace(',', '.');
            }
            if (MPCMaxSingleFilter != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"MPCMaxSingle={MPCMaxSingleFilter}";
            }
            if (DateTimeFromFilter != null)
            {
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeFrom={DateTimeFromFilter?.ToString(dateTimeFormatInfo)}";
            }
            if (DateTimeToFilter != null)
            {
                DateTimeToFilter = DateTimeToFilter?.AddDays(1).AddMilliseconds(-1);
                DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"DateTimeTo={DateTimeToFilter?.ToString(dateTimeFormatInfo)}";
            }
            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);
            ViewBag.PageSizeList = new SelectList(pageSizeList.OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), "Key", "Value");
            if (PageSize == null)
            {
                PageSize = Convert.ToInt32(pageSizeList.Min(p => p.Value));
            }
            if (PageSize == 0)
            {
                PageSize = null;
            }
            if (PageSize != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageSize={PageSize.ToString()}";
                if (PageNumber == null)
                {
                    PageNumber = 1;
                }
            }
            if (PageNumber != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageNumber={PageNumber.ToString()}";
            }
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                monitoringDataViewModel = await response.Content.ReadAsAsync<MonitoringDataViewModel>();
                monitoringDatas = monitoringDataViewModel.MonitoringDatas;
                monitoringDataCount = monitoringDataViewModel.MonitoringDatasCount;
            }
            ViewBag.SortOrder = SortOrder;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(monitoringDataCount / (decimal)PageSize) : 1;
            ViewBag.StartPage = PageNumber - 5;
            ViewBag.EndPage = PageNumber + 4;
            if (ViewBag.StartPage <= 0)
            {
                ViewBag.EndPage -= (ViewBag.StartPage - 1);
                ViewBag.StartPage = 1;
            }
            if (ViewBag.EndPage > ViewBag.TotalPages)
            {
                ViewBag.EndPage = ViewBag.TotalPages;
                if (ViewBag.EndPage > 10)
                {
                    ViewBag.StartPage = ViewBag.EndPage - 9;
                }
            }

            return View(monitoringDatas);
        }
    }
}
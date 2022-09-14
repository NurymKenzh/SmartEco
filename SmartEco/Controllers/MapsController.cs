using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartEco.Models;
using SmartEco.Models.UPRZA;

namespace SmartEco.Controllers
{
    public class MapsController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly HttpApiClientController _HttpApiClient;

        public MapsController(IHostingEnvironment appEnvironment, HttpApiClientController HttpApiClient)
        {
            _appEnvironment = appEnvironment;
            _HttpApiClient = HttpApiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Aktau()
        {
            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            string urlLayers = "api/Layers";
            List<Layer> layers = new List<Layer>();
            HttpResponseMessage responseLayers = await _HttpApiClient.GetAsync(urlLayers);
            layers = await responseLayers.Content.ReadAsAsync<List<Layer>>();
            ViewBag.Layers = layers;

            string urlEcomonMonitoringPoints = "api/EcomonMonitoringPoints";
            List<EcomonMonitoringPoint> ecomons = new List<EcomonMonitoringPoint>();
            HttpResponseMessage responseEcomonMonitoringPoints = await _HttpApiClient.GetAsync(urlEcomonMonitoringPoints);
            ecomons = await responseEcomonMonitoringPoints.Content.ReadAsAsync<List<EcomonMonitoringPoint>>();
            JObject ecomonsObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from ecomon in ecomons
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = ecomon.Id,
                                   Number = ecomon.Number
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                            {
                            Convert.ToDecimal(ecomon.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                            Convert.ToDecimal(ecomon.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                            },
                               }
                           }
            });
            ViewBag.EcomonsLayerJson = ecomonsObject.ToString();

            string urlKazHydrometAirPosts = "api/KazHydrometAirPosts";
            List<KazHydrometAirPost> kazHydrometAirPosts = new List<KazHydrometAirPost>();
            HttpResponseMessage responseKazHydrometAirPosts = await _HttpApiClient.GetAsync(urlKazHydrometAirPosts);
            kazHydrometAirPosts = await responseKazHydrometAirPosts.Content.ReadAsAsync<List<KazHydrometAirPost>>();
            JObject objectKazHydrometAirPosts = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from kazHydrometAirPost in kazHydrometAirPosts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = kazHydrometAirPost.Id,
                                   Number = kazHydrometAirPost.Number
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                            {
                            Convert.ToDecimal(kazHydrometAirPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                            Convert.ToDecimal(kazHydrometAirPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                            },
                               }
                           }
            });
            ViewBag.KazHydrometAirPostsLayerJson = objectKazHydrometAirPosts.ToString();

            string urlKazHydrometSoilPosts = "api/KazHydrometSoilPosts";
            List<KazHydrometSoilPost> kazHydrometSoilPosts = new List<KazHydrometSoilPost>();
            HttpResponseMessage responseKazHydrometSoilPosts = await _HttpApiClient.GetAsync(urlKazHydrometSoilPosts);
            kazHydrometSoilPosts = await responseKazHydrometSoilPosts.Content.ReadAsAsync<List<KazHydrometSoilPost>>();
            JObject objectKazHydrometSoilPosts = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from kazHydrometSoilPost in kazHydrometSoilPosts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = kazHydrometSoilPost.Id,
                                   Number = kazHydrometSoilPost.Number
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                            {
                            Convert.ToDecimal(kazHydrometSoilPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                            Convert.ToDecimal(kazHydrometSoilPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                            },
                               }
                           }
            });
            ViewBag.KazHydrometSoilPostsLayerJson = objectKazHydrometSoilPosts.ToString();

            JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            string urlMeasuredDatas = "api/MeasuredDatas",
                 route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={3}";
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double temperature = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 4).Value);
            double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 5).Value);
            double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 6).Value);
            ViewBag.MeasuredData = measuredDatas;
            ViewBag.Temperature = temperature;
            ViewBag.SpeedWind = speedWind == 0 ? 1.0 : speedWind; //Если скорость ветра "0", то рассеивания нет (изолинии будут отсутствовать)
            ViewBag.DirectionWind = directionWind;

            List<SelectListItem> pollutants = new List<SelectListItem>();
            pollutants.Add(new SelectListItem() { Text = "Азот (II) оксид (Азота оксид) (6)", Value = "12" });
            pollutants.Add(new SelectListItem() { Text = "Азота (IV) диоксид (Азота диоксид) (4)", Value = "13" });
            pollutants.Add(new SelectListItem() { Text = "Сера диоксид (Ангидрид сернистый, Сернистый газ, Сера (IV) оксид) (516)", Value = "16" });
            pollutants.Add(new SelectListItem() { Text = "Углерод оксид (Окись углерода, Угарный газ) (584)", Value = "17" });
            ViewBag.Pollutants = pollutants;

            return View();
        }
        public async Task<IActionResult> KaragandaRegion()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "KaragandaRegion" || role == "Kazhydromet"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "KaragandaRegion"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
            
            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();
            
            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "KaragandaRegion"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
            
            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            Task<string> jsonString = null;
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { Id = 0, AQI = .0m })
                .ToList();
            string urlLedScreens = "api/LEDScreens/GetAQIPosts",
                routeLEDScreens = "";
            routeLEDScreens += string.IsNullOrEmpty(routeLEDScreens) ? "?" : "&";
            routeLEDScreens += $"ProjectName=KaragandaRegion";
            HttpResponseMessage responseLedScreens = await _HttpApiClient.GetAsync(urlLedScreens + routeLEDScreens);
            if (responseLedScreens.IsSuccessStatusCode)
            {
                jsonString = responseLedScreens.Content.ReadAsStringAsync();
            }
            var resultString = jsonString.Result.ToString();
            dynamic json = JArray.Parse(resultString);
            foreach (dynamic data in json)
            {
                int id = data.id;
                decimal aqi = data.aqi;
                jsonResult.Add(new { Id = id, AQI = aqi });
            }
            ViewBag.LEDScreensId = jsonResult.Select(r => r.Id).ToArray();
            ViewBag.LEDScreensAQI = jsonResult.Select(r => r.AQI).ToArray();

            return View();
        }

        public async Task<IActionResult> Arys()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Arys"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 59).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 42.32M && m.NorthLatitude <= 42.50M*/
                    m.Project != null && m.Project.Name == "Arys"
                    && m.EastLongitude >= 68.6M && m.EastLongitude <= 69.0M
                    && m.TurnOnOff == true)
                .Where(m => m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString())
                .ToList();

            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();

            return View();
        }

        public async Task<IActionResult> Almaty()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Almaty" || role == "Kazhydromet"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> kazHydrometWaterMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 3
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometWaterMonitoringPostsObject = GetObjectKazHydrometWaterMonitoringPosts(decimaldelimiter, kazHydrometWaterMonitoringPosts);
            ViewBag.KazHydrometWaterMonitoringPostsLayerJson = kazHydrometWaterMonitoringPostsObject.ToString();

            List<MonitoringPost> kazHydrometTransportMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 7
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometTransportMonitoringPostsObject = GetObjectKazHydrometTransportMonitoringPosts(decimaldelimiter, kazHydrometTransportMonitoringPosts);
            ViewBag.KazHydrometTransportMonitoringPostsLayerJson = kazHydrometTransportMonitoringPostsObject.ToString();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
            
            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            List<LEDScreen> ledScreens = await GetLEDScreens();

            JObject objectLEDScreens = GetObjectLEDScreens(decimaldelimiter, ledScreens);
            ViewBag.LEDScreensLayerJson = objectLEDScreens.ToString();

            List<Ecopost> ecoposts = await GetEcoposts();

            JObject objectEcoposts = GetObjectEcoposts(decimaldelimiter, ecoposts);
            ViewBag.EcopostsLayerJson = objectEcoposts.ToString();

            List<ReceptionRecyclingPoint> receptionRecyclingPoints = await GetReceptionRecyclingPoints();

            JObject objectReceptionRecyclingPoints = GetObjectReceptionRecyclingPoints(decimaldelimiter, receptionRecyclingPoints);
            ViewBag.ReceptionRecyclingPointsLayerJson = objectReceptionRecyclingPoints.ToString();

            //Data for calculate dissipation
            string urlMeasuredDatas = "api/MeasuredDatas",
                 route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //List<MeasuredData> measuredDatas = new List<MeasuredData>();
            //HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double temperature = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 4).Value);
            //double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 5).Value);
            //double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 6).Value);
            //ViewBag.MeasuredData = measuredDatas;
            //ViewBag.Temperature = temperature;
            //ViewBag.SpeedWind = speedWind;
            //ViewBag.DirectionWind = directionWind;

            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={4}";
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double temperature = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={5}";
            measuredDatas = new List<MeasuredData>();
            responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={6}";
            measuredDatas = new List<MeasuredData>();
            responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);
            ViewBag.MeasuredData = measuredDatas;
            ViewBag.Temperature = temperature;
            ViewBag.SpeedWind = speedWind == 0 ? 1.0 : speedWind; //Если скорость ветра "0", то рассеивания нет (изолинии будут отсутствовать)
            ViewBag.DirectionWind = directionWind;

            List<SelectListItem> pollutants = new List<SelectListItem>();
            pollutants.Add(new SelectListItem() { Text = "Азот (II) оксид (Азота оксид) (6)", Value = "12" });
            pollutants.Add(new SelectListItem() { Text = "Азота (IV) диоксид (Азота диоксид) (4)", Value = "13" });
            pollutants.Add(new SelectListItem() { Text = "Сера диоксид (Ангидрид сернистый, Сернистый газ, Сера (IV) оксид) (516)", Value = "16" });
            pollutants.Add(new SelectListItem() { Text = "Углерод оксид (Окись углерода, Угарный газ) (584)", Value = "17" });
            pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM2,5", Value = "3" });
            pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM10", Value = "2" });
            ViewBag.PollutantsDessipation = pollutants;

            Task<string> jsonString = null;
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { Id = 0, AQI = .0m })
                .ToList();
            string urlLedScreens = "api/LEDScreens/GetAQIPosts",
                routeLEDScreens = "";
            routeLEDScreens += string.IsNullOrEmpty(routeLEDScreens) ? "?" : "&";
            routeLEDScreens += $"ProjectName=Almaty";
            HttpResponseMessage responseLedScreens = await _HttpApiClient.GetAsync(urlLedScreens + routeLEDScreens);
            if (responseLedScreens.IsSuccessStatusCode)
            {
                jsonString = responseLedScreens.Content.ReadAsStringAsync();
            }
            var resultString = jsonString.Result.ToString();
            dynamic json = JArray.Parse(resultString);
            foreach (dynamic data in json)
            {
                int id = data.id;
                decimal aqi = data.aqi;
                jsonResult.Add(new { Id = id, AQI = aqi });
            }
            ViewBag.LEDScreensId = jsonResult.Select(r => r.Id).ToArray();
            ViewBag.LEDScreensAQI = jsonResult.Select(r => r.AQI).ToArray();

            return View();
        }

        public async Task<IActionResult> AlmatyFree()
        {
            string token = HttpContext.Session.GetString("Token");
            if (!String.IsNullOrEmpty(token))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();
            
            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();

            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            List<LEDScreen> ledScreens = await GetLEDScreens();

            JObject objectLEDScreens = GetObjectLEDScreens(decimaldelimiter, ledScreens);
            ViewBag.LEDScreensLayerJson = objectLEDScreens.ToString();

            List<Ecopost> ecoposts = await GetEcoposts();

            JObject objectEcoposts = GetObjectEcoposts(decimaldelimiter, ecoposts);
            ViewBag.EcopostsLayerJson = objectEcoposts.ToString();

            List<ReceptionRecyclingPoint> receptionRecyclingPoints = await GetReceptionRecyclingPoints();

            JObject objectReceptionRecyclingPoints = GetObjectReceptionRecyclingPoints(decimaldelimiter, receptionRecyclingPoints);
            ViewBag.ReceptionRecyclingPointsLayerJson = objectReceptionRecyclingPoints.ToString();

            Task<string> jsonString = null;
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { Id = 0, AQI = .0m })
                .ToList();
            string urlLedScreens = "api/LEDScreens/GetAQIPosts",
                routeLEDScreens = "";
            routeLEDScreens += string.IsNullOrEmpty(routeLEDScreens) ? "?" : "&";
            routeLEDScreens += $"ProjectName=Shymkent";
            HttpResponseMessage responseLedScreens = await _HttpApiClient.GetAsync(urlLedScreens + routeLEDScreens);
            if (responseLedScreens.IsSuccessStatusCode)
            {
                jsonString = responseLedScreens.Content.ReadAsStringAsync();
            }
            var resultString = jsonString.Result.ToString();
            dynamic json = JArray.Parse(resultString);
            foreach (dynamic data in json)
            {
                int id = data.id;
                decimal aqi = data.aqi;
                jsonResult.Add(new { Id = id, AQI = aqi });
            }
            ViewBag.LEDScreensId = jsonResult.Select(r => r.Id).ToArray();
            ViewBag.LEDScreensAQI = jsonResult.Select(r => r.AQI).ToArray();

            return View();
        }

        public async Task<IActionResult> Shymkent()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Shymkent" || role == "Kazhydromet"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> kazHydrometWaterMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 3
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometWaterMonitoringPostsObject = GetObjectKazHydrometWaterMonitoringPosts(decimaldelimiter, kazHydrometWaterMonitoringPosts);
            ViewBag.KazHydrometWaterMonitoringPostsLayerJson = kazHydrometWaterMonitoringPostsObject.ToString();

            List<MonitoringPost> kazHydrometTransportMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 7
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometTransportMonitoringPostsObject = GetObjectKazHydrometTransportMonitoringPosts(decimaldelimiter, kazHydrometTransportMonitoringPosts);
            ViewBag.KazHydrometTransportMonitoringPostsLayerJson = kazHydrometTransportMonitoringPostsObject.ToString();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
            
            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            //string urlPollutionSources = "api/PollutionSources";
            //List<PollutionSource> pollutionSources = new List<PollutionSource>();
            //HttpResponseMessage responsePollutionSources = await _HttpApiClient.GetAsync(urlPollutionSources);
            //pollutionSources = await responsePollutionSources.Content.ReadAsAsync<List<PollutionSource>>();
            //JObject objectPollutionSources = JObject.FromObject(new
            //{
            //    type = "FeatureCollection",
            //    crs = new
            //    {
            //        type = "name",
            //        properties = new
            //        {
            //            name = "urn:ogc:def:crs:EPSG::3857"
            //        }
            //    },
            //    features = from pollutionSource in pollutionSources
            //               select new
            //               {
            //                   type = "Feature",
            //                   properties = new
            //                   {
            //                       Id = pollutionSource.Id,
            //                       Name = pollutionSource.Name
            //                   },
            //                   geometry = new
            //                   {
            //                       type = "Point",
            //                       coordinates = new List<decimal>
            //                {
            //                Convert.ToDecimal(pollutionSource.EastLongitude.ToString().Replace(".", decimaldelimiter)),
            //                Convert.ToDecimal(pollutionSource.NorthLatitude.ToString().Replace(".", decimaldelimiter))
            //                },
            //                   }
            //               }
            //});
            //ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            ////Data for calculate dissipation
            //string urlMeasuredDatas = "api/MeasuredDatas",
            //     route = "";
            ////route += string.IsNullOrEmpty(route) ? "?" : "&";
            ////route += $"MonitoringPostId={55}";
            ////List<MeasuredData> measuredDatas = new List<MeasuredData>();
            ////HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            ////measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            ////double temperature = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 4).Value);
            ////double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 5).Value);
            ////double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 6).Value);
            ////ViewBag.MeasuredData = measuredDatas;
            ////ViewBag.Temperature = temperature;
            ////ViewBag.SpeedWind = speedWind;
            ////ViewBag.DirectionWind = directionWind;

            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={4}";
            //List<MeasuredData> measuredDatas = new List<MeasuredData>();
            //HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double temperature = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            //route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={5}";
            //measuredDatas = new List<MeasuredData>();
            //responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            //route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={6}";
            //measuredDatas = new List<MeasuredData>();
            //responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);
            //ViewBag.MeasuredData = measuredDatas;
            //ViewBag.Temperature = temperature;
            //ViewBag.SpeedWind = speedWind;
            //ViewBag.DirectionWind = directionWind;

            //List<SelectListItem> pollutants = new List<SelectListItem>();
            //pollutants.Add(new SelectListItem() { Text = "Азот (II) оксид (Азота оксид) (6)", Value = "12" });
            //pollutants.Add(new SelectListItem() { Text = "Азота (IV) диоксид (Азота диоксид) (4)", Value = "13" });
            //pollutants.Add(new SelectListItem() { Text = "Сера диоксид (Ангидрид сернистый, Сернистый газ, Сера (IV) оксид) (516)", Value = "16" });
            //pollutants.Add(new SelectListItem() { Text = "Углерод оксид (Окись углерода, Угарный газ) (584)", Value = "17" });
            //pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM2,5", Value = "3" });
            //pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM10", Value = "2" });
            //ViewBag.PollutantsDessipation = pollutants;

            Task<string> jsonString = null;
            var jsonResult = Enumerable.Range(0, 0)
                .Select(e => new { Id = 0, AQI = .0m })
                .ToList();
            string urlLedScreens = "api/LEDScreens/GetAQIPosts",
                routeLEDScreens = "";
            routeLEDScreens += string.IsNullOrEmpty(routeLEDScreens) ? "?" : "&";
            routeLEDScreens += $"ProjectName=Shymkent";
            HttpResponseMessage responseLedScreens = await _HttpApiClient.GetAsync(urlLedScreens + routeLEDScreens);
            if (responseLedScreens.IsSuccessStatusCode)
            {
                jsonString = responseLedScreens.Content.ReadAsStringAsync();
            }
            var resultString = jsonString.Result.ToString();
            dynamic json = JArray.Parse(resultString);
            foreach (dynamic data in json)
            {
                int id = data.id;
                decimal aqi = data.aqi;
                jsonResult.Add(new { Id = id, AQI = aqi });
            }
            ViewBag.LEDScreensId = jsonResult.Select(r => r.Id).ToArray();
            ViewBag.LEDScreensAQI = jsonResult.Select(r => r.AQI).ToArray();

            return View();
        }

        public async Task<IActionResult> Altynalmas()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Altynalmas" || role == "Kazhydromet"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Altynalmas"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> kazHydrometWaterMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Altynalmas"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 3
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometWaterMonitoringPostsObject = GetObjectKazHydrometWaterMonitoringPosts(decimaldelimiter, kazHydrometWaterMonitoringPosts);
            ViewBag.KazHydrometWaterMonitoringPostsLayerJson = kazHydrometWaterMonitoringPostsObject.ToString();

            List<MonitoringPost> kazHydrometTransportMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Altynalmas"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 7
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometTransportMonitoringPostsObject = GetObjectKazHydrometTransportMonitoringPosts(decimaldelimiter, kazHydrometTransportMonitoringPosts);
            ViewBag.KazHydrometTransportMonitoringPostsLayerJson = kazHydrometTransportMonitoringPostsObject.ToString();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Altynalmas"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();

            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            List<LEDScreen> ledScreens = await GetLEDScreens();

            JObject objectLEDScreens = GetObjectLEDScreens(decimaldelimiter, ledScreens);
            ViewBag.LEDScreensLayerJson = objectLEDScreens.ToString();

            List<Ecopost> ecoposts = await GetEcoposts();

            JObject objectEcoposts = GetObjectEcoposts(decimaldelimiter, ecoposts);
            ViewBag.EcopostsLayerJson = objectEcoposts.ToString();

            List<ReceptionRecyclingPoint> receptionRecyclingPoints = await GetReceptionRecyclingPoints();

            JObject objectReceptionRecyclingPoints = GetObjectReceptionRecyclingPoints(decimaldelimiter, receptionRecyclingPoints);
            ViewBag.ReceptionRecyclingPointsLayerJson = objectReceptionRecyclingPoints.ToString();

            //Data for calculate dissipation
            string urlMeasuredDatas = "api/MeasuredDatas",
                 route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //List<MeasuredData> measuredDatas = new List<MeasuredData>();
            //HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double temperature = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 4).Value);
            //double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 5).Value);
            //double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault(t => t.MeasuredParameterId == 6).Value);
            //ViewBag.MeasuredData = measuredDatas;
            //ViewBag.Temperature = temperature;
            //ViewBag.SpeedWind = speedWind;
            //ViewBag.DirectionWind = directionWind;

            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={4}";
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double temperature = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={5}";
            measuredDatas = new List<MeasuredData>();
            responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MonitoringPostId={55}";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"MeasuredParameterId={6}";
            measuredDatas = new List<MeasuredData>();
            responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);
            ViewBag.MeasuredData = measuredDatas;
            ViewBag.Temperature = temperature;
            ViewBag.SpeedWind = speedWind;
            ViewBag.DirectionWind = directionWind;

            List<SelectListItem> pollutants = new List<SelectListItem>();
            pollutants.Add(new SelectListItem() { Text = "Азот (II) оксид (Азота оксид) (6)", Value = "12" });
            pollutants.Add(new SelectListItem() { Text = "Азота (IV) диоксид (Азота диоксид) (4)", Value = "13" });
            pollutants.Add(new SelectListItem() { Text = "Сера диоксид (Ангидрид сернистый, Сернистый газ, Сера (IV) оксид) (516)", Value = "16" });
            pollutants.Add(new SelectListItem() { Text = "Углерод оксид (Окись углерода, Угарный газ) (584)", Value = "17" });
            pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM2,5", Value = "3" });
            pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM10", Value = "2" });
            ViewBag.PollutantsDessipation = pollutants;

            return View();
        }

        public async Task<IActionResult> Zhanatas()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Zhanatas" || role == "Kazhydromet"))
            {
                return Redirect("/");
            }

            List<MeasuredParameter> measuredParameters = await GetMeasuredParameters();

            ViewBag.GeoServerWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressServer"].ToString();
            if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            {
                ViewBag.GeoServerAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            }
            ViewBag.GeoServerPort = Startup.Configuration["GeoServerPort"].ToString();
            //ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.MonitoringPostMeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.Pollutants = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode) && m.MPCMaxSingle != null).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 00).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            List<MonitoringPost> monitoringPosts = await GetMonitoringPosts();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Zhanatas"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = GetObjectKazHydrometAirMonitoringPostsAutomatic(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

            JObject kazHydrometAirMonitoringPostsHandsObject = GetObjectKazHydrometAirMonitoringPostsHands(decimaldelimiter, kazHydrometAirMonitoringPosts);
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m =>
                m.Project != null && m.Project.Name == "Zhanatas"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();

            JObject ecoserviceAirMonitoringPostsObject = GetObjectEcoserviceAirMonitoringPosts(decimaldelimiter, ecoserviceAirMonitoringPosts);
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();

            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

            //JObject objectPollutionSources = await GetObjectPollutionSources(decimaldelimiter);
            //ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            ////Data for calculate dissipation
            //string urlMeasuredDatas = "api/MeasuredDatas",
            //     route = "";

            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={4}";
            //List<MeasuredData> measuredDatas = new List<MeasuredData>();
            //HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double temperature = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            //route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={5}";
            //measuredDatas = new List<MeasuredData>();
            //responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double speedWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);

            //route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MonitoringPostId={55}";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"MeasuredParameterId={6}";
            //measuredDatas = new List<MeasuredData>();
            //responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + route);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            //double directionWind = Convert.ToDouble(measuredDatas.LastOrDefault().Value);
            //ViewBag.MeasuredData = measuredDatas;
            //ViewBag.Temperature = temperature;
            //ViewBag.SpeedWind = speedWind == 0 ? 1.0 : speedWind; //Если скорость ветра "0", то рассеивания нет (изолинии будут отсутствовать)
            //ViewBag.DirectionWind = directionWind;

            //List<SelectListItem> pollutants = new List<SelectListItem>();
            //pollutants.Add(new SelectListItem() { Text = "Азот (II) оксид (Азота оксид) (6)", Value = "12" });
            //pollutants.Add(new SelectListItem() { Text = "Азота (IV) диоксид (Азота диоксид) (4)", Value = "13" });
            //pollutants.Add(new SelectListItem() { Text = "Сера диоксид (Ангидрид сернистый, Сернистый газ, Сера (IV) оксид) (516)", Value = "16" });
            //pollutants.Add(new SelectListItem() { Text = "Углерод оксид (Окись углерода, Угарный газ) (584)", Value = "17" });
            //pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM2,5", Value = "3" });
            //pollutants.Add(new SelectListItem() { Text = "Взвешенные частицы PM10", Value = "2" });
            //ViewBag.PollutantsDessipation = pollutants;

            return View();
        }

        public async Task<JObject> GetObjectPollutionSources(string decimaldelimiter)
        {
            string urlPollutionSources = "api/PollutionSources";
            List<PollutionSource> pollutionSources = new List<PollutionSource>();
            HttpResponseMessage responsePollutionSources = await _HttpApiClient.GetAsync(urlPollutionSources);
            pollutionSources = await responsePollutionSources.Content.ReadAsAsync<List<PollutionSource>>();
            JObject objectPollutionSources = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from pollutionSource in pollutionSources
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = pollutionSource.Id,
                                   Name = pollutionSource.Name
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                            {
                            Convert.ToDecimal(pollutionSource.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                            Convert.ToDecimal(pollutionSource.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                            },
                               }
                           }
            });

            return objectPollutionSources;
        }

        public async Task<List<MeasuredParameter>> GetMeasuredParameters()
        {
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                return await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            return new List<MeasuredParameter>();
        }

        public async Task<List<MonitoringPost>> GetMonitoringPosts()
        {
            string urlMonitoringPosts = "api/MonitoringPosts";
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            if (responseMonitoringPosts.IsSuccessStatusCode)
            {
                return await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();
            }
            return new List<MonitoringPost>();
        }

        public JObject GetObjectKazHydrometAirMonitoringPostsAutomatic(string decimaldelimiter, List<MonitoringPost> kazHydrometAirMonitoringPosts)
        {
            List<MonitoringPost> kazHydrometAirMonitoringPostsAutomatic = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == true)
                .ToList();

            JObject kazHydrometAirMonitoringPostsAutomaticObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from monitoringPost in kazHydrometAirMonitoringPostsAutomatic
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = monitoringPost.Id,
                                   Number = monitoringPost.Number,
                                   Name = monitoringPost.Name,
                                   AdditionalInformation = monitoringPost.AdditionalInformation,
                                   DataProviderName = monitoringPost.DataProvider.Name,
                                   PollutionEnvironmentName = monitoringPost.PollutionEnvironment.Name,
                                   Automatic = monitoringPost.Automatic
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                    {
                                        Convert.ToDecimal(monitoringPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                        Convert.ToDecimal(monitoringPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                    },
                               }
                           }
            });
            return kazHydrometAirMonitoringPostsAutomaticObject;
        }

        public JObject GetObjectKazHydrometAirMonitoringPostsHands(string decimaldelimiter, List<MonitoringPost> kazHydrometAirMonitoringPosts)
        {
            List<MonitoringPost> kazHydrometAirMonitoringPostsHands = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == false)
                .ToList();

            JObject kazHydrometAirMonitoringPostsHandsObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from monitoringPost in kazHydrometAirMonitoringPostsHands
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = monitoringPost.Id,
                                   Number = monitoringPost.Number,
                                   Name = monitoringPost.Name,
                                   AdditionalInformation = monitoringPost.AdditionalInformation,
                                   DataProviderName = monitoringPost.DataProvider.Name,
                                   PollutionEnvironmentName = monitoringPost.PollutionEnvironment.Name,
                                   Automatic = monitoringPost.Automatic
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                    {
                                        Convert.ToDecimal(monitoringPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                        Convert.ToDecimal(monitoringPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                    },
                               }
                           }
            });
            return kazHydrometAirMonitoringPostsHandsObject;
        }

        public JObject GetObjectEcoserviceAirMonitoringPosts(string decimaldelimiter, List<MonitoringPost> ecoserviceAirMonitoringPosts)
        {
            JObject ecoserviceAirMonitoringPostsObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from monitoringPost in ecoserviceAirMonitoringPosts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = monitoringPost.Id,
                                   Number = monitoringPost.Number,
                                   Name = monitoringPost.Name,
                                   AdditionalInformation = monitoringPost.AdditionalInformation,
                                   DataProviderName = monitoringPost.DataProvider.Name,
                                   PollutionEnvironmentName = monitoringPost.PollutionEnvironment.Name,
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                    {
                                        Convert.ToDecimal(monitoringPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                        Convert.ToDecimal(monitoringPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                    },
                               }
                           }
            });
            return ecoserviceAirMonitoringPostsObject;
        }

        public JObject GetObjectKazHydrometWaterMonitoringPosts(string decimaldelimiter, List<MonitoringPost> kazHydrometWaterMonitoringPosts)
        {
            JObject kazHydrometWaterMonitoringPostsObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from monitoringPost in kazHydrometWaterMonitoringPosts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = monitoringPost.Id,
                                   Number = monitoringPost.Number,
                                   Name = monitoringPost.Name,
                                   AdditionalInformation = monitoringPost.AdditionalInformation,
                                   DataProviderName = monitoringPost.DataProvider.Name,
                                   PollutionEnvironmentName = monitoringPost.PollutionEnvironment.Name,
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                    {
                                        Convert.ToDecimal(monitoringPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                        Convert.ToDecimal(monitoringPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                    },
                               }
                           }
            });
            return kazHydrometWaterMonitoringPostsObject;
        }

        public JObject GetObjectKazHydrometTransportMonitoringPosts(string decimaldelimiter, List<MonitoringPost> kazHydrometTransportMonitoringPosts)
        {
            JObject kazHydrometTransportMonitoringPostsObject = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from monitoringPost in kazHydrometTransportMonitoringPosts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = monitoringPost.Id,
                                   Number = monitoringPost.Number,
                                   Name = monitoringPost.Name,
                                   AdditionalInformation = monitoringPost.AdditionalInformation,
                                   DataProviderName = monitoringPost.DataProvider.Name,
                                   PollutionEnvironmentName = monitoringPost.PollutionEnvironment.Name,
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                    {
                                        Convert.ToDecimal(monitoringPost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                        Convert.ToDecimal(monitoringPost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                    },
                               }
                           }
            });
            return kazHydrometTransportMonitoringPostsObject;
        }
        public async Task<List<LEDScreen>> GetLEDScreens()
        {
            string urlLEDScreens = "api/LEDScreens";
            HttpResponseMessage responseLEDScreens = await _HttpApiClient.GetAsync(urlLEDScreens);
            if (responseLEDScreens.IsSuccessStatusCode)
            {
                return await responseLEDScreens.Content.ReadAsAsync<List<LEDScreen>>();
            }
            return new List<LEDScreen>();
        }
        public JObject GetObjectLEDScreens(string decimaldelimiter, List<LEDScreen> ledScreens)
        {
            JObject objectLEDScreens = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from ledScreen in ledScreens
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = ledScreen.Id,
                                   Name = ledScreen.Name
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                   {
                                    Convert.ToDecimal(ledScreen.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                    Convert.ToDecimal(ledScreen.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                   },
                               }
                           }
            });
            return objectLEDScreens;
        }

        public async Task<List<Ecopost>> GetEcoposts()
        {
            string urlEcoposts = "api/Ecoposts";
            HttpResponseMessage responseEcoposts = await _HttpApiClient.GetAsync(urlEcoposts);
            if (responseEcoposts.IsSuccessStatusCode)
            {
                return await responseEcoposts.Content.ReadAsAsync<List<Ecopost>>();
            }
            return new List<Ecopost>();
        }
        public JObject GetObjectEcoposts(string decimaldelimiter, List<Ecopost> ecoposts)
        {
            JObject objectEcoposts = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from ecopost in ecoposts
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = ecopost.Id,
                                   Name = ecopost.Name
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                   {
                                    Convert.ToDecimal(ecopost.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                    Convert.ToDecimal(ecopost.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                   },
                               }
                           }
            });
            return objectEcoposts;
        }

        public async Task<List<ReceptionRecyclingPoint>> GetReceptionRecyclingPoints()
        {
            string urlReceptionRecyclingPoints = "api/ReceptionRecyclingPoints";
            List<ReceptionRecyclingPoint> receptionRecyclingPoints = new List<ReceptionRecyclingPoint>();
            HttpResponseMessage responseReceptionRecyclingPoints = await _HttpApiClient.GetAsync(urlReceptionRecyclingPoints);
            if (responseReceptionRecyclingPoints.IsSuccessStatusCode)
            {
                receptionRecyclingPoints = await responseReceptionRecyclingPoints.Content.ReadAsAsync<List<ReceptionRecyclingPoint>>();
                receptionRecyclingPoints = receptionRecyclingPoints.Where(r => r.NorthLatitude != null && r.EastLongitude != null).ToList();
            }
            return receptionRecyclingPoints;
        }
        public JObject GetObjectReceptionRecyclingPoints(string decimaldelimiter, List<ReceptionRecyclingPoint> receptionRecyclingPoints)
        {
            JObject objectReceptionRecyclingPoints = JObject.FromObject(new
            {
                type = "FeatureCollection",
                crs = new
                {
                    type = "name",
                    properties = new
                    {
                        name = "urn:ogc:def:crs:EPSG::3857"
                    }
                },
                features = from receptionRecyclingPoint in receptionRecyclingPoints
                           select new
                           {
                               type = "Feature",
                               properties = new
                               {
                                   Id = receptionRecyclingPoint.Id,
                                   Organization = receptionRecyclingPoint.Organization,
                                   Address = receptionRecyclingPoint.Address,
                                   TypesRaw = receptionRecyclingPoint.TypesRaw
                               },
                               geometry = new
                               {
                                   type = "Point",
                                   coordinates = new List<decimal>
                                   {
                                    Convert.ToDecimal(receptionRecyclingPoint.EastLongitude.ToString().Replace(".", decimaldelimiter)),
                                    Convert.ToDecimal(receptionRecyclingPoint.NorthLatitude.ToString().Replace(".", decimaldelimiter))
                                   },
                               }
                           }
            });
            return objectReceptionRecyclingPoints;
        }

        [HttpPost]
        public async Task<ActionResult> CalculateDissipation(
            float temperature,
            float windSpeed,
            float startSpeed,
            float endSpeed,
            float stepSpeed,
            float windDirection,
            float startDirection,
            float endDirection,
            float stepDirection,
            float uSpeed,
            int pollutants,
            int width,
            int length)
        {
            int code = 0301;
            decimal pdk = 0.4m;

            switch (pollutants)
            {
                case 12:
                    code = 0301;
                    pdk = 0.4m;
                    break;
                case 13:
                    code = 0304;
                    pdk = 0.085m;
                    break;
                case 16:
                    code = 0330;
                    pdk = 0.5m;
                    break;
                case 17:
                    code = 0337;
                    pdk = 5m;
                    break;
                case 3:
                    code = 0010;
                    pdk = 0.16m;
                    break;
                case 2:
                    code = 0008;
                    pdk = 0.3m;
                    break;
            }

            //string urlMeasuredDatas = "api/MeasuredDatas";
            //List<MeasuredData> measuredDatas = new List<MeasuredData>();
            //HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas);
            //measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();

            //var measuredDatas = await GetMeasuredDatas(null, pollutants, new DateTime(2000, 1, 1), DateTime.Now, true);
            List<MeasuredData> measuredDatas = new List<MeasuredData>()
            {
                new MeasuredData {PollutionSourceId = 3, MeasuredParameterId = 12, Value = 3.4711m},
                new MeasuredData {PollutionSourceId = 3, MeasuredParameterId = 13, Value = 21.3608m},
                new MeasuredData {PollutionSourceId = 3, MeasuredParameterId = 16, Value = 53.5362m},
                new MeasuredData {PollutionSourceId = 3, MeasuredParameterId = 17, Value = 2.5523m},

                new MeasuredData {PollutionSourceId = 4, MeasuredParameterId = 12, Value = 2.4711m},
                new MeasuredData {PollutionSourceId = 4, MeasuredParameterId = 13, Value = 23.3608m},
                new MeasuredData {PollutionSourceId = 4, MeasuredParameterId = 16, Value = 49.5362m},
                new MeasuredData {PollutionSourceId = 4, MeasuredParameterId = 17, Value = 5.5523m}
            };
            if (pollutants == 2)
            {
                var measuredDatasPM10 = await GetMeasuredDatas(10, 2, new DateTime(2000, 1, 1), DateTime.Now, true);
                measuredDatasPM10 = measuredDatasPM10.OrderBy(m => m.DateTime).ToList();
                MeasuredData measuredDataPM101 = new MeasuredData {
                    PollutionSourceId = 3,
                    MeasuredParameterId = 2,
                    Value = measuredDatasPM10.LastOrDefault().Value
                };
                MeasuredData measuredDataPM102 = new MeasuredData
                {
                    PollutionSourceId = 4,
                    MeasuredParameterId = 2,
                    Value = measuredDatasPM10.LastOrDefault().Value
                };
                measuredDatas.Add(measuredDataPM101);
                measuredDatas.Add(measuredDataPM102);
            }
            if (pollutants == 3)
            {
                var measuredDatasPM25 = await GetMeasuredDatas(10, 3, new DateTime(2000, 1, 1), DateTime.Now, true);
                measuredDatasPM25 = measuredDatasPM25.OrderBy(m => m.DateTime).ToList();
                MeasuredData measuredDataPM251 = new MeasuredData
                {
                    PollutionSourceId = 3,
                    MeasuredParameterId = 3,
                    Value = measuredDatasPM25.LastOrDefault().Value
                };
                MeasuredData measuredDataPM252 = new MeasuredData
                {
                    PollutionSourceId = 4,
                    MeasuredParameterId = 3,
                    Value = measuredDatasPM25.LastOrDefault().Value
                };
                measuredDatas.Add(measuredDataPM251);
                measuredDatas.Add(measuredDataPM252);
            }

            List<double> pollutantsValue = new List<double>();
            pollutantsValue.Add(Convert.ToDouble(measuredDatas.Where(p => p.PollutionSourceId == 3).LastOrDefault(p => p.MeasuredParameterId == pollutants).Value));
            pollutantsValue.Add(Convert.ToDouble(measuredDatas.Where(p => p.PollutionSourceId == 4).LastOrDefault(p => p.MeasuredParameterId == pollutants).Value));

            List<double> longitude = new List<double> { 8572410, 8572371 };
            List<double> latitude = new List<double> { 5376722, 5376650 };

            List<double> height = new List<double> { 20, 4 };
            List<double> diameter = new List<double> { 0.5, 0.25 };
            List<double> flow_temperature = new List<double> { 24, 20 };
            List<double> flow_speed = new List<double> { 1, 8.5 };

            var reqCalCreate = new ReqCalcCreate();
            for (int i = 0; i < pollutantsValue.Count; i++)
            {
                reqCalCreate.AirPollutionSources.Add(
                    new AirPollutionSource()
                    {
                        Id = i,
                        IsOrganized = true,
                        Methodical = 1,
                        BackgroundRelation = 3,
                        Configuration = new Configuration()
                        {
                            Type = 1,
                            Height = height[i],
                            Diameter = diameter[i],
                            FlowTemperature = flow_temperature[i],
                            FlowSpeed = flow_speed[i],
                            ReliefCoefficient = 1,
                            Point1 = new Point1()
                            {
                                //X и Y временно поменяны местами, чтобы было правильное отображение
                                Y = longitude[i],
                                X = latitude[i],
                                Z = 0
                            }
                        },
                        Emissions = new List<Emission>()
                        {
                            new Emission()
                            {
                                PollutantCode = code,
                                Power = pollutantsValue[i],
                                Coefficient = 2
                            }
                        }
                    }
                );
            }
            reqCalCreate.ThresholdPdk = 0;
            reqCalCreate.Locality = new Locality()
            {
                Square = 682,
                ReliefCoefficient = 1,
                StratificationCoefficient = 200
            };
            reqCalCreate.Meteo = new Meteo()
            {
                Temperature = temperature,
                WindSpeedSettings = new WindSpeedSettings()
                {
                    Mode = 1,
                    Speed = windSpeed,
                    StartSpeed = startSpeed,
                    EndSpeed = endSpeed,
                    StepSpeed = stepSpeed
                },
                WindDirectionSettings = new WindDirectionSettings()
                {
                    Mode = 1,
                    Direction = windDirection,
                    StartDirection = startDirection,
                    EndDirection = endDirection,
                    StepDirection = stepDirection
                },
                USpeed = uSpeed
            };
            reqCalCreate.Background = new Background()
            {
                Mode = 0
            };
            reqCalCreate.Method = 1;
            reqCalCreate.ContributorCount = pollutantsValue.Count;
            reqCalCreate.UseSummationGroups = false;
            reqCalCreate.CalculatedArea = new CalculatedArea
            {
                Rectangles = new List<Rectangle>()
                {
                    new Rectangle()
                    {
                        Id = 0,
                        CenterPoint = new CenterPoint()
                        {
                            //X и Y временно поменяны местами, чтобы было правильное отображение
                            Y = 8572343.29,
                            X = 5376759.33,
                            Z = 0
                        },
                        Width = width,
                        Length = length,
                        Height = 1,
                        StepByWidth = 100,
                        StepByLength = 100
                    }
                }
            };
            reqCalCreate.Pollutants = new List<Models.UPRZA.Pollutant>()
            {
                new Models.UPRZA.Pollutant()
                {
                    Code = code,
                    Pdk = Convert.ToDouble(pdk, CultureInfo.InvariantCulture)
                }
            };

            string jsonContent = JsonConvert.SerializeObject(reqCalCreate).Replace(@"""", @"\""");

            bool server = Convert.ToBoolean(Startup.Configuration["Server"]);
            string URPZAUrl = server ? Startup.Configuration["URPZAUrlServer"] : Startup.Configuration["URPZAUrlDebug"];

            string calculate = "-X POST \"" + URPZAUrl + "calculation/create\" -H \"accept: application/json\" -H \"Content-Type: application/json\" -d \"" + jsonContent + "\"";
            Process process = CurlExecute(calculate);
            string answer = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var jObject = JObject.Parse(answer);
            int id = (int)jObject["id"];

            //string statusQuery = "-X GET \"http://185.125.44.116:50006/calculation/status?id=" + id + "\" -H \"accept: application/json\"";
            string statusQuery = "-X GET \"" + URPZAUrl + "calculation/status?id=" + id + "\" -H \"accept: application/json\"";
            process = CurlExecute(statusQuery);
            answer = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            jObject = JObject.Parse(answer);
            string status = (string)jObject["status"];

            while (status != "ready")
            {
                process = CurlExecute(statusQuery);
                answer = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                jObject = JObject.Parse(answer);
                status = (string)jObject["status"];
            }
            //string resultEmissions = "-X GET \"http://185.125.44.116:50006/result-emissions?jobId=" + id + "&containerType=rectangle&containerId=0&pollutantCode=" + code + "\" -H \"accept: application/json\"";
            string resultEmissions = "-X GET \"" + URPZAUrl + "result-emissions?jobId=" + id + "&containerType=rectangle&containerId=0&pollutantCode=" + code + "\" -H \"accept: application/json\"";
            process = CurlExecute(resultEmissions);
            answer = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            answer = answer
                .Replace("\"type\":8", "\"type\":\"FeatureCollection\"")
                .Replace("\"type\":7", "\"type\":\"Feature\"")
                .Replace("\"type\":0", "\"type\":\"Point\"")
                .Replace("\"C\"", "\"c\"")
                .Replace("\"Cpdk\"", "\"c_pdk\"");
            if (!answer.Contains("coordinates"))
            {
                dynamic datas = JObject.Parse(answer);
                dynamic features = datas.features;
                foreach (var feature in features)
                {
                    var x = feature.properties.X;
                    var y = feature.properties.Y;
                    var z = feature.properties.Z;
                    feature.geometry.Add(new JProperty("coordinates", new JArray(x, y, z)));
                }
                answer = Convert.ToString(datas);
            }

            return Json(new
            {
                answer
            });
        }

        private Process CurlExecute(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = Startup.Configuration["CurlFullPath"];
                process.StartInfo.Arguments = Arguments;
                process.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        [HttpPost]
        public async Task<IActionResult> GetTodayTimes20()
        {
            List<DateTime> times20l = new List<DateTime>();
            for (DateTime i = DateTime.Today; i <= DateTime.Now; i = i.AddMinutes(20))
            {
                times20l.Add(i);
            }
            DateTime[] times20 = times20l.ToArray();
            return Json(new
            {
                times20
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetLayersCount(
            string LayerBaseName)
        {
            string GSUser = Startup.Configuration["GeoServerUser"].ToString(),
                GSPassword = Startup.Configuration["GeoServerPassword"].ToString(),
                GSAddress = Startup.Configuration["GeoServerAddressDebug"].ToString(),
                GSPort = Startup.Configuration["GeoServerPort"].ToString(),
                GSWorkspace = Startup.Configuration["GeoServerWorkspace"].ToString();
            //if (!Convert.ToBoolean(Startup.Configuration["Server"]))
            //{
            //    GSAddress = Startup.Configuration["GeoServerAddressDebug"].ToString();
            //}
            int count = 0;
            while (true)
            {
                string Layer = LayerBaseName + "_" + (count + 1).ToString();
                Process process = CurlExecute($" -u " +
                    $"{GSUser}:" +
                    $"{GSPassword}" +
                    $" -XGET" +
                    $" http://{GSAddress}:" +
                    $"{GSPort}/geoserver/rest/layers/{GSWorkspace}:{Layer}.json");
                string json = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (json.Contains("No such layer"))
                {
                    break;
                }
                else
                {
                    count++;
                }
            }
            return Json(new
            {
                count
            });
        }

        [HttpPost]
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

            DateTime dateTimeFrom, dateTimeTo;
            dateTimeFrom = DateTime.Now.AddMinutes(-Convert.ToInt32(Startup.Configuration["InactivePastMinutes"]));
            dateTimeTo = DateTime.Now;
            List<int> measuredParametersEmpty = new List<int>();
            var measuredDatas = await GetMeasuredDatas(MonitoringPostId, null, dateTimeFrom, dateTimeTo, true);
            if (result[0].MonitoringPost.DataProviderId != 1)
            {
                foreach (var item in result)
                {
                    var measuredData = measuredDatas.Where(m => m.MeasuredParameterId == item.MeasuredParameterId).FirstOrDefault();
                    if (measuredData == null)
                    {
                        measuredParametersEmpty.Add(item.MeasuredParameterId);
                    }
                }
                if (measuredParametersEmpty.Count != 0)
                {
                    foreach (var item in measuredParametersEmpty)
                    {
                        result.RemoveAll(r => r.MeasuredParameterId == item);
                    }
                }
            }

            return Json(
                result
            );
        }

        public async Task<IActionResult> GetMeasuredParametersPollutionSource()
        {
            //List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            //string urlMeasuredParameters = "api/MeasuredParameters";
            //string routeMeasuredParameters = "";
            //HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            //if (responseMeasuredParameters.IsSuccessStatusCode)
            //{
            //    measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            //}
            //var result = measuredParameters.Where(m => m.Id == 9).OrderBy(m => m.Name).ToList();

            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            string urlMeasuredDatas = "api/MeasuredDatas/pollutionSource";
            string routeMeasuredDatas = "";

            routeMeasuredDatas += string.IsNullOrEmpty(routeMeasuredDatas) ? "?" : "&";
            routeMeasuredDatas += $"PollutionSourceId=5";

            HttpResponseMessage responseMeasuredDatas = await _HttpApiClient.GetAsync(urlMeasuredDatas + routeMeasuredDatas);
            if (responseMeasuredDatas.IsSuccessStatusCode)
            {
                measuredDatas = await responseMeasuredDatas.Content.ReadAsAsync<List<MeasuredData>>();
            }

            var result = measuredDatas
                .Select(m => new { m.MeasuredParameter.Id, m.MeasuredParameter.Name })
                .Distinct()
                .ToList();

            return Json(
                result
            );
        }

        public async Task<IList<MeasuredData>> GetMeasuredDatas(
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
            return measureddatas;
        }

        public async Task<IActionResult> GetMinMax(
            int MonitoringPostId,
            int MeasuredParameterId)
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
            var min = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault()?.Min;
            var max = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault()?.Max;
            var minMeasured = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault()?.MinMeasuredValue;
            var maxMeasured = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault()?.MaxMeasuredValue;

            return Json(new
            {
                min,
                max,
                minMeasured,
                maxMeasured
            }
            );
        }

        [HttpPost]
        public async Task<IActionResult> GetMeasuredParameterMPC(
            string GrayIndex,
            int MeasuredParameterId)
        {
            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }
            //var measuredParameter = measuredParameters.Where(m => m.Id == MeasuredParameterId).ToList().FirstOrDefault();
            var measuredParameter = measuredParameters.FirstOrDefault(m => m.Id == MeasuredParameterId);
            var value = Decimal.Parse(GrayIndex, CultureInfo.InvariantCulture) * measuredParameter.MPCMaxSingle;
            var name = measuredParameter.Name;

            return Json(new
            {
                name,
                value
            });
        }
    }
}
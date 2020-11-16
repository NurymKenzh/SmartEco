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
using Newtonsoft.Json.Linq;
using SmartEco.Models;

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
            ViewBag.SpeedWind = speedWind;
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

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }

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

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "KaragandaRegion"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsAutomatic = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsHands = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == false)
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
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

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
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "KaragandaRegion"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

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
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            return View();
        }

        public async Task<IActionResult> Arys()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Arys"))
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

            ViewBag.MeasuredParameters = new SelectList(measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).OrderBy(m => m.Name), "Id", "Name");
            ViewBag.DateFrom = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeFrom = (DateTime.Today).ToString("HH:mm:ss");
            ViewBag.DateTo = (DateTime.Now).ToString("yyyy-MM-dd");
            ViewBag.TimeTo = new DateTime(2000, 1, 1, 23, 59, 59).ToString("HH:mm:ss");

            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 42.32M && m.NorthLatitude <= 42.50M*/
                    m.Project != null && m.Project.Name == "Arys"
                    && m.EastLongitude >= 68.6M && m.EastLongitude <= 69.0M
                    && m.TurnOnOff == true)
                .Where(m => m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString())
                .ToList();
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

            List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
            string urlMeasuredParameters = "api/MeasuredParameters",
                routeMeasuredParameters = "";
            HttpResponseMessage responseMeasuredParameters = await _HttpApiClient.GetAsync(urlMeasuredParameters + routeMeasuredParameters);
            if (responseMeasuredParameters.IsSuccessStatusCode)
            {
                measuredParameters = await responseMeasuredParameters.Content.ReadAsAsync<List<MeasuredParameter>>();
            }

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

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsAutomatic = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsHands = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == false)
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
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

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
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> kazHydrometWaterMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 3
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.KazHydrometWaterMonitoringPostsLayerJson = kazHydrometWaterMonitoringPostsObject.ToString();

            List<MonitoringPost> kazHydrometTransportMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 7
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.KazHydrometTransportMonitoringPostsLayerJson = kazHydrometTransportMonitoringPostsObject.ToString();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Almaty"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.EcoserviceAirMonitoringPostsLayerJson = ecoserviceAirMonitoringPostsObject.ToString();
            ViewBag.EcoserviceAirMonitoringPosts = ecoserviceAirMonitoringPosts.ToArray();

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
            ViewBag.PollutionSourcesLayerJson = objectPollutionSources.ToString();

            string urlLEDScreens = "api/LEDScreens";
            List<LEDScreen> ledScreens = new List<LEDScreen>();
            HttpResponseMessage responseLEDScreens = await _HttpApiClient.GetAsync(urlLEDScreens);
            ledScreens = await responseLEDScreens.Content.ReadAsAsync<List<LEDScreen>>();
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
            ViewBag.LEDScreensLayerJson = objectLEDScreens.ToString();

            string urlEcoposts = "api/Ecoposts";
            List<Ecopost> ecoposts = new List<Ecopost>();
            HttpResponseMessage responseEcoposts = await _HttpApiClient.GetAsync(urlEcoposts);
            ecoposts = await responseEcoposts.Content.ReadAsAsync<List<Ecopost>>();
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
            ViewBag.EcopostsLayerJson = objectEcoposts.ToString();

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

        public async Task<IActionResult> Shymkent()
        {
            string role = HttpContext.Session.GetString("Role");
            if (!(role == "admin" || role == "moderator" || role == "Shymkent" || role == "Kazhydromet"))
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

            string urlMonitoringPosts = "api/MonitoringPosts";
            List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
            HttpResponseMessage responseMonitoringPosts = await _HttpApiClient.GetAsync(urlMonitoringPosts);
            monitoringPosts = await responseMonitoringPosts.Content.ReadAsAsync<List<MonitoringPost>>();

            List<MonitoringPost> kazHydrometAirMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 2
                && m.TurnOnOff == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsAutomatic = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == true)
                .ToList();
            List<MonitoringPost> kazHydrometAirMonitoringPostsHands = kazHydrometAirMonitoringPosts
                .Where(m => m.Automatic == false)
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
            ViewBag.KazHydrometAirMonitoringPostsAutomaticLayerJson = kazHydrometAirMonitoringPostsAutomaticObject.ToString();

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
            ViewBag.KazHydrometAirMonitoringPostsHandsLayerJson = kazHydrometAirMonitoringPostsHandsObject.ToString();

            ViewBag.KazHydrometAirMonitoringPosts = kazHydrometAirMonitoringPosts.ToArray();

            List<MonitoringPost> kazHydrometWaterMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 3
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.KazHydrometWaterMonitoringPostsLayerJson = kazHydrometWaterMonitoringPostsObject.ToString();

            List<MonitoringPost> kazHydrometTransportMonitoringPosts = monitoringPosts
                .Where(m => m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["KazhydrometName"].ToString()
                && m.PollutionEnvironmentId == 7
                && m.TurnOnOff == true)
                .ToList();
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
            ViewBag.KazHydrometTransportMonitoringPostsLayerJson = kazHydrometTransportMonitoringPostsObject.ToString();

            List<MonitoringPost> ecoserviceAirMonitoringPosts = monitoringPosts
                .Where(m => /*m.NorthLatitude >= 46.00M && m.NorthLatitude <= 51.00M*/
                m.Project != null && m.Project.Name == "Shymkent"
                && m.DataProvider.Name == Startup.Configuration["EcoserviceName"].ToString()
                && m.TurnOnOff == true)
                .ToList();
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

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CalculateDissipation(float temperature,
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
            float width,
            float length)
        {
            int code = 0301;
            if (pollutants == 12)
            {
                code = 0301;
            }
            if (pollutants == 13)
            {
                code = 0304;
            }
            if (pollutants == 16)
            {
                code = 0330;
            }
            if (pollutants == 17)
            {
                code = 0337;
            }
            if (pollutants == 3)
            {
                code = 0010;
            }
            if (pollutants == 2)
            {
                code = 0008;
            }

            string temperatureString = Convert.ToString(temperature, CultureInfo.InvariantCulture);
            string windSpeedString = Convert.ToString(windSpeed, CultureInfo.InvariantCulture);
            string startSpeedString = Convert.ToString(startSpeed, CultureInfo.InvariantCulture);
            string endSpeedString = Convert.ToString(endSpeed, CultureInfo.InvariantCulture);
            string stepSpeedString = Convert.ToString(stepSpeed, CultureInfo.InvariantCulture);
            string windDirectionString = Convert.ToString(windDirection, CultureInfo.InvariantCulture);
            string startDirectionString = Convert.ToString(startDirection, CultureInfo.InvariantCulture);
            string endDirectionString = Convert.ToString(endDirection, CultureInfo.InvariantCulture);
            string stepDirectionString = Convert.ToString(stepDirection, CultureInfo.InvariantCulture);
            string uSpeedString = Convert.ToString(uSpeed, CultureInfo.InvariantCulture);
            string widthString = Convert.ToString(width, CultureInfo.InvariantCulture);
            string lengthString = Convert.ToString(length, CultureInfo.InvariantCulture);

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
            //pollutantsValue.Add(Convert.ToDouble(measuredDatas.Where(p => p.PollutionSourceId == 3).LastOrDefault().Value));
            //pollutantsValue.Add(Convert.ToDouble(measuredDatas.Where(p => p.PollutionSourceId == 4).LastOrDefault().Value));

            List<string> pollutantsValueString = new List<string>();
            foreach (double pollutantValue in pollutantsValue)
            {
                pollutantsValueString.Add(Convert.ToString(pollutantValue, CultureInfo.InvariantCulture));
            }

            //List<string> longitude = new List<string> { "76.89392209053041", "76.89093410968779" };
            //List<string> latitude = new List<string> { "43.25245478496336", "43.252024999269906" };
            //List<string> longitude = new List<string> { "77.00667", "76.89093410968779" };
            //List<string> latitude = new List<string> { "43.42417", "43.252024999269906" };

            //List<string> longitude = new List<string> { "8572102.20", "8572696.97" };
            //List<string> latitude = new List<string> { "5376224.66", "5376683.28" };
            List<string> longitude = new List<string> { "8572410", "8572371" };
            List<string> latitude = new List<string> { "5376722", "5376650" };

            List<string> height = new List<string> { "20", "4" };
            List<string> diameter = new List<string> { "0.5", "0.25" };
            List<string> flow_temperature = new List<string> { "24", "20" };
            List<string> flow_speed = new List<string> { "1", "8.5" };

            string content = "";
            //string airPollutionSources = "\"air_pollution_sources\": [ ";
            string airPollutionSources = @"air_pollution_sources\"": [ ";

            //Old version UPRZA-kernel
            for (int i = 0; i < pollutantsValueString.Count; i++)
            {
                airPollutionSources += @"{ \""id\"": " + i + @", \""is_organized\"": true, \""methodical\"": 1, \""background_relation\"": 3, " +
                    @"\""configuration\"": { \""type\"": 1, \""height\"": " + height[i] + @", \""diameter\"": " + diameter[i] + @", \""flow_temperature\"": " + flow_temperature[i] + @", \""flow_speed\"": " + flow_speed[i] + @", " +
                    @"\""point_1\"": { \""x\"": " + longitude[i] + @", \""y\"": " + latitude[i] + @", \""z\"": 0 }, \""relief_coefficient\"": 1 }, \""emissions\"": [";
                airPollutionSources += @"{ \""pollutant_code\"": " + code + @", \""power\"": " + pollutantsValueString[i] + @", \""coefficient\"": 2 }";
                airPollutionSources += " ] }";
                if (i < pollutantsValueString.Count - 1)
                {
                    airPollutionSources += ", ";
                }
            }
            airPollutionSources += " ], ";

            content = @"{ \""threshold_pdk\"": 0, \""locality\"": { \""square\"": 682, \""relief_coefficient\"": 1, \""stratification_coefficient\"": 200 }, \""meteo\"": " +
                @"{ \""temperature\"": " + temperatureString + @", \""wind_speed_settings\"": { \""mode\"": 1, \""speed\"": " + windSpeedString + @", \""start_speed\"": " + startSpeedString + @", \""end_speed\"": " + endSpeedString + @", \""step_speed\"": " + stepSpeedString + @" }, " +
                @"\""wind_direction_settings\"": { \""mode\"": 1, \""direction\"": " + windDirectionString + @", \""start_direction\"": " + startDirectionString + @", \""end_direction\"": " + endDirectionString + @", \""step_direction\"": " + stepDirectionString + @" }, \""u_speed\"": " + uSpeedString + @" }" +
                @", \""background\"": { \""mode\"": 0 }, \""method\"": 1, \""contributor_count\"": " + pollutantsValueString.Count + @", \""use_summation_groups\"": false, \""";
            content += airPollutionSources;
            //content += "\"calculated_area\": { \"rectangles\": [{ \"id\": 0, \"center_point\": { \"y\": 43.42417, \"x\": 77.00667, \"z\": 0 }, \"width\": 10, \"length\": 10, \"height\": 1, \"step_by_width\": 1, \"step_by_length\": 1 }], \"points\": [], \"lines\": [] }}";
            content += @"\""calculated_area\"": { \""rectangles\"": [{ \""id\"": 0, \""center_point\"": { \""y\"": 5376759.33, \""x\"": 8572343.29, \""z\"": 0 }, \""width\"": " + widthString + @", \""length\"": " + lengthString + @", \""height\"": 1, \""step_by_width\"": 100, \""step_by_length\"": 100 }], \""points\"": [], \""lines\"": [] }, \""buildings\"": []}";

            //New version UPRZA-kernel
            //content += "{ \"contributor_count\": 2, \"locality\": { \"square\": 682, \"stratification_coefficient\": 200, \"relief_coefficient\": 1 }, " +
            //    "\"calculated_area\": { \"points\": [], \"rectangles\": [{ \"step_y\": 100, \"step_x\": 100, \"number\": 0, \"right_top_point\": { " +
            //    "\"y\": 5378759.33, \"x\": 8574343.29, \"z\": 1 }, \"left_bottom_point\": { \"y\": 5374759.33, \"x\": 8570343.29, \"z\": 1 }}]}, " +
            //    "\"threshold_pdk\": 0, \"meteo\": { \"wind_speed_settings\": { \"end_speed\": 2, \"step_speed\": 0.1, \"mode\": 1, \"start_speed\": 0 }, " +
            //    "\"wind_direction_settings\": { \"end_direction\": 360, \"mode\": 1, \"step_direction\": 1, \"start_direction\": 0 }, " +
            //    "\"rose_of_wind\": { \"east\": 0, \"north\": 0, \"southwest\": 0, \"west\": 0, \"northeast\": 0, \"northwest\": 0, \"southeast\": 0, \"south\": 0 }, " +
            //    "\"temperature\": 39.93, \"u_speed\": 1 }, \"use_summation_groups\": false, ";

            //for (int i = 0; i < pollutantsValueString.Count; i++)
            //{
            //    airPollutionSources += "{ \"count\": 1, \"methodical\": 1, \"is_organized\": true, \"relief_coefficient\": 1, \"emissions\": [{ " +
            //        "\"coefficient\": 2, \"power\": 3.4711, \"pollutant_code\": 301 }], \"configuration\": { \"diameter\": 0.5, \"flow_temperature\": 24, " +
            //        "\"flow_speed\": 1, \"height\": 20, \"type\": 1, \"point_1\": { \"y\": 5376722, \"x\": 8572410, \"z\": 0 }}, \"id\": 0 }";
            //    if (i < pollutantsValueString.Count - 1)
            //    {
            //        airPollutionSources += ", ";
            //    }
            //}
            //airPollutionSources += " ], ";

            //content += airPollutionSources;
            //content += "\"background\": { \"mode\": 0 }, \"method\": 1 }";

            ////content += "{ \"contributor_count\": 10, \"locality\": { \"square\": 1, \"stratification_coefficient\": 0, \"relief_coefficient\": 0 }, \"calculated_area\": { \"points\": [], \"rectangles\": [ { \"step_y\": 50, \"step_x\": 50, \"number\": 0, \"right_top_point\": { \"y\": 2000, \"x\": 2000, \"z\": 2 }, \"left_bottom_point\": { \"y\": 0, \"x\": 0, \"z\": 2 } } ] }, \"threshold_pdk\": 0, \"meteo\": { \"wind_speed_settings\": { \"end_speed\": 11, \"step_speed\": 0.1, \"mode\": 3, \"start_speed\": 0.5 }, \"wind_direction_settings\": { \"end_direction\": 360, \"mode\": 3, \"step_direction\": 1, \"start_direction\": 0 }, \"rose_of_wind\": { \"east\": 0, \"north\": 0, \"southwest\": 0, \"west\": 0, \"northeast\": 0, \"northwest\": 0, \"southeast\": 0, \"south\": 0 }, \"temperature\": 0, \"u_speed\": 11 }, \"use_summation_groups\": false, \"air_pollution_sources\": [ { \"count\": 1, \"methodical\": 1, \"is_organized\": true, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 350, \"flow_speed\": 7.957747155, \"height\": 10, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 76 }, { \"count\": 1, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 100, \"flow_speed\": 7.957747154619572, \"height\": 10, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 76 }, { \"count\": 1, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 4.5, \"flow_speed\": 7.957747154619572, \"height\": 10, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 76 }, { \"count\": 1, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 3, \"flow_speed\": 7.957747154619572, \"height\": 10, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 76 }, { \"count\": 1, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 0, \"flow_speed\": 7.957747154619572, \"height\": 10, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 76 }, { \"count\": 2, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 0, \"flow_speed\": 7.957747154619572, \"height\": 5, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 77 }, { \"count\": 2, \"is_organized\": true, \"methodical\": 1, \"relief_coefficient\": 1, \"emissions\": [ { \"coefficient\": 1, \"power\": 2, \"pollutant_code\": 1063 } ], \"configuration\": { \"diameter\": 0.4, \"flow_temperature\": 0, \"flow_speed\": 7.957747154619572, \"height\": 1, \"type\": 1, \"point_1\": { \"y\": 0, \"x\": 0, \"z\": 0 } }, \"id\": 77 } ], \"background\": { \"post_monitors\": [], \"value_type\": 1, \"mode\": 0 }, \"method\": 1 }";

            bool server = Convert.ToBoolean(Startup.Configuration["Server"]);
            string URPZAUrl = server ? Startup.Configuration["URPZAUrlServer"] : Startup.Configuration["URPZAUrlDebug"];

            //string calculate = "-X POST \"http://185.125.44.116:50006/calculation/create\" -H \"accept: application/json\" -H \"Content-Type: application/json\" -d \"" + content + "\"";
            string calculate = "-X POST \"" + URPZAUrl + "calculation/create\" -H \"accept: application/json\" -H \"Content-Type: application/json\" -d \"" + content + "\"";
            //string calculate = "-X POST \"" + URPZAUrl + "uprza/calculations/create\" -H \"Authorization: Bearer t9TfgXMhr6CNVTPmKFlYzMCh3JdUjYH4\" -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36\" -d \"" + content + "\"";
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
            var min = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().Min;
            var max = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().Max;
            var minMeasured = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().MinMeasuredValue;
            var maxMeasured = monitoringPostMeasuredParameters.Where(m => m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().MaxMeasuredValue;

            return Json(new
            {
                min,
                max,
                minMeasured,
                maxMeasured
            }
            );
        }
    }
}
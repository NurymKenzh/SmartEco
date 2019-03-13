using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SmartEco.Models;

namespace SmartEco.Controllers
{
    public class MapsController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public MapsController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        public async Task<IActionResult> Index()
        {
            string decimaldelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

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

            return View();
        }
    }
}
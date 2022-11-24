using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartEco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers
{
    public class LEDDisplayController : Controller
    {
        private readonly HttpApiClientController _httpApiClient;

        public LEDDisplayController(HttpApiClientController httpApiClient)
        {
            _httpApiClient = httpApiClient;
        }


        public async Task<IActionResult> Temirtau()
        {
            return View();
        }

        [HttpGet]
        public async Task<List<DisplayData>> GetDisplayData(string namePosts)
        {
            string url = "api/LEDDisplay/GetLedWebDatas",
               route = "";
            route += string.IsNullOrEmpty(route) ? "?" : "&";
            route += $"namePosts={namePosts}";
            HttpResponseMessage response = await _httpApiClient.GetAsync(url + route);

            Data data = new Data();
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<Data>(resultContent);
            }

            List<DisplayData> displayDatas = new List<DisplayData>();
            foreach (var monitoringPost in data.MonitoringPosts.OrderBy(m => m.Id))
            {
                DisplayData displayData = new DisplayData()
                {
                    NamePost = monitoringPost.Name,
                    AddressPost = monitoringPost.AdditionalInformation,
                    DateMeasurement = data.MeasuredDatas
                        .FirstOrDefault(m => m.MonitoringPostId == monitoringPost.Id)?.DateTime.Value.ToString("dd.MM.yyyy HH:mm")
                };
                foreach (var measuredParameter in data.MeasuredParameters)
                {
                    //Временная мера. С постов получаем неверные данные по температуре. Пока что берём с поста Temir-005
                    MeasuredData measuredData = null;
                    if (measuredParameter.Id == 4)
                    {
                        measuredData = data.MeasuredDatas
                            .FirstOrDefault(m => m.MonitoringPostId == 166 && m.MeasuredParameterId == measuredParameter.Id);
                    }
                    else
                    {
                        measuredData = data.MeasuredDatas
                            .FirstOrDefault(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == measuredParameter.Id);
                    }

                    displayData.Parameters.Add(new DisplayData.Parameter()
                    {
                        Id = measuredParameter.Id,
                        Value = measuredData?.Value,
                        MPC = measuredParameter.MPCMaxSingle
                    });
                }
                displayDatas.Add(displayData);
            }

            return displayDatas;
        }
    }

    public class Data
    {
        public Data()
        {
            MeasuredDatas = new List<MeasuredData>();
            MonitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameters>();
            MeasuredParameters = new List<MeasuredParameter>();
            MonitoringPosts = new List<MonitoringPost>();
        }

        public List<MeasuredData> MeasuredDatas { get; set; }
        public List<MonitoringPostMeasuredParameters> MonitoringPostMeasuredParameters { get; set; }
        public List<MeasuredParameter> MeasuredParameters { get; set; }
        public List<MonitoringPost> MonitoringPosts { get; set; }
    }

    public class DisplayData
    {
        public DisplayData()
        {
            Parameters = new List<Parameter>();
        }

        public string NamePost { get; set; }
        public string AddressPost { get; set; }
        public string DateMeasurement { get; set; }
        public List<Parameter> Parameters { get; set; }

        public class Parameter
        {
            public int Id { get; set; }
            public decimal? Value { get; set; }
            public decimal? MPC { get; set; }
        }
    }
}

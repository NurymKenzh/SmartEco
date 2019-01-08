using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace EcomonDownloader
{
    public class EcomonMonitoringPoint
    {
        public int Id { get; set; }
        public int Number { get; set; }
    }

    public class MeasuredParameter
    {
        public int Id { get; set; }
        public string NameKK { get; set; }
        public string NameRU { get; set; }
        public string NameEN { get; set; }
        public int? EcomonCode { get; set; }
    }

    public class MeasuredData
    {
        public long Id { get; set; }
        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
        public int? EcomonMonitoringPointId { get; set; }
        public EcomonMonitoringPoint EcomonMonitoringPoint { get; set; }
        public long? Ecomontimestamp_ms { get; set; }
    }

    class Program
    {
        public static string TokenFileName = @"Token.txt";

        static void Main(string[] args)
        {
            int sleepSeconds = 120;
            while(true)
            {
                string token = File.ReadAllText(TokenFileName);
                long start = 0,
                    finish = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 6, 0, 0, 0)).TotalSeconds);
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<EcomonMonitoringPoint> ecomonMonitoringPoints = new List<EcomonMonitoringPoint>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    var startDB = connection.Query<long?>("SELECT \"Ecomontimestamp_ms\" FROM public.\"MeasuredData\" ORDER BY \"Ecomontimestamp_ms\" DESC LIMIT 1;");
                    if (startDB.Count() != 0)
                    {
                        start = (long)startDB.FirstOrDefault() / 1000;
                    }
                    if (start == 0)
                    {
                        start = Convert.ToInt64((DateTime.Now.AddDays(-14) - new DateTime(1970, 1, 1, 6, 0, 0, 0)).TotalSeconds);
                    }

                    var measuredParametersDB = connection.Query<MeasuredParameter>("SELECT \"Id\", \"NameKK\", \"NameRU\", \"NameEN\", \"EcomonCode\" FROM public.\"MeasuredParameter\";");
                    measuredParameters = measuredParametersDB.Where(m => m.EcomonCode != null).ToList();
                    var ecomonMonitoringPointsDB = connection.Query<EcomonMonitoringPoint>("SELECT \"Id\", \"Number\" FROM public.\"EcomonMonitoringPoint\";");
                    ecomonMonitoringPoints = ecomonMonitoringPointsDB.ToList();
                    var measuredDatasDB = connection.Query<MeasuredData>("SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"EcomonMonitoringPointId\", \"Ecomontimestamp_ms\" FROM public.\"MeasuredData\"");
                    foreach (MeasuredParameter measuredParameter in measuredParameters)
                    {
                        DownloadOne(measuredDatasDB.ToList(), measuredParameter, ecomonMonitoringPoints.FirstOrDefault(), token, start, finish);
                    }
                }
                Thread.Sleep(sleepSeconds * 1000);
            }
        }

        static async void DownloadOne(List<MeasuredData> MeasuredDatas, MeasuredParameter MeasuredParameter, EcomonMonitoringPoint EcomonMonitoringPoint, string Token, long Start, long Finish)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string> { });
            Token = $"Bearer {Token}";
            client.DefaultRequestHeaders.Add("authorization", Token);
            var response = await client.GetAsync($"https://central.demo.svc.urusit.com/meters/{MeasuredParameter.EcomonCode.ToString()}/readings?from={Start.ToString()}&to={Finish.ToString()}");
            var responseString = response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseString.Result);
            var inner = json["readings"];
            JArray a = JArray.Parse(JsonConvert.SerializeObject(inner));
            foreach (JObject o in a.Children<JObject>())
            {
                MeasuredData measuredData = new MeasuredData();
                foreach (JProperty p in o.Properties())
                {
                    string name = p.Name;
                    string value = (string)p.Value;
                    if (name == "timestamp_ms")
                    {
                        measuredData.Ecomontimestamp_ms = Convert.ToInt64(value);
                    }
                    if (name == "value")
                    {
                        try
                        {
                            measuredData.Value = Convert.ToDecimal(value.Replace('.', ','));
                        }
                        catch
                        {
                            measuredData.Value = Convert.ToDecimal(value.Replace(',', '.'));
                        }
                        DateTime DateTime = new DateTime(1970, 1, 1, 6, 0, 0, 0);
                        measuredData.DateTime = DateTime.AddSeconds((double)measuredData.Ecomontimestamp_ms / 1000);
                        measuredData.EcomonMonitoringPointId = EcomonMonitoringPoint.Id;
                        measuredData.MeasuredParameterId = MeasuredParameter.Id;
                        // save
                        if(MeasuredDatas.Count(m => m.Ecomontimestamp_ms == measuredData.Ecomontimestamp_ms &&
                            m.EcomonMonitoringPointId == measuredData.EcomonMonitoringPointId &&
                            m.MeasuredParameterId == measuredData.MeasuredParameterId) == 0)
                        {
                            measuredDatas.Add(measuredData);
                        }
                        measuredData = new MeasuredData();
                    }
                }
            }
            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
            {
                connection.Open();
                foreach(MeasuredData measuredData in measuredDatas)
                {
                    string execute = $"INSERT INTO public.\"MeasuredData\"(" +
                        $"\"MeasuredParameterId\", " +
                        $"\"DateTime\", " +
                        $"\"Value\", " +
                        $"\"EcomonMonitoringPointId\", " +
                        $"\"Ecomontimestamp_ms\")" +
                        $"VALUES ({measuredData.MeasuredParameterId.ToString()}, " +
                        $"make_timestamptz(" +
                            $"{measuredData.DateTime.Year.ToString()}, " +
                            $"{measuredData.DateTime.Month.ToString()}, " +
                            $"{measuredData.DateTime.Day.ToString()}, " +
                            $"{measuredData.DateTime.Hour.ToString()}, " +
                            $"{measuredData.DateTime.Minute.ToString()}, " +
                            $"{measuredData.DateTime.Second.ToString()}), " +
                        $"{measuredData.Value.ToString()}, " +
                        $"{measuredData.EcomonMonitoringPointId.ToString()}, " +
                        $"{measuredData.Ecomontimestamp_ms.ToString()});";
                    connection.Execute(execute);
                }
            }
        }
    }
}

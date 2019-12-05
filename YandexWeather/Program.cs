using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace YandexWeather
{
    class Program
    {
        public class MonitoringPost
        {
            public int Id { get; set; }
            public string MN { get; set; }
            public decimal NorthLatitude { get; set; }
            public decimal EastLongitude { get; set; }
        }
        public class MeasuredData
        {
            public long Id { get; set; }
            public int MeasuredParameterId { get; set; }
            public DateTime? DateTime { get; set; }
            public decimal? Value { get; set; }
            public int? MonitoringPostId { get; set; }
            public bool? Averaged { get; set; }
            public long? Ecomontimestamp_ms { get; set; }
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? MaxValueMonth { get; set; }
            public int? MaxValueDay { get; set; }
            public decimal? MaxValuePerYear { get; set; }
            public decimal? MaxValuePerMonth { get; set; }
            public int? PollutionSourceId { get; set; }
        }
        static void Main(string[] args)
        {
            int sleepSeconds = 60;
            Console.WriteLine("Program started!");
            while (true)
            {
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                DateTime? dateTime = DateTime.Now;

                if ((new int[] { 00, 20, 36 }).Contains(dateTime.Value.Minute))
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();

                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"MN\", \"NorthLatitude\", \"EastLongitude\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"MN\" <> '' and \"MN\" is not null;");
                        monitoringPosts = monitoringPostsv.ToList();
                    }

                    foreach (var monitoringPost in monitoringPosts)
                    {
                        if (monitoringPost.MN == "kazA2019082001" || monitoringPost.MN == "kazA2019082002" || 
                            monitoringPost.MN == "0032" || monitoringPost.MN == "0033" || monitoringPost.MN == "0035" || 
                            monitoringPost.MN == "0036" || monitoringPost.MN == "0037" || monitoringPost.MN == "0039" || 
                            monitoringPost.MN == "0040" || monitoringPost.MN == "0041")
                        {
                            decimal lat = monitoringPost.NorthLatitude;
                            decimal lon = monitoringPost.EastLongitude;
                            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format($"https://api.weather.yandex.ru/v1/forecast?lat={lat}&lon={lon}"));
                            WebReq.Method = "GET";
                            WebReq.Headers.Add("X-Yandex-API-Key", "8997ff67-00e3-4f0a-8c88-f7b72f90f2c6");
                            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                            string jsonString;
                            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                            {
                                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                                jsonString = reader.ReadToEnd();
                            }

                            dynamic data = JObject.Parse(jsonString);
                            string temp = data.fact.temp;

                            //Console.WriteLine($"Temperature for {monitoringPost.MN}: {temp}");

                            measuredDatas.Add(new MeasuredData()
                            {
                                DateTime = dateTime.Value.AddSeconds(-dateTime.Value.Second),
                                MeasuredParameterId = 21,
                                MonitoringPostId = monitoringPost.Id,
                                Value = Convert.ToDecimal(temp),
                                Averaged = true
                            });

                            //Console.WriteLine($"Post - {monitoringPost.Id}, Date - {dateTime.Value.AddSeconds(-dateTime.Value.Second)}, Temp - {temp}");
                        }
                    }

                    using (var connection2 = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection2.Open();
                        foreach (MeasuredData measuredData in measuredDatas)
                        {
                            string execute = $"INSERT INTO public.\"MeasuredData\"(\"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\", \"Averaged\")" +
                                $"VALUES({measuredData.MeasuredParameterId.ToString()}," +
                                $"make_timestamptz(" +
                                    $"{measuredData.DateTime?.Year.ToString()}, " +
                                    $"{measuredData.DateTime?.Month.ToString()}, " +
                                    $"{measuredData.DateTime?.Day.ToString()}, " +
                                    $"{measuredData.DateTime?.Hour.ToString()}, " +
                                    $"{measuredData.DateTime?.Minute.ToString()}, " +
                                    $"{measuredData.DateTime?.Second.ToString()})," +
                                $"{measuredData.Value.ToString()}," +
                                $"{measuredData.MonitoringPostId.ToString()}," +
                                $"{measuredData.Averaged.ToString()});";
                            connection2.Execute(execute);
                        }
                    }
                }
                //Console.WriteLine($"");

                Thread.Sleep(sleepSeconds * 1000);
            }
        }
    }
}

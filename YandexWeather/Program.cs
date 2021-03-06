﻿using Dapper;
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

                if ((new int[] { 00, 20, 40 }).Contains(dateTime.Value.Minute))
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();

                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"MN\", \"NorthLatitude\", \"EastLongitude\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"MN\" <> '' and \"MN\" is not null;");
                        monitoringPosts = monitoringPostsv.ToList();
                    }

                    Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Yandex started.{Environment.NewLine}");
                    foreach (var monitoringPost in monitoringPosts)
                    {
                        if (monitoringPost.MN == "kazA2019082001" || monitoringPost.MN == "kazA2019082002" || 
                            monitoringPost.MN == "0032" || monitoringPost.MN == "0033" || monitoringPost.MN == "0035" || 
                            monitoringPost.MN == "0036" || monitoringPost.MN == "0037" || monitoringPost.MN == "0039" || 
                            monitoringPost.MN == "0040" || monitoringPost.MN == "0041")
                        {
                            decimal lat = monitoringPost.NorthLatitude;
                            decimal lon = monitoringPost.EastLongitude;
                            try
                            {
                                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format($"https://api.weather.yandex.ru/v1/forecast?lat={lat}&lon={lon}"));
                                WebReq.Method = "GET";
                                WebReq.Headers.Add("X-Yandex-API-Key", "697247f6-ab7e-4e47-a309-efbddc396285");
                                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                                string jsonString;
                                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                                {
                                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                                    jsonString = reader.ReadToEnd();
                                }

                                dynamic data = JObject.Parse(jsonString);
                                string temp = data.fact.temp;
                                string pressure = data.fact.pressure_mm;

                                measuredDatas.Add(new MeasuredData()
                                {
                                    DateTime = dateTime.Value.AddSeconds(-dateTime.Value.Second),
                                    MeasuredParameterId = 21,
                                    MonitoringPostId = monitoringPost.Id,
                                    Value = Convert.ToDecimal(temp),
                                    Averaged = true
                                });
                                measuredDatas.Add(new MeasuredData()
                                {
                                    DateTime = dateTime.Value.AddSeconds(-dateTime.Value.Second),
                                    MeasuredParameterId = 22,
                                    MonitoringPostId = monitoringPost.Id,
                                    Value = Convert.ToDecimal(pressure),
                                    Averaged = true
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString() + " >> Error: " + ex.ToString() + Environment.NewLine);
                            }

                            //Console.WriteLine($"Post - {monitoringPost.Id}, Date - {dateTime.Value.AddSeconds(-dateTime.Value.Second)}, Temp - {temp}");
                        }
                    }
                    Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Yandex finished. Data from Yandex \"Temperature\" count: {measuredDatas.Where(m => m.MeasuredParameterId == 21).Count().ToString()}{Environment.NewLine}");
                    Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Yandex finished. Data from Yandex \"Pressure\" count: {measuredDatas.Where(m => m.MeasuredParameterId == 22).Count().ToString()}{Environment.NewLine}");

                    Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data to MeasuredData started.{Environment.NewLine}");
                    try
                    {
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " >> Error: " + ex.ToString() + Environment.NewLine);
                    }
                    Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data to MeasuredData finished.{Environment.NewLine}");
                }

                Thread.Sleep(sleepSeconds * 1000);
            }
        }
    }
}

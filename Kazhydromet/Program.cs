using Dapper;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Kazhydromet
{
    class Program
    {
        public class MonitoringPost
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int KazhydrometID { get; set; }
            public bool Automatic { get; set; }
        }
        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string KazhydrometCode { get; set; }
            public string NameRU { get; set; }
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
            Console.WriteLine("Program started!");
            DateTime lastInsertAutoPostsTime = new DateTime(2000, 1, 1);
            DateTime lasInsertHandPostsTime = new DateTime(2000, 1, 1);
            while (true)
            {
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                try
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();

                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"Name\", \"KazhydrometID\", \"Automatic\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"DataProviderId\" = 1 and \"ProjectId\" = 3 and \"PollutionEnvironmentId\" = 2");
                        monitoringPosts = monitoringPostsv.ToList();

                        var measuredParametersv = connection.Query<MeasuredParameter>(
                        $"SELECT \"Id\", \"NameRU\", \"KazhydrometCode\"" +
                        $"FROM public.\"MeasuredParameter\" WHERE \"KazhydrometCode\" <> '' and \"KazhydrometCode\" is not null;");
                        measuredParameters = measuredParametersv.ToList();
                    }

                    //Get Data Automatic Posts
                    if ((DateTime.Now - lastInsertAutoPostsTime) > new TimeSpan(0, 1, 0, 0))
                    {
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Kazhydromet Automatic Posts started.{Environment.NewLine}");

                        HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format($"http://93.185.75.19:4003/simple/averages/last?key=21c44891715d8504e1d5de1fabebeb1a796148c34a011d4329553e064771590f"));
                        WebReq.Method = "GET";
                        string jsonString;
                        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                        using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                        {
                            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                            jsonString = reader.ReadToEnd();
                        }
                        dynamic datas = JArray.Parse(jsonString);
                        //var monitoringPostsAuto = monitoringPostsAutoHand.Where(m => m.Type == "A");

                        foreach (dynamic data in datas)
                        {
                            int stationId = data.stationId;
                            string code = data.code;
                            var monitoringPost = monitoringPosts.Where(m => m.KazhydrometID == stationId).ToList();
                            if (monitoringPost.Count != 0)
                            {
                                var measuredParameter = measuredParameters.Where(m => m.KazhydrometCode == code).ToList();
                                if (measuredParameter.Count != 0)
                                {
                                    var value = data.value != null ? Convert.ToDecimal(data.value) : null;
                                    DateTime date = Convert.ToDateTime(data.date);
                                    date = date.AddHours(6);   //Greenwich conversion
                                    measuredDatas.Add(new MeasuredData()
                                    {
                                        DateTime = date,
                                        MeasuredParameterId = measuredParameter[0].Id,
                                        MonitoringPostId = monitoringPost[0].Id,
                                        Value = value,
                                        Averaged = true
                                    });
                                }
                            }
                        }
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Kazhydromet Automatic Posts finished. Data from Kazhydromet count: {measuredDatas.Count().ToString()}{Environment.NewLine}");

                        //Insert Data from Automatic Posts to MeasuredData
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data from Automatic Posts to MeasuredData started.{Environment.NewLine}");
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
                                        $"{measuredData.Value.ToString().Replace(",", ".")}," +
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
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data from Automatic Posts to MeasuredData finished.{Environment.NewLine}");

                        lastInsertAutoPostsTime = DateTime.Now;
                    }

                    //Get Data Hands Posts
                    if ((DateTime.Now - lasInsertHandPostsTime) > new TimeSpan(1, 0, 0, 0))
                    {
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Kazhydromet Hands Posts started.{Environment.NewLine}");

                        measuredDatas.Clear();
                        var monitoringPostHands = monitoringPosts.Where(m => m.Automatic == false).ToList();
                        string dateTime = DateTime.Now.ToString("yyyy-MM-dd");
                        foreach (var monitoringPost in monitoringPostHands)
                        {
                            HttpWebRequest WebReqHand = (HttpWebRequest)WebRequest.Create(string.Format($"http://93.185.75.19:4003/simple/averages?stationNumber={monitoringPost.KazhydrometID}&after={dateTime}&key=be927b4b6fbd304984b72d1456c098860b57ee0692f3d9634a41a230272f73d0"));
                            WebReqHand.Method = "GET";
                            string jsonStringHand;
                            HttpWebResponse WebRespHand = (HttpWebResponse)WebReqHand.GetResponse();
                            using (Stream stream = WebRespHand.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                            {
                                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                                jsonStringHand = reader.ReadToEnd();
                            }
                            dynamic datasHand = JArray.Parse(jsonStringHand);

                            foreach (dynamic data in datasHand)
                            {
                                string code = data.code;
                                var measuredParameter = measuredParameters.Where(m => m.KazhydrometCode == code).ToList();
                                if (measuredParameter.Count != 0)
                                {
                                    var value = data.value != null ? Convert.ToDecimal(data.value) : null;
                                    DateTime date = Convert.ToDateTime(data.date);
                                    date = date.AddHours(6);   //Greenwich conversion
                                    measuredDatas.Add(new MeasuredData()
                                    {
                                        DateTime = date,
                                        MeasuredParameterId = measuredParameter[0].Id,
                                        MonitoringPostId = monitoringPost.Id,
                                        Value = value,
                                        Averaged = true
                                    });
                                }
                            }
                        }
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Kazhydromet Hands Posts finished. Data from Kazhydromet count: {measuredDatas.Count().ToString()}{Environment.NewLine}");

                        //Insert Data from Hands Posts to MeasuredData
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data from Hands Posts to MeasuredData started.{Environment.NewLine}");
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
                                        $"{measuredData.Value.ToString().Replace(",", ".")}," +
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
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Insert Data from Hands Posts to MeasuredData finished.{Environment.NewLine}");

                        lasInsertHandPostsTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " >> Error: " + ex.ToString() + Environment.NewLine);
                }

                Thread.Sleep(60000);
            }
        }
    }
}

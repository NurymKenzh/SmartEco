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
        }
        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string OceanusCode { get; set; }
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
        public class MonitoringPostAutoHand
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public int StationId { get; set; }
        }
        public class MeasuredParameterCode
        {
            public int Id { get; set; }
            public string Code { get; set; }
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
                List<MonitoringPostAutoHand> monitoringPostsAutoHand = new List<MonitoringPostAutoHand>
                {
                    new MonitoringPostAutoHand { Id = 15, Name = "ПНЗ №2", Type = "A", StationId = 25},
                    new MonitoringPostAutoHand { Id = 11, Name = "ПНЗ №29", Type = "A", StationId = 26},
                    new MonitoringPostAutoHand { Id = 13, Name = "ПНЗ №31", Type = "A", StationId = 27},
                    new MonitoringPostAutoHand { Id = 12, Name = "ПНЗ №30", Type = "A", StationId = 28},
                    new MonitoringPostAutoHand { Id = 9, Name = "ПНЗ №27", Type = "A", StationId = 55},
                    new MonitoringPostAutoHand { Id = 14, Name = "ПНЗ №1", Type = "A", StationId = 56},
                    new MonitoringPostAutoHand { Id = 17, Name = "ПНЗ №4", Type = "A", StationId = 57},
                    new MonitoringPostAutoHand { Id = 16, Name = "ПНЗ №3", Type = "A", StationId = 58},
                    new MonitoringPostAutoHand { Id = 19, Name = "ПНЗ №6", Type = "A", StationId = 59},
                    new MonitoringPostAutoHand { Id = 18, Name = "ПНЗ №5", Type = "A", StationId = 60},
                    new MonitoringPostAutoHand { Id = 10, Name = "ПНЗ №28", Type = "A", StationId = 97},
                    new MonitoringPostAutoHand { Id = 4, Name = "ПНЗ №1", Type = "R", StationId = 102},
                    new MonitoringPostAutoHand { Id = 5, Name = "ПНЗ №12", Type = "R", StationId = 103},
                    new MonitoringPostAutoHand { Id = 6, Name = "ПНЗ №16", Type = "R", StationId = 104},
                    new MonitoringPostAutoHand { Id = 7, Name = "ПНЗ №25", Type = "R", StationId = 105},
                    new MonitoringPostAutoHand { Id = 8, Name = "ПНЗ №26", Type = "R", StationId = 106}
                };
                List<MeasuredParameterCode> measuredParametersCode = new List<MeasuredParameterCode>
                {
                    new MeasuredParameterCode { Id = 2, Code = "PM10"},
                    new MeasuredParameterCode { Id = 3, Code = "PM2.5"},
                    new MeasuredParameterCode { Id = 7, Code = "CO"},
                    new MeasuredParameterCode { Id = 13, Code = "NO2"}
                };
                try
                {
                    //using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    //{
                    //    connection.Open();

                    //    var monitoringPostsv = connection.Query<MonitoringPost>(
                    //        $"SELECT \"Id\", \"Name\"" +
                    //        $"FROM public.\"MonitoringPost\" WHERE \"DataProviderId\" = 1 and \"ProjectId\" = 3");
                    //    monitoringPosts = monitoringPostsv.ToList();

                    //    var measuredParametersv = connection.Query<MeasuredParameter>(
                    //    $"SELECT \"Id\", \"NameRU\", \"OceanusCode\"" +
                    //    $"FROM public.\"MeasuredParameter\" WHERE \"OceanusCode\" <> '' and \"OceanusCode\" is not null;");
                    //    measuredParameters = measuredParametersv.ToList();
                    //}

                    //Get Data Automatic Posts
                    if ((DateTime.Now - lastInsertAutoPostsTime) > new TimeSpan(0, 1, 0, 0))
                    {
                        Console.WriteLine($"{DateTime.Now.ToString()} >> Get Data from Kazhydromet Automatic Posts started.{Environment.NewLine}");

                        HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format($"http://93.185.75.19:4003/simple/averages/last?key=be927b4b6fbd304984b72d1456c098860b57ee0692f3d9634a41a230272f73d0"));
                        WebReq.Method = "GET";
                        string jsonString;
                        HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                        using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                        {
                            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                            jsonString = reader.ReadToEnd();
                        }
                        dynamic datas = JArray.Parse(jsonString);
                        var monitoringPostsAuto = monitoringPostsAutoHand.Where(m => m.Type == "A");

                        foreach (dynamic data in datas)
                        {
                            int stationId = data.stationId;
                            string code = data.code;
                            var monitoringPostAuto = monitoringPostsAuto.Where(m => m.StationId == stationId).ToList();
                            if (monitoringPostAuto.Count != 0)
                            {
                                var measuredParameterCode = measuredParametersCode.Where(m => m.Code == code).ToList();
                                if (measuredParameterCode.Count != 0)
                                {
                                    var value = data.value != null ? Convert.ToDecimal(data.value) : null;
                                    measuredDatas.Add(new MeasuredData()
                                    {
                                        DateTime = data.date,
                                        MeasuredParameterId = measuredParameterCode[0].Id,
                                        MonitoringPostId = monitoringPostAuto[0].Id,
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
                        var monitoringPostHands = monitoringPostsAutoHand.Where(m => m.Type == "R").ToList();
                        string dateTime = DateTime.Now.ToString("yyyy-MM-dd");
                        foreach (var monitoringPostHand in monitoringPostHands)
                        {
                            HttpWebRequest WebReqHand = (HttpWebRequest)WebRequest.Create(string.Format($"http://93.185.75.19:4003/simple/averages?stationNumber={monitoringPostHand.StationId}&after={dateTime}&key=be927b4b6fbd304984b72d1456c098860b57ee0692f3d9634a41a230272f73d0"));
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
                                var measuredParameterCode = measuredParametersCode.Where(m => m.Code == code).ToList();
                                if (measuredParameterCode.Count != 0)
                                {
                                    var value = data.value != null ? data.value : null;
                                    measuredDatas.Add(new MeasuredData()
                                    {
                                        DateTime = data.date,
                                        MeasuredParameterId = measuredParameterCode[0].Id,
                                        MonitoringPostId = monitoringPostHand.Id,
                                        Value = Convert.ToDecimal(value),
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

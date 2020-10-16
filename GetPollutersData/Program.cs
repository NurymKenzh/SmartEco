using Dapper;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GetPollutersData
{
    class Program
    {
        const string FromEmail = "smartecokz@gmail.com",
            Password = "Qwerty123_",
            POPServer = "pop.gmail.com";
        const int POPPort = 995;

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
            NewLog("Program started!");
            DateTime lastGetSourceDateTime = new DateTime(2000, 1, 1);
            while (true)
            {
                if ((DateTime.Now - lastGetSourceDateTime) > new TimeSpan(0, 0, 20, 0))
                {
                    List<MeasuredData> measuredDatasPost = new List<MeasuredData>();
                    List<MeasuredData> measuredDatasSourceDB = new List<MeasuredData>();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"PollutionSourceId\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"PollutionSourceId\" = '5' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"", commandTimeout: 86400);
                        measuredDatasSourceDB = measuredDatasv.ToList();

                        var measuredDatasPostv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\", \"Averaged\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"MonitoringPostId\" = '53' AND \"Averaged\" = 'true' AND \"DateTime\" is not null " + //Пост - Школа №10 (Balhash-001)
                            $"ORDER BY \"DateTime\"", commandTimeout: 86400);
                        measuredDatasPost = measuredDatasPostv.ToList();
                    }
                    List<MeasuredData> measuredDatasSource = new List<MeasuredData>();
                    List<int> measuredParametersId = new List<int> { 1, 4, 5, 6, 19 }; //Атм. давл., Темп., Скор. ветра, Напр. ветра, Влажность

                    // Get Datas for Pollution Source
                    NewLog("Get. Get Data for Pollution Source started");
                    if (measuredDatasSourceDB.Count != 0)
                    {
                        // Get Datas from Email
                        var measuredDatasSO2 = measuredDatasSourceDB
                            .Where(m => m.MeasuredParameterId == 9)
                            .LastOrDefault();
                        if (measuredDatasSO2 == null)
                        {
                            measuredDatasSource = ReceiveEmailAsync(null).Result;
                        }
                        else
                        {
                            measuredDatasSource = ReceiveEmailAsync(measuredDatasSO2.DateTime).Result;
                        }

                        // Get Datas from Post
                        foreach (var measuredParameterId in measuredParametersId)
                        {
                            var measuredDataSourceDB = measuredDatasSourceDB
                                .Where(m => m.MeasuredParameterId == measuredParameterId)
                                .LastOrDefault();
                            if (measuredDataSourceDB == null)
                            {
                                measuredDatasSource.AddRange(measuredDatasPost
                                    .Where(m => m.MeasuredParameterId == measuredParameterId)
                                    .Select(m => new MeasuredData
                                    {
                                        MonitoringPostId = null,
                                        PollutionSourceId = 5,
                                        Value = m.Value,
                                        DateTime = m.DateTime,
                                        MeasuredParameterId = m.MeasuredParameterId,
                                        Averaged = m.Averaged
                                    }));
                            }
                            else
                            {
                                measuredDatasSource.AddRange(measuredDatasPost
                                    .Where(m => m.MeasuredParameterId == measuredParameterId && m.DateTime > measuredDataSourceDB.DateTime)
                                    .Select(m => new MeasuredData
                                    {
                                        MonitoringPostId = null,
                                        PollutionSourceId = 5,
                                        Value = m.Value,
                                        DateTime = m.DateTime,
                                        MeasuredParameterId = m.MeasuredParameterId,
                                        Averaged = m.Averaged
                                    }));
                            }
                        }

                        // Get Datas by Formula
                        var measuredDataSourceDBFormula = measuredDatasSourceDB
                                .Where(m => m.MeasuredParameterId == 25)
                                .LastOrDefault();
                        if (measuredDataSourceDBFormula == null)
                        {
                            DateTime? dateTime = new DateTime(2000, 1, 1);
                            do
                            {
                                try
                                {
                                    dateTime = measuredDatasSourceDB.Where(m => m.DateTime > dateTime).FirstOrDefault().DateTime;

                                    var temp = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 4 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var pres = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 1 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var so2 = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var value = GetValueByFormula(temp, pres, so2);

                                    measuredDatasSource.Add(new MeasuredData
                                    {
                                        PollutionSourceId = 5,
                                        Value = value,
                                        DateTime = dateTime,
                                        MeasuredParameterId = 25,
                                        Averaged = true
                                    });
                                }
                                catch
                                {

                                }

                            //} while (measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9).Last().DateTime != dateTime);
                            } while (measuredDatasSourceDB.LastOrDefault().DateTime != dateTime);
                        }
                        else
                        {
                            DateTime? dateTime = measuredDataSourceDBFormula.DateTime;
                            do
                            {
                                try
                                {
                                    dateTime = measuredDatasSourceDB.Where(m => m.DateTime > dateTime).FirstOrDefault().DateTime;

                                    var temp = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 4 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var pres = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 1 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var so2 = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9 && m.DateTime == dateTime).FirstOrDefault().Value;
                                    var value = GetValueByFormula(temp, pres, so2);

                                    measuredDatasSource.Add(new MeasuredData
                                    {
                                        PollutionSourceId = 5,
                                        Value = value,
                                        DateTime = dateTime,
                                        MeasuredParameterId = 25,
                                        Averaged = true
                                    });
                                }
                                catch
                                {

                                }

                            //} while (measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9).Last().DateTime != dateTime);
                            } while (measuredDatasSourceDB.LastOrDefault().DateTime != dateTime);
                        }

                        measuredDatasSource = measuredDatasSource
                            .OrderBy(m => m.DateTime)
                            .ToList();
                    }
                    else
                    {
                        measuredDatasSource = ReceiveEmailAsync(null).Result;

                        foreach (var measuredParameterId in measuredParametersId)
                        {
                            measuredDatasSource.AddRange(measuredDatasPost
                                .Where(m => m.MeasuredParameterId == measuredParameterId)
                                .Select(m => new MeasuredData
                                {
                                    MonitoringPostId = null,
                                    PollutionSourceId = 5,
                                    Value = m.Value,
                                    DateTime = m.DateTime,
                                    MeasuredParameterId = m.MeasuredParameterId,
                                    Averaged = m.Averaged
                                }));
                        }

                        DateTime? dateTime = new DateTime(2000, 1, 1);
                        do
                        {
                            try
                            {
                                dateTime = measuredDatasPost.Where(m => m.DateTime > dateTime).FirstOrDefault().DateTime;

                                var temp = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 4 && m.DateTime == dateTime).FirstOrDefault().Value;
                                var pres = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 1 && m.DateTime == dateTime).FirstOrDefault().Value;
                                var so2 = measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9 && m.DateTime == dateTime).FirstOrDefault().Value;
                                var value = GetValueByFormula(temp, pres, so2);

                                measuredDatasSource.Add(new MeasuredData
                                {
                                    PollutionSourceId = 5,
                                    Value = value,
                                    DateTime = dateTime,
                                    MeasuredParameterId = 25,
                                    Averaged = true
                                });
                            }
                            catch
                            {

                            }

                        //} while (measuredDatasSourceDB.Where(m => m.MeasuredParameterId == 9).Last().DateTime != dateTime);
                        } while (measuredDatasSourceDB.LastOrDefault().DateTime != dateTime);

                        measuredDatasSource = measuredDatasSource
                            .OrderBy(m => m.DateTime)
                            .ToList();
                    }
                    NewLog($"Get. Get Data for Pollution Source finished. Data count: {measuredDatasSource.Count.ToString()}");

                    // Insert MeasuredDatas into SmartEcoAPI
                    NewLog($"Insert data for Pollution Source to MeasuredDatas started");
                    try
                    {
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            foreach (var measuredData in measuredDatasSource)
                            {
                                string execute = $"INSERT INTO public.\"MeasuredData\"(\"MeasuredParameterId\", \"DateTime\", \"Value\", \"PollutionSourceId\", \"Averaged\")" +
                                    $"VALUES({measuredData.MeasuredParameterId.ToString()}," +
                                    $"make_timestamptz(" +
                                        $"{measuredData.DateTime?.Year.ToString()}, " +
                                        $"{measuredData.DateTime?.Month.ToString()}, " +
                                        $"{measuredData.DateTime?.Day.ToString()}, " +
                                        $"{measuredData.DateTime?.Hour.ToString()}, " +
                                        $"{measuredData.DateTime?.Minute.ToString()}, " +
                                        $"{measuredData.DateTime?.Second.ToString()})," +
                                    $"{measuredData.Value.ToString().Replace(",", ".")}," +
                                    $"{measuredData.PollutionSourceId.ToString()}," +
                                    $"{measuredData.Averaged.ToString()});";
                                connection2.Execute(execute);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NewLog($"Error - Insert data for Pollution Source to MeasuredDatas");
                    }
                    NewLog($"Insert data for Pollution Source to MeasuredDatas finished");

                    lastGetSourceDateTime = DateTime.Now;
                }

                Thread.Sleep(30000);
            }
        }

        public static decimal? GetValueByFormula(decimal? t, decimal? P, decimal? K)
        {
            var Vt = 200000; // м3/час
            var V0 = (Vt * 273 * P) / ((273 + t) * 760); // м3/час
            K = K / 1000; // мг/м3 --> г/м3
            V0 = V0 / 3600; // м3/час --> м3/с
            var F = K * V0; // г/с

            return F;
        }

        public static void NewLog(string Log)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} >> {Log}{Environment.NewLine}");
            using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
            {
                connection.Open();
                DateTime now = DateTime.Now;
                string execute = $"INSERT INTO public.\"Log\"(" +
                    $"\"Log\"," +
                    $"\"DateTime\") " +
                    $"VALUES ('{Log}'," +
                    $"make_timestamptz(" +
                        $"{now.Year.ToString()}, " +
                        $"{now.Month.ToString()}, " +
                        $"{now.Day.ToString()}, " +
                        $"{now.Hour.ToString()}, " +
                        $"{now.Minute.ToString()}, " +
                        $"{now.Second.ToString()})" +
                    $");";
                connection.Execute(execute);
                connection.Close();
            }
        }

        public static async Task<List<MeasuredData>> ReceiveEmailAsync(DateTime? dateTime)
        {
            List<MeasuredData> measuredDatas = new List<MeasuredData>();
            try
            {

                using (ImapClient client = new ImapClient())
                {
                    await client.ConnectAsync("imap.gmail.com", 993, true);
                    await client.AuthenticateAsync(FromEmail, Password);
                    IMailFolder inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    dynamic query;
                    if (dateTime == null)
                    {
                        query = SearchQuery.FromContains("balpisystem");
                    }
                    else
                    {
                        query = SearchQuery.DeliveredAfter(DateTime.Parse(dateTime.Value.AddHours(-24).ToString())) //Message sending time
                            .And(SearchQuery.FromContains("balpisystem"));
                    }
                    foreach (var uid in await inbox.SearchAsync(query))
                    {
                        try
                        {
                            List<string> text = new List<string>();
                            var message = await inbox.GetMessageAsync(uid);
                            var html = new HtmlAgilityPack.HtmlDocument();
                            html.LoadHtml(message.HtmlBody);
                            html.DocumentNode.SelectNodes("//span/text()").ToList().ForEach(x => text.Add(x.InnerHtml.Replace("\t", "")));

                            var value = text[text.FindIndex(x => x.Contains("Значение")) + 1];
                            var date = text[text.FindIndex(x => x.Contains("среднего")) + 1];
                            date = date.Substring(0, date.IndexOf("Central") - 1);
                            var dateTimeServer = DateTime.ParseExact(date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                            if (dateTimeServer > dateTime || dateTime == null)
                            {
                                int MeasuredParameterId = 9,
                                    PollutionSourceId = 5;
                                bool Averaged = true;
                                measuredDatas.Add(new MeasuredData
                                {
                                    Value = Convert.ToDecimal(value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)),
                                    DateTime = dateTimeServer,
                                    MeasuredParameterId = MeasuredParameterId,
                                    PollutionSourceId = PollutionSourceId,
                                    Averaged = Averaged
                                });
                            }
                        }
                        catch
                        {

                        }
                    }

                    await client.DisconnectAsync(true);
                }

                return measuredDatas;
            }
            catch (Exception ex)
            {
                return measuredDatas;
            }
        }
    }
}

using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Globalization;

namespace GetPostsData
{
    class Program
    {
        const string Heading = "Нет данных!",
            Theme = "SmartEco",
            FromEmail = "smartecokz@gmail.com",
            //Password = "Qwerty123_",
            Password = "skqjcaiyizgljuak",
            SMTPServer = "smtp.gmail.com";
        const int SMTPPort = 465;

        public enum DataProviders
        {
            Kazhydromet = 1,
            Urus = 2,
            Ecoservice = 3,
            Clarity = 4
        }

        public enum Projects
        {
            KaragandaRegion = 1,
            Arys = 2,
            Almaty = 3,
            Shymkent = 4,
            Altynalmas = 5,
            Zhanatas = 6,
            Oskemen = 7
        }

        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string OceanusCode { get; set; }
            public string NameRU { get; set; }
            public string NameKK { get; set; }
            public decimal? MPCMaxSingle { get; set; }
        }
        public class MonitoringPost
        {
            public int Id { get; set; }
            public string MN { get; set; }
            public int DataProviderId { get; set; }
            public int? ProjectId { get; set; }
        }
        public class PostData
        {
            public long Id { get; set; }
            public string Data { get; set; }
            public DateTime DateTimeServer { get; set; }
            public DateTime? DateTimePost { get; set; }
            public string MN { get; set; }
            public string IP { get; set; }
            public bool? Taken { get; set; }
            public bool? Averaged { get; set; }
        }
        public class PostLog
        {
            public string Log { get; set; }
            public DateTime DateTime { get; set; }
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
        public class Person
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }
        public class LogSendMail
        {
            public DateTime DateTime { get; set; }
            public int MonitoringPostId { get; set; }
            public int MeasuredParameterId { get; set; }
            public string Log { get; set; }
        }
        public class MonitoringPostMeasuredParameter
        {
            public int MonitoringPostId { get; set; }
            public int MeasuredParameterId { get; set; }
            public string Min { get; set; }
            public string Max { get; set; }
            public string MinMeasuredValue { get; set; }
            public string MaxMeasuredValue { get; set; }
            public string Coefficient { get; set; }
        }
        static void Main(string[] args)
        {
            NewLog("Program started!");
            DateTime lastBackupDateTime = new DateTime(2000, 1, 1);
            DateTime lastCheckDateTime = new DateTime(2000, 1, 1);
            DateTime lastWriteFileDateTime = new DateTime(2000, 1, 1);
            DateTime lastGetSourceDateTime = new DateTime(2000, 1, 1);
            DateTime lastSendReportDateTime = new DateTime(2000, 1, 1);
            while (true)
            {
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                List<MonitoringPostMeasuredParameter> monitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameter>();
                List<MeasuredData> measuredDatasCheck = new List<MeasuredData>();
                List<MeasuredData> measuredDatasCheckMinMax = new List<MeasuredData>();
                List<MeasuredData> measuredDatasWriteFile = new List<MeasuredData>();
                List<Person> persons = new List<Person>();

                // Get MeasuredParameters, MonitoringPosts, MonitoringPostMeasuredParameter
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    var measuredParametersv = connection.Query<MeasuredParameter>(
                        $"SELECT \"Id\", \"OceanusCode\"" +
                        $"FROM public.\"MeasuredParameter\" WHERE \"OceanusCode\" <> '' and \"OceanusCode\" is not null;");
                    measuredParameters = measuredParametersv.ToList();

                    var monitoringPostsv = connection.Query<MonitoringPost>(
                        $"SELECT \"Id\", \"MN\"" +
                        $"FROM public.\"MonitoringPost\" WHERE \"MN\" <> '' and \"MN\" is not null;");
                    monitoringPosts = monitoringPostsv.ToList();

                    var monitoringPostMeasuredParametersv = connection.Query<MonitoringPostMeasuredParameter>(
                            $"SELECT \"MonitoringPostId\", \"MeasuredParameterId\", \"Coefficient\", \"MinMeasuredValue\", \"MaxMeasuredValue\"" +
                            $"FROM public.\"MonitoringPostMeasuredParameters\"");
                    monitoringPostMeasuredParameters = monitoringPostMeasuredParametersv.ToList();
                }

                // Copy data
                // Get Data from PostsData
                //NewLog("Get. Get Data from PostsData started");
                int postDatasCount = 0;
                //using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                //{
                //    List<MeasuredData> measuredDatas = new List<MeasuredData>();
                //    connection.Open();
                //    connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = false WHERE \"Taken\" is null;", commandTimeout: 86400);
                //    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\" FROM public.\"Data\" WHERE \"Taken\" = false;", commandTimeout: 86400);
                //    postDatasCount = postDatas.Count();
                //    try
                //    {
                //        foreach (PostData postData in postDatas)
                //        {
                //            foreach (string value in postData.Data.Split(";").Where(d => d.Contains("-Rtd")))
                //            {
                //                int? MeasuredParameterId = measuredParameters.FirstOrDefault(m => m.OceanusCode == value.Split("-Rtd")[0])?.Id,
                //                    MonitoringPostId = monitoringPosts.FirstOrDefault(m => m.MN == postData.MN)?.Id;
                //                var checkPost = monitoringPostMeasuredParameters.FirstOrDefault(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId);
                //                var coef = checkPost != null ? checkPost.Coefficient : null;

                //                if (MeasuredParameterId != null && MonitoringPostId != null)
                //                {
                //                    try
                //                    {
                //                        bool adequateDateTimePost = true;
                //                        if (postData.DateTimePost == null)
                //                        {
                //                            adequateDateTimePost = false;
                //                        }
                //                        else if (Math.Abs((postData.DateTimePost.Value - postData.DateTimeServer).Days) > 3)
                //                        {
                //                            adequateDateTimePost = false;
                //                        }

                //                        var measuredValue = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0]);
                //                        if (checkPost != null && (string.IsNullOrEmpty(checkPost.MaxMeasuredValue) || Convert.ToDecimal(checkPost.MaxMeasuredValue) > measuredValue))
                //                        {
                //                            if (coef != null)
                //                            {
                //                                measuredDatas.Add(new MeasuredData()
                //                                {
                //                                    //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                //                                    DateTime = postData.DateTimeServer,
                //                                    MeasuredParameterId = (int)MeasuredParameterId,
                //                                    MonitoringPostId = (int)MonitoringPostId,
                //                                    Value = measuredValue * Convert.ToDecimal(coef)
                //                                });
                //                            }
                //                            else
                //                            {
                //                                measuredDatas.Add(new MeasuredData()
                //                                {
                //                                    //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                //                                    DateTime = postData.DateTimeServer,
                //                                    MeasuredParameterId = (int)MeasuredParameterId,
                //                                    MonitoringPostId = (int)MonitoringPostId,
                //                                    Value = measuredValue
                //                                });
                //                            }
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {

                //                    }

                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //    NewLog($"Get. Get Data from PostsData finished. Data from PostsData count: {postDatasCount.ToString()}. Data to MeasuredDatas count: {measuredDatas.Count().ToString()}. " +
                //        $"From {measuredDatas.Min(m => m.DateTime).ToString()} to {measuredDatas.Max(m => m.DateTime).ToString()}");
                //    NewLog($"Get. Insert data to MeasuredDatas started");
                //    // Insert MeasuredDatas into SmartEcoAPI
                //    try
                //    {
                //        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                //        {
                //            connection2.Open();
                //            foreach (MeasuredData measuredData in measuredDatas)
                //            {
                //                string execute = $"INSERT INTO public.\"MeasuredData\"(\"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\")" +
                //                    $"VALUES({measuredData.MeasuredParameterId.ToString()}," +
                //                    $"make_timestamptz(" +
                //                        $"{measuredData.DateTime?.Year.ToString()}, " +
                //                        $"{measuredData.DateTime?.Month.ToString()}, " +
                //                        $"{measuredData.DateTime?.Day.ToString()}, " +
                //                        $"{measuredData.DateTime?.Hour.ToString()}, " +
                //                        $"{measuredData.DateTime?.Minute.ToString()}, " +
                //                        $"{measuredData.DateTime?.Second.ToString()})," +
                //                    $"{measuredData.Value.ToString()}," +
                //                    $"{measuredData.MonitoringPostId.ToString()});";
                //                connection2.Execute(execute);
                //            }
                //        }
                //        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                //        {
                //            connection2.Open();
                //            connection2.Execute("UPDATE public.\"Data\" SET \"Taken\" = true WHERE \"Taken\" = false;", commandTimeout: 86400);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                //        {
                //            connection2.Open();
                //            connection2.Execute("UPDATE public.\"Data\" SET \"Taken\" = null WHERE \"Taken\" = false;", commandTimeout: 86400);
                //        }
                //    }
                //    NewLog($"Get. Insert data to MeasuredDatas finished");
                //}
                //=================================================================================================================================================================
                // Average data
                // Get Data from PostsData
                NewLog($"Average. Get Data from PostsData started");
                List<MeasuredData> measuredDatasAveraged = new List<MeasuredData>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    List<MeasuredData> measuredDatas = new List<MeasuredData>();
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = false WHERE \"Averaged\" is null;", commandTimeout: 86400);
                    var postDatas = connection.Query<PostData>("SELECT \"Id\", \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Averaged\" FROM public.\"Data\" WHERE \"Averaged\" = false;");
                    postDatasCount = postDatas.Count();
                    try
                    {
                        foreach (PostData postData in postDatas)
                        {
                            foreach (string value in postData.Data.Split(";").Where(d => d.Contains("-Rtd")))
                            {
                                int? MeasuredParameterId = measuredParameters.FirstOrDefault(m => m.OceanusCode == value.Split("-Rtd")[0])?.Id,
                                    MonitoringPostId = monitoringPosts.FirstOrDefault(m => m.MN == postData.MN)?.Id;
                                var checkPost = monitoringPostMeasuredParameters.FirstOrDefault(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId);
                                var coef = checkPost != null ? checkPost.Coefficient : null;

                                if (MeasuredParameterId != null && MonitoringPostId != null)
                                {
                                    try
                                    {
                                        bool adequateDateTimePost = true;
                                        if (postData.DateTimePost == null)
                                        {
                                            adequateDateTimePost = false;
                                        }
                                        else if (Math.Abs((postData.DateTimePost.Value - postData.DateTimeServer).Days) > 3)
                                        {
                                            adequateDateTimePost = false;
                                        }

                                        var valueArray = value.Split("-Rtd=");
                                        if (valueArray.Length < 2 || string.IsNullOrEmpty(valueArray[1]))
                                            continue;

                                        if (!decimal.TryParse(valueArray[1].Split("&&")[0], out decimal measuredValue))
                                            continue;

                                        if (checkPost != null && (string.IsNullOrEmpty(checkPost.MaxMeasuredValue) || Convert.ToDecimal(checkPost.MaxMeasuredValue) > measuredValue))
                                        {
                                            if (coef != null)
                                            {
                                                measuredDatas.Add(new MeasuredData()
                                                {
                                                    Id = postData.Id,
                                                    //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                                    DateTime = postData.DateTimeServer,
                                                    MeasuredParameterId = (int)MeasuredParameterId,
                                                    MonitoringPostId = (int)MonitoringPostId,
                                                    Value = measuredValue * Convert.ToDecimal(coef)
                                                });
                                            }
                                            else
                                            {
                                                measuredDatas.Add(new MeasuredData()
                                                {
                                                    Id = postData.Id,
                                                    //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                                    DateTime = postData.DateTimeServer,
                                                    MeasuredParameterId = (int)MeasuredParameterId,
                                                    MonitoringPostId = (int)MonitoringPostId,
                                                    Value = measuredValue
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    NewLog($"Average. Get Data from PostsData finished. Data from PostsData count: {postDatasCount.ToString()}. Data to MeasuredDatas count: {measuredDatas.Count().ToString()}. " +
                    $"From {measuredDatas.Min(m => m.DateTime).ToString()} to {measuredDatas.Max(m => m.DateTime).ToString()}");
                    NewLog($"Average. Average data started");
                    // Average data
                    List<long> averagedPostsDatas = new List<long>();
                    if (measuredDatas.Count() > 0)
                    {
                        DateTime? dateTimeMin = measuredDatas.Min(m => m.DateTime),
                        dateTimeMax = measuredDatas.Max(m => m.DateTime);
                        dateTimeMax = dateTimeMax.Value.AddSeconds(-dateTimeMax.Value.Second);
                        while (!(new int[] { 0, 20, 40 }).Contains(dateTimeMax.Value.Minute))
                        {
                            dateTimeMax = dateTimeMax.Value.AddMinutes(-1);
                        }
                        dateTimeMin = dateTimeMin.Value.AddSeconds(-dateTimeMin.Value.Second);
                        while (!(new int[] { 0, 20, 40 }).Contains(dateTimeMin.Value.Minute))
                        {
                            dateTimeMin = dateTimeMin.Value.AddMinutes(-1);
                        }
                        for (DateTime dateTimeStart = dateTimeMin.Value; dateTimeStart < dateTimeMax.Value; dateTimeStart = dateTimeStart.AddMinutes(20))
                        {
                            DateTime dateTimeFinish = dateTimeStart.AddMinutes(20);
                            List<MeasuredData> measuredDatasCurrentAll = measuredDatas.Where(m => m.DateTime.Value > dateTimeStart && m.DateTime.Value <= dateTimeFinish).ToList();
                            foreach (MonitoringPost monitoringPost in monitoringPosts)
                            {
                                foreach (MeasuredParameter measuredParameter in measuredParameters)
                                {
                                    List<MeasuredData> measuredDatasCurrent = measuredDatasCurrentAll
                                        .Where(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == measuredParameter.Id)
                                        .ToList();
                                    if (measuredDatasCurrent.Count() > 0)
                                    {
                                        measuredDatasAveraged.Add(new MeasuredData()
                                        {
                                            DateTime = dateTimeFinish,
                                            MeasuredParameterId = measuredParameter.Id,
                                            MonitoringPostId = monitoringPost.Id,
                                            Value = measuredDatasCurrent.Average(m => m.Value),
                                            Averaged = true
                                        });
                                        averagedPostsDatas.AddRange(measuredDatasCurrent.Select(m => m.Id));
                                    }
                                }
                            }
                        }
                    }
                    averagedPostsDatas = averagedPostsDatas.Distinct().ToList();
                    NewLog($"Average. Average data finished. Average data count: {measuredDatasAveraged.Count().ToString()}. " +
                        $"From {measuredDatasAveraged.Min(m => m.DateTime).ToString()} to {measuredDatasAveraged.Max(m => m.DateTime).ToString()}.");
                    NewLog($"Average. Insert data to MeasuredDatas started");
                    // Insert measuredDatasAveraged into SmartEcoAPI
                    try
                    {
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            foreach (MeasuredData measuredData in measuredDatasAveraged)
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
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            //connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = true WHERE \"Averaged\" = false;");
                            foreach (long id in averagedPostsDatas)
                            {
                                connection2.Execute($"UPDATE public.\"Data\" SET \"Averaged\" = true WHERE \"Id\" = {id.ToString()};");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            connection2.Execute("UPDATE public.\"Data\" SET \"Averaged\" = null WHERE \"Averaged\" = false;", commandTimeout: 86400);
                        }
                    }
                    NewLog($"Average. Insert data to MeasuredDatas finished");
                }
                //=================================================================================================================================================================
                // Insert data to inactive posts
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                {
                    List<MeasuredData> measuredDatasDB = new List<MeasuredData>();
                    List<MeasuredData> measuredDatas = new List<MeasuredData>();
                    connection.Open();
                    var measuredDatasv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\", \"Averaged\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" > '{DateTime.Now.AddMinutes(-20).ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null AND " +
                            $"\"MonitoringPostId\" IN ('184', '185', '168', '169', '193', '194') AND \"Averaged\" = 'true' " +
                            $"ORDER BY \"DateTime\"", commandTimeout: 86400);
                    measuredDatasDB = measuredDatasv.ToList();

                    //Check post Balhash-002
                    if (measuredDatasDB.Where(m => m.MonitoringPostId == 184).Count() == 0)
                    {
                        //Copy data from Balhash-003
                        if (measuredDatasDB.Where(m => m.MonitoringPostId == 185).Count() != 0)
                        {
                            measuredDatas.AddRange(measuredDatasDB
                                .Where(m => m.MonitoringPostId == 185)
                                .Select(m => new MeasuredData
                                {
                                    MeasuredParameterId = m.MeasuredParameterId,
                                    DateTime = m.DateTime,
                                    Value = m.Value,
                                    MonitoringPostId = 184,
                                    Averaged = m.Averaged
                                }));
                        }
                    }
                    //Check post Temir-009
                    if (measuredDatasDB.Where(m => m.MonitoringPostId == 193).Count() == 0)
                    {
                        //Copy data from Temir-007
                        if (measuredDatasDB.Where(m => m.MonitoringPostId == 168).Count() != 0)
                        {
                            measuredDatas.AddRange(measuredDatasDB
                                .Where(m => m.MonitoringPostId == 168)
                                .Select(m => new MeasuredData
                                {
                                    MeasuredParameterId = m.MeasuredParameterId,
                                    DateTime = m.DateTime,
                                    Value = m.Value,
                                    MonitoringPostId = 193,
                                    Averaged = m.Averaged
                                }));
                        }
                    }
                    //Check post Temir-010
                    if (measuredDatasDB.Where(m => m.MonitoringPostId == 194).Count() == 0)
                    {
                        //Copy data from Temir-008
                        if (measuredDatasDB.Where(m => m.MonitoringPostId == 169).Count() != 0)
                        {
                            measuredDatas.AddRange(measuredDatasDB
                                .Where(m => m.MonitoringPostId == 169)
                                .Select(m => new MeasuredData
                                {
                                    MeasuredParameterId = m.MeasuredParameterId,
                                    DateTime = m.DateTime,
                                    Value = m.Value,
                                    MonitoringPostId = 194,
                                    Averaged = m.Averaged
                                }));
                        }
                    }

                    foreach (MeasuredData measuredData in measuredDatas)
                    {
                        string avg = measuredData.Averaged == null ? "NULL" : measuredData.Averaged.ToString();
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
                            $"{avg});";
                        connection.Execute(execute);
                    }
                    connection.Close();
                }
                //=================================================================================================================================================================
                // Insert "Lead" to Shymkent posts
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                {
                    List<MeasuredData> measuredDatasDB = new List<MeasuredData>();
                    List<MeasuredData> measuredDatas = new List<MeasuredData>();
                    connection.Open();

                    //Get posts that selected "Lead"
                    var MPMPs = connection.Query<MonitoringPostMeasuredParameter>($"SELECT * " +
                            $"FROM public.\"MonitoringPostMeasuredParameters\" as mpmp " +
                            $"INNER JOIN public.\"MonitoringPost\" as post ON mpmp.\"MonitoringPostId\" = post.\"Id\" " +
                            $"INNER JOIN public.\"MeasuredParameter\" as param ON mpmp.\"MeasuredParameterId\" = param.\"Id\" " +
                            $"WHERE post.\"Name\" LIKE 'Shym%' AND param.\"Id\" = 74", commandTimeout: 86400) // selected "Lead" - 74 id
                        .ToList();

                    //Get last data posts that selected "Lead" (data by "Lead")
                    var measuredDataLead = connection.Query<MeasuredData>($"SELECT max(md.\"DateTime\") as \"DateTime\", md.\"MonitoringPostId\" " +
                            $"FROM public.\"MeasuredData\" as md " +
                            $"WHERE md.\"MonitoringPostId\" IN ({string.Join(", ", MPMPs.Select(m => m.MonitoringPostId))}) AND md.\"MeasuredParameterId\" = 74 AND md.\"Averaged\" = 'true' " +
                            $"GROUP BY md.\"MonitoringPostId\"", commandTimeout: 86400)
                        .ToList();

                    var dateTime = measuredDataLead.Count != 0 ? (DateTime)measuredDataLead.Min(m => m.DateTime) : DateTime.Now.AddMinutes(-20);
                    //Get last data posts
                    var measuredDatasv = connection.Query<MeasuredData>($"SELECT md.\"Id\", md.\"MeasuredParameterId\", md.\"DateTime\", md.\"Value\", md.\"MonitoringPostId\", md.\"Averaged\" " +
                            $"FROM public.\"MeasuredData\" as md " +
                            $"WHERE md.\"DateTime\" > '{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}' AND md.\"DateTime\" is not null AND " +
                            $"md.\"MonitoringPostId\" IN ({string.Join(", ", MPMPs.Select(m => m.MonitoringPostId))}) AND md.\"Averaged\" = 'true' " +
                            $"ORDER BY md.\"DateTime\"", commandTimeout: 86400);
                    measuredDatasDB = measuredDatasv.ToList();

                    foreach (var MPMP in MPMPs)
                    {
                        if (measuredDatasDB.Where(m => m.MonitoringPostId == MPMP.MonitoringPostId && m.MeasuredParameterId == 74 && m.DateTime > DateTime.Now.AddMinutes(-20)).FirstOrDefault() is null) //check "Lead" for the last datetime
                        {
                            if (measuredDatasDB.Where(m => m.MonitoringPostId == MPMP.MonitoringPostId && m.DateTime > DateTime.Now.AddMinutes(-20)).FirstOrDefault() != null) //checking whether the post is working
                            {
                                //Check if there is any data on the post in "Lead"
                                var measuredDataPost = measuredDatasDB.Where(m => m.MonitoringPostId == MPMP.MonitoringPostId && m.MeasuredParameterId == 74).FirstOrDefault();
                                if (measuredDataPost is null)
                                {
                                    measuredDatas.Add(new MeasuredData()
                                    {
                                        MeasuredParameterId = 74,
                                        DateTime = measuredDatasDB.FirstOrDefault().DateTime,
                                        Value = 0m,
                                        MonitoringPostId = MPMP.MonitoringPostId,
                                        Averaged = true
                                    });
                                }
                                else
                                {
                                    var dates = measuredDatasDB
                                        .Where(m => m.DateTime > measuredDataPost.DateTime)
                                        .Select(m => m.DateTime)
                                        .Distinct()
                                        .ToList();

                                    measuredDatas.AddRange(dates
                                        .Select(date => new MeasuredData
                                        {
                                            MeasuredParameterId = 74,
                                            DateTime = date,
                                            Value = 0m,
                                            MonitoringPostId = MPMP.MonitoringPostId,
                                            Averaged = true
                                        }));
                                }
                            }
                        }
                    }

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
                            $"{measuredData.Value}," +
                            $"{measuredData.MonitoringPostId}," +
                            $"{measuredData.Averaged});";
                        connection.Execute(execute);
                    }
                    connection.Close();
                }
                //=================================================================================================================================================================
                // Backup data
                long countBackup = 0;
                if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                {
                    NewLog($"Backup. Get Data from MeasuredData (SmartEcoAPI) started");
                    DateTime dateTimeLast = DateTime.Now.AddDays(-14);
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasB = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"Ecomontimestamp_ms\", \"MaxValueDay\", \"MaxValueMonth\", \"Month\", \"Year\", \"MaxValuePerMonth\", \"MaxValuePerYear\", \"MonitoringPostId\", \"PollutionSourceId\", \"Averaged\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null " +
                            $"AND \"MonitoringPostId\" <> 234 AND \"MonitoringPostId\" <> 235;", commandTimeout: 86400); //exclude Zhanatas posts
                        countBackup = measuredDatasB.Count();
                        foreach (MeasuredData measuredData in measuredDatasB)
                        {
                            string fileName = Path.Combine(@"C:\Users\Administrator\source\repos\Backup", $"SmartEcoAPI_MeasuredData {measuredData.DateTime?.ToString("yyyy-MM")}");
                            fileName = Path.ChangeExtension(fileName, "csv");
                            string data = measuredData.MeasuredParameterId.ToString() + "\t" +
                                measuredData.DateTime?.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                measuredData.Value?.ToString() + "\t" +
                                measuredData.Ecomontimestamp_ms?.ToString() + "\t" +
                                measuredData.MaxValueDay?.ToString() + "\t" +
                                measuredData.MaxValueMonth?.ToString() + "\t" +
                                measuredData.Month?.ToString() + "\t" +
                                measuredData.Year?.ToString() + "\t" +
                                measuredData.MaxValuePerMonth?.ToString() + "\t" +
                                measuredData.MaxValuePerYear?.ToString() + "\t" +
                                measuredData.MonitoringPostId?.ToString() + "\t" +
                                measuredData.PollutionSourceId?.ToString() + "\t" +
                                measuredData.Averaged?.ToString() + Environment.NewLine;
                            if (!File.Exists(fileName))
                            {
                                File.AppendAllText(fileName, $"MeasuredParameterId\tDateTime\tValue\tEcomontimestamp_ms\tMaxValueDay\tMaxValueMonth\t" +
                                    $"Month\tYear\tMaxValuePerMonth\tMaxValuePerYear\tMonitoringPostId\tPollutionSourceId\tAveraged" + Environment.NewLine);
                            }
                            File.AppendAllText(fileName, data);
                        }
                        try
                        {
                            connection.Execute($"DELETE FROM public.\"MeasuredData\" " +
                                $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null " +
                                $"AND \"MonitoringPostId\" <> 234 AND \"MonitoringPostId\" <> 235;", commandTimeout: 86400); //exclude Zhanatas posts
                        }
                        catch
                        {

                        }
                        finally
                        {
                            connection.Execute($"VACUUM public.\"MeasuredData\"", commandTimeout: 86400);
                        }
                    }
                    NewLog($"Backup. Get Data from MeasuredData (SmartEcoAPI) finished. {countBackup.ToString()} rows backed up");

                    NewLog($"Backup. Get Data from Data (PostsData) started");
                    countBackup = 0;
                    if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                    {
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                        {
                            connection.Open();
                            var postDatas = connection.Query<PostData>($"SELECT \"Id\", \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\", \"Averaged\" " +
                                $"FROM public.\"Data\" " +
                                $"WHERE \"DateTimeServer\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                            countBackup = postDatas.Count();
                            foreach (PostData postData in postDatas)
                            {
                                string fileName = Path.Combine(@"C:\Users\Administrator\source\repos\Backup", $"PostsData_Data {postData.DateTimeServer.ToString("yyyy-MM")}");
                                fileName = Path.ChangeExtension(fileName, "csv");
                                string data = postData.Data.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "") + "\t" +
                                    postData.DateTimeServer.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                    postData.DateTimePost?.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                    postData.MN + "\t" +
                                    postData.IP + "\t" +
                                    postData.Taken?.ToString() + "\t" +
                                    postData.Averaged?.ToString() + Environment.NewLine;
                                if (!File.Exists(fileName))
                                {
                                    string s = $"Data\tDateTimeServer\tDateTimePost\tMN\tIP\tTaken\tAveraged";
                                    File.AppendAllText(fileName, $"Data\tDateTimeServer\tDateTimePost\tMN\tIP\tTaken\tAveraged" + Environment.NewLine);
                                }
                                File.AppendAllText(fileName, data);
                            }
                            try
                            {
                                connection.Execute($"DELETE FROM public.\"Data\" " +
                                    $"WHERE \"DateTimeServer\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                            }
                            catch
                            {

                            }
                            finally
                            {
                                connection.Execute($"VACUUM public.\"Data\"", commandTimeout: 86400);
                            }
                        }
                    }
                    NewLog($"Backup. Get Data from Data (PostsData) finished. {countBackup.ToString()} rows backed up");

                    //NewLog($"Backup. Get Data from Log (PostsData) started");
                    //countBackup = 0;
                    //if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                    //{
                    //    using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                    //    {
                    //        connection.Open();
                    //        var postLogs = connection.Query<PostLog>($"SELECT \"Log\", \"DateTime\" " +
                    //            $"FROM public.\"Log\" " +
                    //            $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                    //        countBackup = postLogs.Count();
                    //        foreach (PostLog log in postLogs)
                    //        {
                    //            string fileName = Path.Combine(@"C:\Users\Administrator\source\repos\Backup", $"PostsData_Log {log.DateTime.ToString("yyyy-MM")}");
                    //            fileName = Path.ChangeExtension(fileName, "csv");
                    //            string data = log.Log.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "") + "\t" +
                    //                log.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                    //            if (!File.Exists(fileName))
                    //            {
                    //                File.AppendAllText(fileName, $"Log\tDateTime" + Environment.NewLine);
                    //            }
                    //            File.AppendAllText(fileName, data);
                    //        }
                    //        try
                    //        {
                    //            connection.Execute($"DELETE FROM public.\"Log\" " +
                    //                $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                    //        }
                    //        catch
                    //        {

                    //        }
                    //        finally
                    //        {
                    //            connection.Execute($"VACUUM public.\"Log\"", commandTimeout: 86400);
                    //        }
                    //    }
                    //}
                    //NewLog($"Backup. Get Data from Log (PostsData) finished. {countBackup.ToString()} rows backed up");

                    NewLog($"Backup. Get Data from Log (GetPostsData) started");
                    countBackup = 0;
                    if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                    {
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                        {
                            connection.Open();
                            var postLogs = connection.Query<PostLog>($"SELECT \"Log\", \"DateTime\" " +
                                $"FROM public.\"Log\" " +
                                $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                            countBackup = postLogs.Count();
                            foreach (PostLog log in postLogs)
                            {
                                string fileName = Path.Combine(@"C:\Users\Administrator\source\repos\Backup", $"PostsData_Log {log.DateTime.ToString("yyyy-MM")}");
                                fileName = Path.ChangeExtension(fileName, "csv");
                                string data = log.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                    log.Log + Environment.NewLine;
                                if (!File.Exists(fileName))
                                {
                                    File.AppendAllText(fileName, $"DateTime\tLog" + Environment.NewLine);
                                }
                                File.AppendAllText(fileName, data);
                            }
                            try
                            {
                                connection.Execute($"DELETE FROM public.\"Log\" " +
                                    $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}';", commandTimeout: 86400);
                            }
                            catch
                            {

                            }
                            finally
                            {
                                connection.Execute($"VACUUM public.\"Log\"", commandTimeout: 86400);
                            }
                        }
                    }
                    NewLog($"Backup. Get Data from Log (GetPostsData) finished. {countBackup.ToString()} rows backed up");


                    //Backup Zhanatas data
                    NewLog($"Backup. Get Data Zhanatas posts from MeasuredData (SmartEcoAPI) started");
                    DateTime dateTimeZhanatasLast = DateTime.Now.AddMonths(-6);
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasB = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"Ecomontimestamp_ms\", \"MaxValueDay\", \"MaxValueMonth\", \"Month\", \"Year\", \"MaxValuePerMonth\", \"MaxValuePerYear\", \"MonitoringPostId\", \"PollutionSourceId\", \"Averaged\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" < '{dateTimeZhanatasLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null " +
                            $"AND (\"MonitoringPostId\" = 234 OR \"MonitoringPostId\" = 235);", commandTimeout: 86400); //Zhanatas posts
                        countBackup = measuredDatasB.Count();
                        foreach (MeasuredData measuredData in measuredDatasB)
                        {
                            string fileName = Path.Combine(@"C:\Users\Administrator\source\repos\Backup", $"SmartEcoAPI_MeasuredData_Zhanatas {measuredData.DateTime?.ToString("yyyy-MM")}");
                            fileName = Path.ChangeExtension(fileName, "csv");
                            string data = measuredData.MeasuredParameterId.ToString() + "\t" +
                                measuredData.DateTime?.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                measuredData.Value?.ToString() + "\t" +
                                measuredData.Ecomontimestamp_ms?.ToString() + "\t" +
                                measuredData.MaxValueDay?.ToString() + "\t" +
                                measuredData.MaxValueMonth?.ToString() + "\t" +
                                measuredData.Month?.ToString() + "\t" +
                                measuredData.Year?.ToString() + "\t" +
                                measuredData.MaxValuePerMonth?.ToString() + "\t" +
                                measuredData.MaxValuePerYear?.ToString() + "\t" +
                                measuredData.MonitoringPostId?.ToString() + "\t" +
                                measuredData.PollutionSourceId?.ToString() + "\t" +
                                measuredData.Averaged?.ToString() + Environment.NewLine;
                            if (!File.Exists(fileName))
                            {
                                File.AppendAllText(fileName, $"MeasuredParameterId\tDateTime\tValue\tEcomontimestamp_ms\tMaxValueDay\tMaxValueMonth\t" +
                                    $"Month\tYear\tMaxValuePerMonth\tMaxValuePerYear\tMonitoringPostId\tPollutionSourceId\tAveraged" + Environment.NewLine);
                            }
                            File.AppendAllText(fileName, data);
                        }
                        try
                        {
                            connection.Execute($"DELETE FROM public.\"MeasuredData\" " +
                                $"WHERE \"DateTime\" < '{dateTimeZhanatasLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null " +
                                $"AND (\"MonitoringPostId\" = 234 OR \"MonitoringPostId\" = 235);", commandTimeout: 86400); //Zhanatas posts
                        }
                        catch
                        {

                        }
                        finally
                        {
                            connection.Execute($"VACUUM public.\"MeasuredData\"", commandTimeout: 86400);
                        }
                    }
                    NewLog($"Backup. Get Data Zhanatas posts from MeasuredData (SmartEcoAPI) finished. {countBackup.ToString()} rows backed up");

                    lastBackupDateTime = DateTime.Now;
                }
                //=================================================================================================================================================================
                // Check data
                if ((DateTime.Now - lastCheckDateTime) > new TimeSpan(0, 1, 0, 0))
                {
                    List<LogSendMail> logSendMails = new List<LogSendMail>();
                    DateTime dateTimeLast = DateTime.Now.AddMinutes(-60);
                    DateTime dateTimeLastMinMax = DateTime.Now.AddMinutes(-60);
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" > '{dateTimeLast.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"", commandTimeout: 86400);
                        measuredDatasCheck = measuredDatasv.ToList();

                        var measuredDatasMinMaxv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" > '{dateTimeLastMinMax.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"", commandTimeout: 86400);
                        measuredDatasCheckMinMax = measuredDatasMinMaxv.ToList();

                        var measuredParametersv = connection.Query<MeasuredParameter>(
                            $"SELECT \"Id\", \"OceanusCode\", \"NameRU\"" +
                            $"FROM public.\"MeasuredParameter\" WHERE \"OceanusCode\" <> '' and \"OceanusCode\" is not null;");
                        measuredParameters = measuredParametersv.ToList();

                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"MN\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"MN\" <> '' and \"MN\" is not null;");
                        monitoringPosts = monitoringPostsv.ToList();

                        var monitoringPostMeasuredParametersv = connection.Query<MonitoringPostMeasuredParameter>(
                            $"SELECT \"MonitoringPostId\", \"MeasuredParameterId\", \"Min\", \"Max\"" +
                            $"FROM public.\"MonitoringPostMeasuredParameters\"");
                        monitoringPostMeasuredParameters = monitoringPostMeasuredParametersv.ToList();

                        var personsv = connection.Query<Person>($"SELECT \"Id\", \"Email\" " +
                            $"FROM public.\"Person\" " +
                            $"WHERE \"Role\" = 'admin' OR \"Role\" = 'moderator' " +
                            $"ORDER BY \"Id\"");
                        persons = personsv
                            .Where(p => p.Email != "rkostylev@ecoservice.kz" &&
                                p.Email != "biskakov@ecoservice.kz" &&
                                p.Email != "n.a.k@bk.ru" &&
                                p.Email != "testuser@smarteco.com")
                            .ToList();
                    }
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\" " +
                            $"FROM public.\"LogSendMail\" " +
                            $"WHERE \"DateTime\" > '{dateTimeLastWrite.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"");
                        logSendMails = logSendMailsv.ToList();
                    }

                    bool check = true,
                        checkPost = true,
                        checkMin = true,
                        checkMax = true,
                        checkLogSendMail = true;
                    string message = "";
                    if (measuredDatasCheck.Count == 0)
                    {
                        if (logSendMails.Count != 0)
                        {
                            foreach (var logSendMail in logSendMails)
                            {
                                if (logSendMail.MonitoringPostId == 0 && logSendMail.MeasuredParameterId == 0)
                                {
                                    checkLogSendMail = false;
                                    break;
                                }
                            }
                            if (checkLogSendMail)
                            {
                                message = "Нет данных по всем постам!";
                                Task.WaitAll(SendEmailAsync(persons, Heading, message));
                                NewLogSendMail(null, null, message);
                            }
                        }
                        else
                        {
                            message = "Нет данных по всем постам!";
                            Task.WaitAll(SendEmailAsync(persons, Heading, message));
                            NewLogSendMail(null, null, message);
                        }
                    }
                    else
                    {
                        foreach (var monitoringPost in monitoringPosts)
                        {
                            foreach (var measuredParameter in measuredParameters)
                            {
                                var monitoringPostMeasuredParameter = monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == measuredParameter.Id).ToList();
                                var min = (monitoringPostMeasuredParameter.Count != 0) ? monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == measuredParameter.Id).FirstOrDefault().Min : null;
                                var max = (monitoringPostMeasuredParameter.Count != 0) ? monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == monitoringPost.Id && m.MeasuredParameterId == measuredParameter.Id).FirstOrDefault().Max : null;
                                foreach (var measuredData in measuredDatasCheck)
                                {
                                    if (measuredData.MonitoringPostId == monitoringPost.Id)
                                    {
                                        checkPost = false;
                                    }
                                    if (monitoringPostMeasuredParameter.Count != 0)
                                    {
                                        if (measuredData.MonitoringPostId == monitoringPost.Id && measuredData.MeasuredParameterId == measuredParameter.Id)
                                        {
                                            check = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        check = false;
                                    }
                                }
                                foreach (var measuredData in measuredDatasCheckMinMax)
                                {
                                    if (monitoringPostMeasuredParameter.Count != 0)
                                    {
                                        if (measuredData.MonitoringPostId == monitoringPost.Id && measuredData.MeasuredParameterId == measuredParameter.Id)
                                        {
                                            if (min != null)
                                            {
                                                var minValue = Convert.ToDecimal(min);
                                                if (measuredData.Value < minValue)
                                                {
                                                    checkMin = false;
                                                }
                                            }
                                            if (max != null)
                                            {
                                                var maxValue = Convert.ToDecimal(max);
                                                if (measuredData.Value > maxValue)
                                                {
                                                    checkMax = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (checkPost)
                                {
                                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                                    {
                                        connection.Open();
                                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\" " +
                                            $"FROM public.\"LogSendMail\" " +
                                            $"WHERE \"DateTime\" > '{dateTimeLastWrite.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                                            $"ORDER BY \"DateTime\"");
                                        logSendMails = logSendMailsv.ToList();
                                    }
                                    if (logSendMails.Count != 0)
                                    {
                                        foreach (var logSendMail in logSendMails)
                                        {
                                            if (logSendMail.MonitoringPostId == monitoringPost.Id && logSendMail.MeasuredParameterId == 0)
                                            {
                                                checkLogSendMail = false;
                                                break;
                                            }
                                        }
                                        if (checkLogSendMail)
                                        {
                                            message += $"Нет данных по посту {monitoringPost.MN} <br/>";
                                            string logText = $"Нет данных по посту {monitoringPost.MN}";
                                            NewLogSendMail(monitoringPost.Id, null, logText);
                                        }
                                    }
                                    else
                                    {
                                        message += $"Нет данных по посту {monitoringPost.MN} <br/>";
                                        string logText = $"Нет данных по посту {monitoringPost.MN}";
                                        NewLogSendMail(monitoringPost.Id, null, logText);
                                    }
                                    checkLogSendMail = true;
                                }
                                else if (check)
                                {
                                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                                    {
                                        connection.Open();
                                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\" " +
                                            $"FROM public.\"LogSendMail\" " +
                                            $"WHERE \"DateTime\" > '{dateTimeLastWrite.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                                            $"ORDER BY \"DateTime\"");
                                        logSendMails = logSendMailsv.ToList();
                                    }
                                    if (logSendMails.Count != 0)
                                    {
                                        foreach (var logSendMail in logSendMails)
                                        {
                                            if (logSendMail.MonitoringPostId == monitoringPost.Id && logSendMail.MeasuredParameterId == measuredParameter.Id)
                                            {
                                                checkLogSendMail = false;
                                                break;
                                            }
                                        }
                                        if (checkLogSendMail)
                                        {
                                            message += $"Нет данных по \"{measuredParameter.NameRU}\" по посту {monitoringPost.MN} <br/>";
                                            string logText = $"Нет данных по \"{measuredParameter.NameRU}\" по посту {monitoringPost.MN}";
                                            NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                        }
                                    }
                                    else
                                    {
                                        message += $"Нет данных по \"{measuredParameter.NameRU}\" по посту {monitoringPost.MN} <br/>";
                                        string logText = $"Нет данных по \"{measuredParameter.NameRU}\" по посту {monitoringPost.MN}";
                                        NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                    }
                                    checkLogSendMail = true;
                                }
                                if (!checkMin)
                                {
                                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                                    {
                                        connection.Open();
                                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\", \"Log\"" +
                                            $"FROM public.\"LogSendMail\" " +
                                            $"WHERE \"DateTime\" > '{dateTimeLastWrite.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                                            $"ORDER BY \"DateTime\"");
                                        logSendMails = logSendMailsv.ToList();
                                    }
                                    if (logSendMails.Count != 0)
                                    {
                                        foreach (var logSendMail in logSendMails)
                                        {
                                            if (logSendMail.MonitoringPostId == monitoringPost.Id && logSendMail.MeasuredParameterId == measuredParameter.Id && logSendMail.Log.Contains("меньше минимума"))
                                            {
                                                checkLogSendMail = false;
                                                break;
                                            }
                                        }
                                        if (checkLogSendMail)
                                        {
                                            message += $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение меньше минимума <br/>";
                                            string logText = $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение меньше минимума";
                                            NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                        }
                                    }
                                    else
                                    {
                                        message += $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение меньше минимума <br/>";
                                        string logText = $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение меньше минимума";
                                        NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                    }
                                    checkLogSendMail = true;
                                }
                                if (!checkMax)
                                {
                                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                                    {
                                        connection.Open();
                                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\", \"Log\"" +
                                            $"FROM public.\"LogSendMail\" " +
                                            $"WHERE \"DateTime\" > '{dateTimeLastWrite.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                                            $"ORDER BY \"DateTime\"");
                                        logSendMails = logSendMailsv.ToList();
                                    }
                                    if (logSendMails.Count != 0)
                                    {
                                        foreach (var logSendMail in logSendMails)
                                        {
                                            if (logSendMail.MonitoringPostId == monitoringPost.Id && logSendMail.MeasuredParameterId == measuredParameter.Id && logSendMail.Log.Contains("больше максимума"))
                                            {
                                                checkLogSendMail = false;
                                                break;
                                            }
                                        }
                                        if (checkLogSendMail)
                                        {
                                            message += $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение больше максимума <br/>";
                                            string logText = $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение больше максимума";
                                            NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                        }
                                    }
                                    else
                                    {
                                        message += $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение больше максимума <br/>";
                                        string logText = $"На посту {monitoringPost.MN} у параметра \"{measuredParameter.NameRU}\" значение больше максимума";
                                        NewLogSendMail(monitoringPost.Id, measuredParameter.Id, logText);
                                    }
                                    checkLogSendMail = true;
                                }
                                checkPost = check = checkMin = checkMax = true;
                            }
                        }
                        if (message != "")
                        {
                            Task.WaitAll(SendEmailAsync(persons, Heading, message));
                        }
                    }
                    lastCheckDateTime = DateTime.Now;
                }
                //=================================================================================================================================================================
                // Writing data to a file for download
                Dictionary<int, string> postDistrictPairsRu = new Dictionary<int, string>
                {
                    [11] = "Турксибский район",  // ПНЗ №29
                    [12] = "Алатауский район",   // ПНЗ №30
                    [13] = "Бостандыкский район", // ПНЗ №31
                    [19] = "Медеуский район", // ПНЗ №6
                    [157] = "Жетысуский район",   // Alm-005
                    [161] = "Алмалинский район",  // Alm-008
                    [163] = "Ауэзовский район",   // Alm-010
                    [151] = "Наурызбайский район" // Alm-003

                };
                Dictionary<int, string> postDistrictPairsKk = new Dictionary<int, string>
                {
                    [11] = "Түрксіб ауданы",  // ПНЗ №29
                    [12] = "Алатау ауданы",   // ПНЗ №30
                    [13] = "Бостандық ауданы", // ПНЗ №31
                    [19] = "Медеу ауданы", // ПНЗ №6
                    [157] = "Жетісу ауданы",   // Alm-005
                    [161] = "Алмалы ауданы",  // Alm-008
                    [163] = "Әуезов ауданы",   // Alm-010
                    [151] = "Наурызбай ауданы" // Alm-003

                };
                Dictionary<int, string> pollutionLevelRu = new Dictionary<int, string>
                {
                    [1] = "Уровень низкий",
                    [2] = "Уровень повышенный",
                    [3] = "Уровень высокий",
                    [4] = "Уровень опасный"
                };
                Dictionary<int, string> pollutionLevelKk = new Dictionary<int, string>
                {
                    [1] = "Төмен деңгей",
                    [2] = "Көтеріңкі деңгей",
                    [3] = "Жоғары деңгей",
                    [4] = "Қауіпті деңгей"
                };

                if ((DateTime.Now - lastWriteFileDateTime) > new TimeSpan(0, 3, 0, 0))
                {
                    MeasuredData measuredData = new MeasuredData();
                    // Get measured data for monitoring posts
                    NewLog($"Writing data to a file >> Get data");
                    foreach (var postDistrict in postDistrictPairsRu)
                    {
                        try
                        {
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                            {
                                var monitoringPostMeasuredParametersv = connection.Query<MonitoringPostMeasuredParameter>($"SELECT * " +
                                $"FROM public.\"MonitoringPostMeasuredParameters\" " +
                                $"WHERE \"MonitoringPostId\" = '{postDistrict.Key}' " +
                                $"ORDER BY \"MeasuredParameterId\"", commandTimeout: 86400);
                                monitoringPostMeasuredParameters = monitoringPostMeasuredParametersv.ToList();

                                //var measuredParametersv = connection.Query<MeasuredParameter>(
                                //    $"SELECT \"Id\", \"MPCMaxSingle\", \"NameRU\", \"NameKK\" " +
                                //    $"FROM public.\"MeasuredParameter\" WHERE \"MPCMaxSingle\" is not null;");
                                //measuredParameters = measuredParametersv.ToList();

                                var measuredParametersv = connection.Query<MeasuredParameter>(
                                    $"SELECT \"Id\", \"MPCMaxSingle\", \"NameRU\", \"NameKK\" " +
                                    $"FROM public.\"MeasuredParameter\" WHERE \"MPCMaxSingle\" is not null AND \"Id\" = 3;"); //measuredParameter only PM2.5
                                measuredParameters = measuredParametersv.ToList();
                            }

                            foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
                            {
                                try
                                {
                                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                                    {
                                        var measuredDatasv = connection.Query<MeasuredData>($"SELECT * " +
                                        $"FROM public.\"MeasuredData\" as datas " +
                                        $"JOIN public.\"MeasuredParameter\" as param ON param.\"Id\" = datas.\"MeasuredParameterId\" " +
                                        $"WHERE datas.\"MonitoringPostId\" = '{postDistrict.Key}' AND datas.\"MeasuredParameterId\" = '{monitoringPostMeasuredParameter.MeasuredParameterId}' AND datas.\"DateTime\" > '{lastWriteFileDateTime.ToString("yyyy-MM-dd HH:mm:ss")}' AND datas.\"DateTime\" is not null AND datas.\"Averaged\" = true AND param.\"MPCMaxSingle\" is not null " +
                                        $"ORDER BY datas.\"DateTime\" DESC " +
                                        $"LIMIT 1", commandTimeout: 86400);
                                        measuredData = measuredDatasv.FirstOrDefault();

                                        if (measuredData != null)
                                        {
                                            measuredDatasWriteFile.Add(measuredData);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    NewLog($"Writing data to a file >> Error get measured data: {ex.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NewLog($"Writing data to a file >> Error get activated parameters: {ex.Message}");
                        }
                    }

                    // Write data from formed array
                    NewLog($"Writing data to a file >> Write data");
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(@"C:\Users\Administrator\source\repos\Download\AlmatyPollution.txt", false, System.Text.Encoding.Default))
                        {
                            foreach (var postDistrict in postDistrictPairsRu)
                            {
                                sw.WriteLine(postDistrict.Value.ToUpper());
                                foreach (var data in measuredDatasWriteFile.Where(m => m.MonitoringPostId == postDistrict.Key))
                                {
                                    var measuredParameter = measuredParameters.Where(m => m.Id == data.MeasuredParameterId).FirstOrDefault();
                                    if (measuredParameter != null)
                                    {
                                        decimal index = Convert.ToDecimal(data.Value / measuredParameter.MPCMaxSingle);
                                        string level = String.Empty;
                                        if (index <= 0.2m)
                                        {
                                            level = pollutionLevelRu[1];
                                        }
                                        else if (index <= 0.5m)
                                        {
                                            level = pollutionLevelRu[2];
                                        }
                                        else if (index <= 1m)
                                        {
                                            level = pollutionLevelRu[3];
                                        }
                                        else
                                        {
                                            level = pollutionLevelRu[4];
                                        }

                                        sw.WriteLine(measuredParameter.NameRU.ToUpper());
                                        sw.WriteLine(level.ToUpper());
                                    }
                                }
                                sw.WriteLine();
                            }
                            sw.WriteLine("-------------------------------------- \n");
                            foreach (var postDistrict in postDistrictPairsKk)
                            {
                                sw.WriteLine(postDistrict.Value.ToUpper());
                                foreach (var data in measuredDatasWriteFile.Where(m => m.MonitoringPostId == postDistrict.Key))
                                {
                                    var measuredParameter = measuredParameters.Where(m => m.Id == data.MeasuredParameterId).FirstOrDefault();
                                    if (measuredParameter != null)
                                    {
                                        decimal index = Convert.ToDecimal(data.Value / measuredParameter.MPCMaxSingle);
                                        string level = String.Empty;
                                        if (index <= 0.2m)
                                        {
                                            level = pollutionLevelKk[1];
                                        }
                                        else if (index <= 0.5m)
                                        {
                                            level = pollutionLevelKk[2];
                                        }
                                        else if (index <= 1m)
                                        {
                                            level = pollutionLevelKk[3];
                                        }
                                        else
                                        {
                                            level = pollutionLevelKk[4];
                                        }

                                        sw.WriteLine(measuredParameter.NameKK.ToUpper());
                                        sw.WriteLine(level.ToUpper());
                                    }
                                }
                                sw.WriteLine();
                            }
                        }

                        lastWriteFileDateTime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        NewLog($"Writing data to a file >> Error write data: {ex.Message}");
                    }
                }
                //=================================================================================================================================================================
                // Send report for Zhanats, Altynalmas posts
                if (lastSendReportDateTime.AddHours(1) < DateTime.Now && lastSendReportDateTime.ToShortDateString() != DateTime.Now.ToShortDateString())
                {
                    var reportPersons = new List<Person>();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var personsv = connection.Query<Person>($"SELECT \"Id\", \"Email\", \"Role\" " +
                            $"FROM public.\"Person\" " +
                            $"WHERE \"Role\" = 'Zhanatas' OR \"Role\" = 'Altynalmas' " +
                            $"ORDER BY \"Id\"");
                        reportPersons = personsv.ToList();
                    }
                    var zhanatasPersons = reportPersons.Where(p => p.Role == "Zhanatas").Select(p => p.Email).ToList();
                    var altynalmasPersons = reportPersons.Where(p => p.Role == "Altynalmas").Select(p => p.Email).ToList();
                    Task.WaitAll(SendReport(Projects.Zhanatas, zhanatasPersons));
                    Task.WaitAll(SendReport(Projects.Altynalmas, altynalmasPersons));
                    lastSendReportDateTime = DateTime.Now;
                }

                Thread.Sleep(30000);
            }
        }

        private static async Task SendReport(Projects project, List<string> emailsTo)
        {
            try
            {
                NewLog($"Send report for {project} posts >> Forming request");

                Dictionary<string, string> nameProjects = new Dictionary<string, string>
                {
                    ["Zhanatas"] = "Жанатас",
                    ["Altynalmas"] = "Алтыналмас"
                };

                HttpResponseMessage result = new HttpResponseMessage();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = Debugger.IsAttached ?
                        new Uri("http://localhost:52207") :
                        new Uri("http://localhost:8084");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdGlyZGFyQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Im1vZGVyYXRvciIsIm5iZiI6MTY5ODkyNzI3NSwiZXhwIjoxNzMwNTQ5Njc1LCJpc3MiOiJTbWFydEVjbyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIyMDcvIn0.4lUrxsfS2eHFTlsM59jzuaGJmgh3jJ7iRUQ14wDbE1M");
                    client.Timeout = TimeSpan.FromMinutes(30);

                    var dateTimeYesterdayFrom = DateTime.Now.AddDays(-1).Date + new TimeSpan(00, 00, 00);
                    var dateTimeYesterdayTo = DateTime.Now.AddDays(-1).Date + new TimeSpan(23, 59, 59);
                    DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CreateSpecificCulture("en").DateTimeFormat;
                    var monitoringPostIds = new List<int>(); //Zhanatas posts: 234, 235
                    var measuredParameterIds = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 9, 13, 19, 73 };

                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"MN\", \"DataProviderId\", \"ProjectId\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"DataProviderId\" = '{(int)DataProviders.Ecoservice}' and \"ProjectId\" = '{(int)project}';");
                        monitoringPostIds = monitoringPostsv
                            .Select(post => post.Id)
                            .ToList();
                        connection.Close();
                    }

                    if (monitoringPostIds.Count is 0)
                    {
                        NewLog($"Send report for {project} posts >> No monitoring posts");
                        return;
                    }

                    string url = "api/Analytics/ExcelFormationByProject",
                    route = "";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeFrom={dateTimeYesterdayFrom.ToString(dateTimeFormatInfo)}";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"DateTimeTo={dateTimeYesterdayTo.ToString(dateTimeFormatInfo)}";

                    foreach (var monitoringPostId in monitoringPostIds)
                    {
                        route += string.IsNullOrEmpty(route) ? "?" : "&";
                        route += $"MonitoringPostsId={monitoringPostId}".Replace(',', '.');
                    }

                    foreach (var measuredParametersId in measuredParameterIds)
                    {
                        route += string.IsNullOrEmpty(route) ? "?" : "&";
                        route += $"MeasuredParametersId={measuredParametersId}".Replace(',', '.');
                    }

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"Server={!Debugger.IsAttached}";

                    route += string.IsNullOrEmpty(route) ? "?" : "&";
                    route += $"Project={nameProjects[project.ToString()]}";

                    emailsTo.Add("gervana81@mail.ru");
                    foreach (var emailTo in emailsTo)
                    {
                        if (IsValidEmail(emailTo))
                        {
                            route += string.IsNullOrEmpty(route) ? "?" : "&";
                            route += $"MailTo={emailTo}";
                        }
                    }

                    result = await client.PostAsync(url + route, null);
                }

                if (result.IsSuccessStatusCode)
                {
                    NewLog($"Send report for {project} posts >> Success sended");
                }
                else
                {
                    NewLog($"Send report for {project} posts >> {result.StatusCode}: {result.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                NewLog($"Send report for {project} posts >> Error: {ex.Message}");
            }
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

        public static async Task SendEmailAsync(List<Person> persons, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(Theme, FromEmail));
                foreach (var person in persons)
                {
                    if (IsValidEmail(person.Email))
                    {
                        emailMessage.To.Add(new MailboxAddress("", person.Email));
                    }
                }
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };
                NewLog($"Send Email. Email Generated");

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SMTPServer, SMTPPort, true);
                    NewLog($"Send Email. Connect Successful");
                    await client.AuthenticateAsync(FromEmail, Password);
                    NewLog($"Send Email. Authenticate Successful");
                    await client.SendAsync(emailMessage);
                    NewLog($"Send Email. Send Successful");

                    await client.DisconnectAsync(true);
                    NewLog($"Send Email. Disconnect Successful");
                }
            }
            catch (Exception ex)
            {
                NewLog($"Send Email. Error: {ex.Message}");
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //public static void CreateMail(
        //    string message,
        //    List<Person> persons)
        //{
        //    foreach (var person in persons)
        //    {
        //        if (IsValidEmail(person.Email))
        //        {
        //            Task.WaitAll(SendEmailAsync(person.Email, Heading, message));
        //        }
        //    }
        //}

        public static void NewLogSendMail(
            int? MonitoringPostId,
            int? MeasuredParameterId,
            string LogSendMail)
        {
            string MonitoringPostIdStr = MonitoringPostId.ToString(),
                MeasuredParameterIdStr = MeasuredParameterId.ToString();
            if (MonitoringPostId == null)
            {
                MonitoringPostIdStr = "null";
            }
            if (MeasuredParameterId == null)
            {
                MeasuredParameterIdStr = "null";
            }
            Console.WriteLine($"{DateTime.Now.ToString()} >> {LogSendMail}{Environment.NewLine}");
            using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
            {
                connection.Open();
                DateTime now = DateTime.Now;
                string execute = $"INSERT INTO public.\"LogSendMail\"(" +
                    $"\"DateTime\"," +
                    $"\"MonitoringPostId\"," +
                    $"\"MeasuredParameterId\", " +
                    $"\"Log\") " +
                    $"VALUES (" +
                    $"make_timestamptz(" +
                        $"{now.Year.ToString()}, " +
                        $"{now.Month.ToString()}, " +
                        $"{now.Day.ToString()}, " +
                        $"{now.Hour.ToString()}, " +
                        $"{now.Minute.ToString()}, " +
                        $"{now.Second.ToString()})," +
                    $"{MonitoringPostIdStr}," +
                    $"{MeasuredParameterIdStr}," +
                    $"'{LogSendMail}'" +
                    $");";
                connection.Execute(execute);
                connection.Close();
            }
        }
    }
}

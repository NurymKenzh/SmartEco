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
using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using System.Globalization;

namespace GetPostsData
{
    class Program
    {
        const string Heading = "Нет данных!",
            Theme = "SmartEco",
            FromEmail = "smartecokz@gmail.com",
            Password = "Qwerty123_",
            SMTPServer = "smtp.gmail.com",
            POPServer = "pop.gmail.com";
        const int SMTPPort = 465,
            POPPort = 995;

        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string OceanusCode { get; set; }
            public string NameRU { get; set; }
        }
        public class MonitoringPost
        {
            public int Id { get; set; }
            public string MN { get; set; }
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
            public string Coefficient { get; set; }
        }
        static void Main(string[] args)
        {
            NewLog("Program started!");
            DateTime lastBackupDateTime = new DateTime(2000, 1, 1);
            DateTime lastCheckDateTime = new DateTime(2000, 1, 1);
            DateTime lastGetSourceDateTime = new DateTime(2000, 1, 1);
            while (true)
            {
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                List<MonitoringPostMeasuredParameter> monitoringPostMeasuredParameters = new List<MonitoringPostMeasuredParameter>();
                List<MeasuredData> measuredDatasCheck = new List<MeasuredData>();
                List<MeasuredData> measuredDatasCheckMinMax = new List<MeasuredData>();
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
                            $"SELECT \"MonitoringPostId\", \"MeasuredParameterId\", \"Coefficient\"" +
                            $"FROM public.\"MonitoringPostMeasuredParameters\"");
                    monitoringPostMeasuredParameters = monitoringPostMeasuredParametersv.ToList();
                }

                // Copy data
                // Get Data from PostsData
                NewLog("Get. Get Data from PostsData started");
                int postDatasCount = 0;
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    List<MeasuredData> measuredDatas = new List<MeasuredData>();
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = false WHERE \"Taken\" is null;", commandTimeout: 86400);
                    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\" FROM public.\"Data\" WHERE \"Taken\" = false;", commandTimeout: 86400);
                    postDatasCount = postDatas.Count();
                    try
                    {
                        foreach (PostData postData in postDatas)
                        {
                            foreach (string value in postData.Data.Split(";").Where(d => d.Contains("-Rtd")))
                            {
                                int? MeasuredParameterId = measuredParameters.FirstOrDefault(m => m.OceanusCode == value.Split("-Rtd")[0])?.Id,
                                    MonitoringPostId = monitoringPosts.FirstOrDefault(m => m.MN == postData.MN)?.Id;
                                var checkPost = monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId).ToList();
                                var coef = (checkPost.Count != 0) ? monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().Coefficient : null;
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
                                        if (coef != null)
                                        {
                                            measuredDatas.Add(new MeasuredData()
                                            {
                                                //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                                DateTime = postData.DateTimeServer,
                                                MeasuredParameterId = (int)MeasuredParameterId,
                                                MonitoringPostId = (int)MonitoringPostId,
                                                Value = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0]) * Convert.ToDecimal(coef)
                                            });
                                        }
                                        else
                                        {
                                            measuredDatas.Add(new MeasuredData()
                                            {
                                                //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                                DateTime = postData.DateTimeServer,
                                                MeasuredParameterId = (int)MeasuredParameterId,
                                                MonitoringPostId = (int)MonitoringPostId,
                                                Value = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0])
                                            });
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    NewLog($"Get. Get Data from PostsData finished. Data from PostsData count: {postDatasCount.ToString()}. Data to MeasuredDatas count: {measuredDatas.Count().ToString()}. " +
                        $"From {measuredDatas.Min(m => m.DateTime).ToString()} to {measuredDatas.Max(m => m.DateTime).ToString()}");
                    NewLog($"Get. Insert data to MeasuredDatas started");
                    // Insert MeasuredDatas into SmartEcoAPI
                    try
                    {
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            foreach (MeasuredData measuredData in measuredDatas)
                            {
                                string execute = $"INSERT INTO public.\"MeasuredData\"(\"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\")" +
                                    $"VALUES({measuredData.MeasuredParameterId.ToString()}," +
                                    $"make_timestamptz(" +
                                        $"{measuredData.DateTime?.Year.ToString()}, " +
                                        $"{measuredData.DateTime?.Month.ToString()}, " +
                                        $"{measuredData.DateTime?.Day.ToString()}, " +
                                        $"{measuredData.DateTime?.Hour.ToString()}, " +
                                        $"{measuredData.DateTime?.Minute.ToString()}, " +
                                        $"{measuredData.DateTime?.Second.ToString()})," +
                                    $"{measuredData.Value.ToString()}," +
                                    $"{measuredData.MonitoringPostId.ToString()});";
                                connection2.Execute(execute);
                            }
                        }
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            connection2.Execute("UPDATE public.\"Data\" SET \"Taken\" = true WHERE \"Taken\" = false;", commandTimeout: 86400);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var connection2 = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                        {
                            connection2.Open();
                            connection2.Execute("UPDATE public.\"Data\" SET \"Taken\" = null WHERE \"Taken\" = false;", commandTimeout: 86400);
                        }
                    }
                    NewLog($"Get. Insert data to MeasuredDatas finished");
                }
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
                                var checkPost = monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId).ToList();
                                var coef = (checkPost.Count != 0) ? monitoringPostMeasuredParameters.Where(m => m.MonitoringPostId == MonitoringPostId && m.MeasuredParameterId == MeasuredParameterId).FirstOrDefault().Coefficient : null;
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
                                        if (coef != null)
                                        {
                                            measuredDatas.Add(new MeasuredData()
                                            {
                                                Id = postData.Id,
                                                //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                                DateTime = postData.DateTimeServer,
                                                MeasuredParameterId = (int)MeasuredParameterId,
                                                MonitoringPostId = (int)MonitoringPostId,
                                                Value = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0]) * Convert.ToDecimal(coef)
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
                                                Value = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0])
                                            });
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
                // Backup data
                long countBackup = 0;
                if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                {
                    NewLog($"Backup. Get Data from MeasuredData (SmartEcoAPI) started");
                    DateTime dateTimeLast = DateTime.Now.AddDays(-30);
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasB = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"Ecomontimestamp_ms\", \"MaxValueDay\", \"MaxValueMonth\", \"Month\", \"Year\", \"MaxValuePerMonth\", \"MaxValuePerYear\", \"MonitoringPostId\", \"PollutionSourceId\", \"Averaged\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null;", commandTimeout: 86400);
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
                                $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null;", commandTimeout: 86400);
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

                    NewLog($"Backup. Get Data from Log (PostsData) started");
                    countBackup = 0;
                    if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                    {
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
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
                                string data = log.Log.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "") + "\t" +
                                    log.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                                if (!File.Exists(fileName))
                                {
                                    File.AppendAllText(fileName, $"Log\tDateTime" + Environment.NewLine);
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
                    NewLog($"Backup. Get Data from Log (PostsData) finished. {countBackup.ToString()} rows backed up");

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
                            //.Where(p => p.Email != "rkostylev@ecoservice.kz" &&
                            //    p.Email != "biskakov@ecoservice.kz" &&
                            //    p.Email != "n.a.k@bk.ru")
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
                                CreateMail(message, persons);
                                NewLogSendMail(null, null, message);
                            }
                        }
                        else
                        {
                            message = "Нет данных по всем постам!";
                            CreateMail(message, persons);
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
                            CreateMail(message, persons);
                        }
                    }
                    lastCheckDateTime = DateTime.Now;
                }
                //=================================================================================================================================================================
                // Get Data For 138 Source
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
                    List<int> measuredParametersId = new List<int> { 1, 5, 6, 19 };

                    NewLog("Get. Get Data for Pollution Source started");
                    if(measuredDatasSourceDB.Count != 0)
                    {
                        var measuredDatasSO2 = measuredDatasSourceDB
                            .Where(m => m.MeasuredParameterId == 9)
                            .LastOrDefault();
                        if (measuredDatasSO2 == null)
                        {
                            measuredDatasSource = ReceiveEmailAsync(null);
                        }
                        else
                        {
                            measuredDatasSource = ReceiveEmailAsync(measuredDatasSO2.DateTime);
                        }

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

                        measuredDatasSource = measuredDatasSource
                            .OrderBy(m => m.DateTime)
                            .ToList();
                    }
                    else
                    {
                        measuredDatasSource = ReceiveEmailAsync(null);

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

        public static async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(Theme, FromEmail));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SMTPServer, SMTPPort, true);
                    await client.AuthenticateAsync(FromEmail, Password);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch
            {

            }
        }

        public static List<MeasuredData> ReceiveEmailAsync(DateTime? dateTime)
        {
            try
            {
                List<MeasuredData> measuredDatas = new List<MeasuredData>();

                using (ImapClient client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(FromEmail, Password);
                    IMailFolder inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
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
                    foreach (var uid in inbox.Search(query))
                    {
                        List<string> text = new List<string>();
                        var message = inbox.GetMessage(uid);
                        var html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(message.HtmlBody);
                        html.DocumentNode.SelectNodes("//span/text()").ToList().ForEach(x => text.Add(x.InnerHtml.Replace("\t","")));
                        
                        var value = text[text.FindIndex(x => x.Contains("Значение")) + 1];
                        var date = text[text.FindIndex(x => x.Contains("среднего")) + 1];
                        date = date.Substring(0, date.IndexOf("Central") - 1);
                        var dateTimeServer = DateTime.ParseExact(date, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
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
                }

                return measuredDatas;
            }
            catch (Exception ex)
            {
                return null;
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

        public static void CreateMail(
            string message,
            List<Person> persons)
        {
            foreach (var person in persons)
            {
                if (IsValidEmail(person.Email))
                {
                    Task.WaitAll(SendEmailAsync(person.Email, Heading, message));
                }
            }
        }

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

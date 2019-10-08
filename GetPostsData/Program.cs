﻿using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace GetPostsData
{
    class Program
    {
        const string Heading = "No data!",
            Theme = "SmartEco",
            FromEmail = "testmail@gmail.com",
            Password = "12345678",
            SMTPServer = "smtp.gmail.com";
        const int SMTPPort = 465;

        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string OceanusCode { get; set; }
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
        }
        static void Main(string[] args)
        {
            NewLog("Program started!");
            DateTime lastBackupDateTime = new DateTime(2000, 1, 1);
            DateTime lastCheckDateTime = new DateTime(2000, 1, 1);
            while (true)
            {
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                List<MeasuredData> measuredDatasCheck = new List<MeasuredData>();
                List<Person> persons = new List<Person>();

                // Get MeasuredParameters, MonitoringPosts
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
                }

                // Copy data
                // Get Data from PostsData
                NewLog("Get. Get Data from PostsData started");
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                int postDatasCount = 0;
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = false WHERE \"Taken\" is null;");
                    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\" FROM public.\"Data\" WHERE \"Taken\" = false;");
                    postDatasCount = postDatas.Count();
                    try
                    {
                        foreach (PostData postData in postDatas)
                        {
                            foreach (string value in postData.Data.Split(";").Where(d => d.Contains("-Rtd")))
                            {
                                int? MeasuredParameterId = measuredParameters.FirstOrDefault(m => m.OceanusCode == value.Split("-Rtd")[0])?.Id,
                                    MonitoringPostId = monitoringPosts.FirstOrDefault(m => m.MN == postData.MN)?.Id;
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
                                        measuredDatas.Add(new MeasuredData()
                                        {
                                            //DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
                                            DateTime = postData.DateTimeServer,
                                            MeasuredParameterId = (int)MeasuredParameterId,
                                            MonitoringPostId = (int)MonitoringPostId,
                                            Value = Convert.ToDecimal(value.Split("-Rtd=")[1].Split("&&")[0])
                                        });
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
                }
                NewLog($"Get. Get Data from PostsData finished. Data from PostsData count: {postDatasCount.ToString()}. Data to MeasuredDatas count: {measuredDatas.Count().ToString()}. " +
                    $"From {measuredDatas.Min(m => m.DateTime).ToString()} to {measuredDatas.Max(m => m.DateTime).ToString()}");
                NewLog($"Get. Insert data to MeasuredDatas started");
                // Insert MeasuredDatas into SmartEcoAPI
                try
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
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
                            connection.Execute(execute);
                        }
                    }
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = true WHERE \"Taken\" = false;");
                    }
                }
                catch (Exception ex)
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = null WHERE \"Taken\" = false;");
                    }
                }
                NewLog($"Get. Insert data to MeasuredDatas finished");
                //=================================================================================================================================================================
                // Average data
                // Get Data from PostsData
                NewLog($"Average. Get Data from PostsData started");
                measuredDatas = new List<MeasuredData>();
                List<MeasuredData> measuredDatasAveraged = new List<MeasuredData>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = false WHERE \"Averaged\" is null;");
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
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
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
                            connection.Execute(execute);
                        }
                    }
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        //connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = true WHERE \"Averaged\" = false;");
                        foreach (long id in averagedPostsDatas)
                        {
                            connection.Execute($"UPDATE public.\"Data\" SET \"Averaged\" = true WHERE \"Id\" = {id.ToString()};");
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = null WHERE \"Averaged\" = false;");
                    }
                }
                NewLog($"Average. Insert data to MeasuredDatas finished");
                //=================================================================================================================================================================
                //// Backup data
                //NewLog($"Backup. Get Data from MeasuredData started");
                //if ((DateTime.Now - lastBackupDateTime) > new TimeSpan(1, 0, 0, 0))
                //{
                //    DateTime dateTimeLast = DateTime.Now.AddDays(-30);
                //    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                //    {
                //        connection.Open();
                //        var measuredDatas = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"Ecomontimestamp_ms\", \"MaxValueDay\", \"MaxValueMonth\", \"Month\", \"Year\", \"MaxValuePerMonth\", \"MaxValuePerYear\", \"MonitoringPostId\", \"PollutionSourceId\", \"Averaged\" " +
                //            $"FROM public.\"MeasuredData\" " +
                //            $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null;");
                //        foreach (MeasuredData measuredData in measuredDatas)
                //        {
                //            string fileName = Path.Combine("E:\\Documents\\New", $"MeasuredData {measuredData.DateTime?.ToString("yyyy-MM")}");
                //            fileName = Path.ChangeExtension(fileName, "csv");
                //            string data = measuredData.MeasuredParameterId.ToString() + ";" +
                //                measuredData.DateTime?.ToString("yyyy-MM-dd HH:mm:ss") + ";" +
                //                measuredData.Value?.ToString() + ";" +
                //                measuredData.Ecomontimestamp_ms?.ToString() + ";" +
                //                measuredData.MaxValueDay?.ToString() + ";" +
                //                measuredData.MaxValueMonth?.ToString() + ";" +
                //                measuredData.Month?.ToString() + ";" +
                //                measuredData.Year?.ToString() + ";" +
                //                measuredData.MaxValuePerMonth?.ToString() + ";" +
                //                measuredData.MaxValuePerYear?.ToString() + ";" +
                //                measuredData.MonitoringPostId?.ToString() + ";" +
                //                measuredData.PollutionSourceId?.ToString() + ";" +
                //                measuredData.Averaged?.ToString() + Environment.NewLine;
                //            if (!File.Exists(fileName))
                //            {
                //                File.AppendAllText(fileName, $"MeasuredParameterId;DateTime;Value;Ecomontimestamp_ms;MaxValueDay;MaxValueMonth;" +
                //                    $"Month;Year;MaxValuePerMonth;MaxValuePerYear;MonitoringPostId;PollutionSourceId;Averaged" + Environment.NewLine);
                //            }
                //            File.AppendAllText(fileName, data);
                //        }
                //        try
                //        {
                //            //connection.Execute($"DELETE FROM public.\"MeasuredData\" " +
                //            //    $"WHERE \"DateTime\" < '{dateTimeLast.ToString("yyyy-MM-dd")}' AND \"DateTime\" is not null;");
                //        }
                //        catch
                //        {

                //        }
                //    }
                //    lastBackupDateTime = DateTime.Now;
                //}

                //=================================================================================================================================================================
                // Check data
                if ((DateTime.Now - lastCheckDateTime) > new TimeSpan(0, 1, 0, 0))
                {
                    List<LogSendMail> logSendMails = new List<LogSendMail>();
                    DateTime dateTimeLast = DateTime.Now.AddMinutes(-20);
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var measuredDatasv = connection.Query<MeasuredData>($"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\" " +
                            $"FROM public.\"MeasuredData\" " +
                            $"WHERE \"DateTime\" > '{dateTimeLast.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"");
                        measuredDatasCheck = measuredDatasv.ToList();

                        var measuredParametersv = connection.Query<MeasuredParameter>(
                            $"SELECT \"Id\", \"OceanusCode\"" +
                            $"FROM public.\"MeasuredParameter\" WHERE \"OceanusCode\" <> '' and \"OceanusCode\" is not null;");
                        measuredParameters = measuredParametersv.ToList();

                        var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"MN\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"MN\" <> '' and \"MN\" is not null;");
                        monitoringPosts = monitoringPostsv.ToList();

                        var personsv = connection.Query<Person>($"SELECT \"Id\", \"Email\" " +
                            $"FROM public.\"Person\" " +
                            $"WHERE \"Role\" = 'admin' OR \"Role\" = 'moderator' " +
                            $"ORDER BY \"Id\"");
                        persons = personsv.ToList();
                    }
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=GetPostsData;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        DateTime dateTimeLastWrite = DateTime.Now.AddHours(-24);
                        var logSendMailsv = connection.Query<LogSendMail>($"SELECT \"DateTime\", \"MeasuredParameterId\", \"MonitoringPostId\" " +
                            $"FROM public.\"LogSendMail\" " +
                            $"WHERE \"DateTime\" > '{dateTimeLast.ToString("yyyy-MM-dd HH:mm:ss")}' AND \"DateTime\" is not null " +
                            $"ORDER BY \"DateTime\"");
                        logSendMails = logSendMailsv.ToList();
                    }

                    bool check = true,
                        checkPost = true,
                        checkLogSendMail = true;
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
                                string message = "Missing data for all posts";
                                CreateMail(message, persons);
                                NewLogSendMail(null, null, message);
                            }
                        }
                        else
                        {
                            string message = "Missing data for all posts";
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
                                foreach (var measuredData in measuredDatasCheck)
                                {
                                    if (measuredData.MonitoringPostId == monitoringPost.Id)
                                    {
                                        checkPost = false;
                                    }
                                    if (measuredData.MonitoringPostId == monitoringPost.Id && measuredData.MeasuredParameterId == measuredParameter.Id)
                                    {
                                        check = false;
                                        break;
                                    }
                                }
                                if (checkPost)
                                {
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
                                            string message = $"Missing data for post {monitoringPost.MN}";
                                            CreateMail(message, persons);
                                            NewLogSendMail(monitoringPost.Id, null, message);
                                        }
                                    }
                                    else
                                    {
                                        string message = $"Missing data for post {monitoringPost.MN}";
                                        CreateMail(message, persons);
                                        NewLogSendMail(monitoringPost.Id, null, message);
                                    }
                                }
                                else if (check)
                                {
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
                                            string message = $"Missing data for parameter {measuredParameter.OceanusCode} on post {monitoringPost.MN}";
                                            CreateMail(message, persons);
                                            NewLogSendMail(monitoringPost.Id, measuredParameter.Id, message);
                                        }
                                    }
                                    else
                                    {
                                        string message = $"Missing data for parameter {measuredParameter.OceanusCode} on post {monitoringPost.MN}";
                                        CreateMail(message, persons);
                                        NewLogSendMail(monitoringPost.Id, measuredParameter.Id, message);
                                    }
                                }
                                checkPost = check = true;
                            }
                        }
                    }
                    lastCheckDateTime = DateTime.Now;
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

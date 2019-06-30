using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace GetPostsData
{
    class Program
    {
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
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Copy data from posts to database and average every 10 seconds!");
            while (true)
            {
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();

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
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = false WHERE \"Taken\" is null;");
                    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\" FROM public.\"Data\" WHERE \"Taken\" = false;");
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
                                        if(postData.DateTimePost == null)
                                        {
                                            adequateDateTimePost = false;
                                        }
                                        else if (Math.Abs((postData.DateTimePost.Value - postData.DateTimeServer).Days) > 3)
                                        {
                                            adequateDateTimePost = false;
                                        }
                                        measuredDatas.Add(new MeasuredData()
                                        {
                                            DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
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

                //=================================================================================================================================================================
                // Average data
                // Get Data from PostsData
                measuredDatas = new List<MeasuredData>();
                List<MeasuredData> measuredDatasAveraged = new List<MeasuredData>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = false WHERE \"Averaged\" is null;");
                    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Averaged\" FROM public.\"Data\" WHERE \"Averaged\" = false;");
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
                                            DateTime = adequateDateTimePost ? postData.DateTimePost : postData.DateTimeServer,
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

                // Average data
                if(measuredDatas.Count()>0)
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
                                }
                            }
                        }
                    }
                }                

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
                        connection.Execute("UPDATE public.\"Data\" SET \"Averaged\" = true WHERE \"Averaged\" = false;");
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

                Thread.Sleep(10000);
            }
        }
    }
}

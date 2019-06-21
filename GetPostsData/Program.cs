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
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Copy data from posts to database every 10 seconds!");
            while (true)
            {
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();

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
                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = false WHERE \"Taken\" is null;");
                    var postDatas = connection.Query<PostData>("SELECT \"Data\", \"DateTimeServer\", \"DateTimePost\", \"MN\", \"IP\", \"Taken\" FROM public.\"Data\" WHERE \"Taken\" = false;");
                    try
                    {
                        // Data -> DB
                        
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
                                        else if((postData.DateTimePost.Value - postData.DateTimeServer).Days > 3)
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
                        //_context.AddRange(measuredDatas);
                        //_context.SaveChanges();
                        
                        //connection.Execute("UPDATE public.\"Data\" SET \"Taken\" = true WHERE \"Taken\" = false;");
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
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
                
                Thread.Sleep(10000);
            }
        }
    }
}

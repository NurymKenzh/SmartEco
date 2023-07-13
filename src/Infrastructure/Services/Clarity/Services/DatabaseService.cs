using Clarity.Helpers;
using Clarity.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clarity.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres;Timeout=300;CommandTimeout=300";

        public List<MonitoringPost> GetMonitoringPosts()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var monitoringPostsv = connection.Query<MonitoringPost>(
                $" SELECT \"Id\", \"Name\"" +
                $" FROM public.\"MonitoringPost\"" +
                $" WHERE \"DataProviderId\" = 4");
            return monitoringPostsv.ToList();
        }

        public DateTime GetLastDateTime()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var lastDateTimeDb = connection.Query<DateTime?>(
                $" SELECT md.\"DateTime\"" +
                $" FROM public.\"MeasuredData\" md" +
                $" LEFT JOIN public.\"MonitoringPost\" mp ON mp.\"Id\" = md.\"MonitoringPostId\"" +
                $" WHERE mp.\"DataProviderId\" = 4" +
                $" ORDER BY md.\"DateTime\" DESC" +
                $" LIMIT 1;").FirstOrDefault();
            return lastDateTimeDb is null ? DateTime.UtcNow.AddHours(-2) : TimeZoneConverter.ToUtc(lastDateTimeDb.Value);
        }

        public void InsertMeasuredDatas(List<MeasuredData> measuredDatas)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"INSERT INTO public.\"MeasuredData\"(\"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\", \"Averaged\") VALUES ");
            foreach (MeasuredData measuredData in measuredDatas)
            {
                sql.Append($"({measuredData.MeasuredParameterId.ToString()}," +
                    $"make_timestamptz(" +
                        $"{measuredData.DateTime?.Year.ToString()}, " +
                        $"{measuredData.DateTime?.Month.ToString()}, " +
                        $"{measuredData.DateTime?.Day.ToString()}, " +
                        $"{measuredData.DateTime?.Hour.ToString()}, " +
                        $"{measuredData.DateTime?.Minute.ToString()}, " +
                        $"{measuredData.DateTime?.Second.ToString()})," +
                    $"{measuredData.Value.ToString().Replace(",", ".")}," +
                    $"{measuredData.MonitoringPostId.ToString()}," +
                    $"{measuredData.Averaged.ToString()})," + Environment.NewLine);
            }
            sql.Length -= 3;
            sql.Append(";");
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            connection.Execute(sql.ToString());
            connection.Close();
            sql.Clear();
        }
    }
}

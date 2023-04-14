using Clarity.Extensions;
using Clarity.Models;
using Clarity.Helpers;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clarity.Services;
using System.Threading.Tasks;

namespace Clarity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Program started!");
            while (true)
            {
                try
                {
                    var databaseService = new DatabaseService();
                    var monitoringPosts = databaseService.GetMonitoringPosts();
                    var lastDateTime = databaseService.GetLastDateTime();

                    if (NeedNewData(lastDateTime))
                    {
                        LogConsole("Get Data from Clarity started");

                        var clarityMeasurements = await new ClarityApi()
                            .GetMeasurements("hour", lastDateTime.AddHours(1).Trim(TimeSpan.TicksPerHour));
                        LogConsole($"Get Data from Clarity finished | Count: {clarityMeasurements.Count()} | Date: {clarityMeasurements.FirstOrDefault()?.Time}");

                        var measuredDatas = Mapper
                            .DataClarityToSmartEco(clarityMeasurements, monitoringPosts);
                        LogConsole($"Data mapped | Count: {measuredDatas.Count()} | Date: {measuredDatas.FirstOrDefault()?.DateTime}");

                        if (measuredDatas.Count != 0)
                        {
                            LogConsole("Insert to MeasuredData started");
                            databaseService.InsertMeasuredDatas(measuredDatas);
                            LogConsole("Insert to MeasuredData finished");

                            Thread.Sleep(new TimeSpan(0, 0, 5));
                            continue;
                        }
                    }

                    Thread.Sleep(new TimeSpan(0, 5, 0));
                }
                catch (Exception ex)
                {
                    LogConsole($"Error: {ex}");
                    Thread.Sleep(new TimeSpan(0, 20, 0));
                }
            }
        }

        private static void LogConsole(string Log)
        {
            Console.WriteLine($"{DateTime.Now} >> {Log}{Environment.NewLine}");
        }

        private static bool NeedNewData(DateTime startDate)
        {
            var dateTime = DateTime.UtcNow.AddHours(-1);
            return 
                dateTime.Year != startDate.Year ||
                dateTime.Month != startDate.Month ||
                dateTime.Day != startDate.Day ||
                dateTime.Hour != startDate.Hour;
        }
    }
}

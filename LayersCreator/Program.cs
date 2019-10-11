using Dapper;
using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace LayersCreator
{
    class Program
    {
        const string GSWorkspace = "SmartEco",
            GSStyle = "KaragandaRegionPollutantSpread",
            GSAddress = "localhost",
            GSPort = "8080",
            GSUser = "admin",
            GSPassword = "geoserver",
            GSDataDir = "E:\\Documents\\Google Drive\\Geoserver\\data_dir\\data\\SmartEco\\KaragandaRegionPollutantSpread",
            //GSDataDir = "C:\\Program Files (x86)\\GeoServer 2.13.4\\data_dir\\data\\SmartEco\\KaragandaRegionPollutantSpread",
            CurlFullPath = "C:\\Windows\\curl.exe",
            LayerNameTemplate = "KaragandaRegionPollutantSpread";
        const decimal MaxDistance = 0.25M;
        const int MinPostsCount = 3;

        public class MeasuredParameter
        {
            public int Id { get; set; }
            public string OceanusCode { get; set; }
            public decimal? MPC { get; set; } // maximum permissible concentration
        }

        public class MonitoringPost
        {
            public int Id { get; set; }
            public decimal NorthLatitude { get; set; }
            public decimal EastLongitude { get; set; }
            public string MN { get; set; }
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
            while (true)
            {
                // Get MeasuredParameters, MonitoringPosts
                List<MeasuredParameter> measuredParameters = new List<MeasuredParameter>();
                List<MonitoringPost> monitoringPosts = new List<MonitoringPost>();
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                {
                    connection.Open();
                    var measuredParametersv = connection.Query<MeasuredParameter>(
                        $"SELECT \"Id\", \"OceanusCode\", \"MPC\"" +
                        $"FROM public.\"MeasuredParameter\" WHERE \"OceanusCode\" <> '' and \"OceanusCode\" is not null and \"MPC\" is not null;");
                    measuredParameters = measuredParametersv.ToList();

                    var monitoringPostsv = connection.Query<MonitoringPost>(
                            $"SELECT \"Id\", \"NorthLatitude\", \"EastLongitude\", \"MN\"" +
                            $"FROM public.\"MonitoringPost\" WHERE \"TurnOnOff\" = true and \"MN\" <> '' and \"MN\" is not null;");
                    monitoringPosts = monitoringPostsv.ToList();
                }

                //List<DateTime> times20 = new List<DateTime>();
                for (DateTime dateTime = DateTime.Today; dateTime <= DateTime.Now; dateTime = dateTime.AddMinutes(20))
                {
                    //times20.Add(i);
                    foreach (MeasuredParameter measuredParameter in measuredParameters)
                    {
                        string layer = LayerNameTemplate + "_" + measuredParameter.Id.ToString() + "_" + dateTime.ToString("yyyyMMddHHmmss") + "_1";
                        try
                        {
                            if (!LayerExists(layer))
                            {
                                // create layer
                                List<MeasuredData> measuredDatas = new List<MeasuredData>();
                                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                                {
                                    connection.Open();
                                    var measuredDatasv = connection.Query<MeasuredData>(
                                        $"SELECT \"Id\", \"MeasuredParameterId\", \"DateTime\", \"Value\", \"MonitoringPostId\", \"Averaged\"" +
                                        $" FROM public.\"MeasuredData\"" +
                                        $" WHERE \"MeasuredParameterId\" = {measuredParameter.Id}" +
                                        $" AND \"Averaged\" = true" +
                                        $" AND \"DateTime\" = '{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}'");
                                    measuredDatas = measuredDatasv.ToList();
                                }

                                // filter Monitoring Posts only with Measured Datas
                                List<MonitoringPost> monitoringPostsWithData = new List<MonitoringPost>();
                                foreach (MonitoringPost monitoringPost in monitoringPosts)
                                {
                                    if (measuredDatas.Select(m => m.MonitoringPostId).Contains(monitoringPost.Id))
                                    {
                                        monitoringPostsWithData.Add(monitoringPost);
                                    }
                                }

                                // collect monitoring posts areas
                                int[,] groups = new int[monitoringPostsWithData.Count(), 2]; // [index, 0] = group number; [index, 1] = MonitoringPostId
                                for (int i = 0; i < monitoringPostsWithData.Count(); i++)
                                {
                                    groups[i, 0] = -1;
                                    groups[i, 1] = monitoringPostsWithData[i].Id;
                                }
                                int groupsCount = 0;
                                foreach (MonitoringPost monitoringPost in monitoringPostsWithData)
                                {
                                    // if need to include in existing group
                                    for (int i = 0; i < monitoringPostsWithData.Count(); i++)
                                    {
                                        if (groups[i, 0] >= 0 && groups[i, 1] != monitoringPost.Id)
                                        {
                                            MonitoringPost monitoringPostInGroup = monitoringPostsWithData.FirstOrDefault(m => m.Id == groups[i, 1]);
                                            decimal distance = (decimal)Math.Sqrt(Math.Pow((double)(monitoringPost.NorthLatitude - monitoringPostInGroup.NorthLatitude), 2) +
                                                Math.Pow((double)(monitoringPost.EastLongitude - monitoringPostInGroup.EastLongitude), 2));
                                            if (distance <= MaxDistance)
                                            {
                                                for (int j = 0; j < monitoringPostsWithData.Count(); j++)
                                                {
                                                    if (groups[j, 1] == monitoringPost.Id)
                                                    {
                                                        groups[j, 0] = groups[i, 0];
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    // else create new group
                                    for (int i = 0; i < monitoringPostsWithData.Count(); i++)
                                    {
                                        if (groups[i, 0] < 0 && groups[i, 1] == monitoringPost.Id)
                                        {
                                            groupsCount++;
                                            groups[i, 0] = groupsCount;
                                            break;
                                        }
                                    }
                                }
                                // merge groups if neccessary
                                for (int k = 0; k < monitoringPostsWithData.Count(); k++)
                                {
                                    for (int i = 0; i < monitoringPostsWithData.Count(); i++)
                                    {
                                        for (int j = 0; j < monitoringPostsWithData.Count(); j++)
                                        {
                                            if (i != j && groups[i, 0] != groups[j, 0])
                                            {
                                                MonitoringPost monitoringPosti = monitoringPostsWithData.FirstOrDefault(m => m.Id == groups[i, 1]),
                                                    monitoringPostj = monitoringPostsWithData.FirstOrDefault(m => m.Id == groups[j, 1]);
                                                decimal distance = (decimal)Math.Sqrt(Math.Pow((double)(monitoringPosti.NorthLatitude - monitoringPostj.NorthLatitude), 2) +
                                                    Math.Pow((double)(monitoringPosti.EastLongitude - monitoringPostj.EastLongitude), 2));
                                                if (distance <= MaxDistance)
                                                {
                                                    int groupOld = groups[j, 0],
                                                        groupNew = groups[i, 0];
                                                    for (int l = 0; l < monitoringPostsWithData.Count(); l++)
                                                    {
                                                        if (groups[l, 0] == groupOld)
                                                        {
                                                            groups[l, 0] = groupNew;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                int layersCount = 0;
                                for (int i = 1; i <= groupsCount; i++)
                                {
                                    int postsCount = 0;
                                    List<int> monitoringPostsIds = new List<int>();
                                    for (int j = 0; j < monitoringPostsWithData.Count(); j++)
                                    {
                                        if (groups[j, 0] == i)
                                        {
                                            postsCount++;
                                            monitoringPostsIds.Add(groups[j, 1]);
                                        }
                                    }
                                    // create layer
                                    if (postsCount >= MinPostsCount)
                                    {
                                        layersCount++;
                                        string data = "xdd,ydd,value";
                                        string layerName = LayerNameTemplate + "_" + measuredParameter.Id.ToString() + "_" + dateTime.ToString("yyyyMMddHHmmss") + "_" + layersCount.ToString();
                                        // create csv
                                        using (var file = new StreamWriter("CSVTIFF/CSV.csv", true))
                                        {
                                            file.WriteLine("xdd,ydd,value");
                                            foreach(MeasuredData measuredData in measuredDatas)
                                            {
                                                if(monitoringPostsIds.Contains((int)measuredData.MonitoringPostId))
                                                {
                                                    MonitoringPost monitoringPost = monitoringPostsWithData.FirstOrDefault(m => m.Id == measuredData.MonitoringPostId);
                                                    decimal valueMPC = (decimal)measuredData.Value / (decimal)measuredParameters.FirstOrDefault(m => m.Id == measuredData.MeasuredParameterId).MPC;
                                                    file.WriteLine($"{monitoringPost.EastLongitude.ToString()},{monitoringPost.NorthLatitude.ToString()}," +
                                                        $"{(valueMPC).ToString()}");

                                                    data += Environment.NewLine + $"{monitoringPost.EastLongitude.ToString()},{monitoringPost.NorthLatitude.ToString()}," +
                                                        $"{(valueMPC).ToString()}";
                                                }
                                            }
                                        }
                                        // create shp
                                        Process processSHP = new Process();
                                        ProcessStartInfo startInfoSHP = new ProcessStartInfo();
                                        startInfoSHP.FileName = "cmd.exe";
                                        startInfoSHP.WorkingDirectory = @"CSVTIFF";
                                        startInfoSHP.Arguments = "/c ogr2ogr SHP.shp CSV.vrt";
                                        processSHP.StartInfo = startInfoSHP;

                                        processSHP.StartInfo.UseShellExecute = false;
                                        processSHP.StartInfo.RedirectStandardOutput = false;
                                        processSHP.StartInfo.RedirectStandardError = false;
                                        processSHP.StartInfo.CreateNoWindow = true;

                                        processSHP.Start();
                                        processSHP.WaitForExit();
                                        // create tiff
                                        Process processTIFF = new Process();
                                        ProcessStartInfo startInfoTIFF = new ProcessStartInfo();
                                        startInfoTIFF.FileName = "cmd.exe";
                                        startInfoTIFF.WorkingDirectory = @"CSVTIFF";
                                        startInfoTIFF.Arguments = $"/c python invdistgis.py -zfield value SHP.shp {layerName}.tiff";
                                        processTIFF.StartInfo = startInfoTIFF;

                                        processTIFF.StartInfo.UseShellExecute = false;
                                        processTIFF.StartInfo.RedirectStandardOutput = false;
                                        processTIFF.StartInfo.RedirectStandardError = false;
                                        processTIFF.StartInfo.CreateNoWindow = true;

                                        processTIFF.Start();
                                        processTIFF.WaitForExit();
                                        //// create hull
                                        //Process processHULL = new Process();
                                        //ProcessStartInfo startInfoHULL = new ProcessStartInfo();
                                        //startInfoHULL.FileName = "cmd.exe";
                                        //startInfoHULL.WorkingDirectory = @"CSVTIFF";
                                        //startInfoHULL.Arguments = $"/c python hull.py";
                                        //processHULL.StartInfo = startInfoHULL;

                                        //processHULL.StartInfo.UseShellExecute = false;
                                        //processHULL.StartInfo.RedirectStandardOutput = false;
                                        //processHULL.StartInfo.RedirectStandardError = false;
                                        //processHULL.StartInfo.CreateNoWindow = true;

                                        //processHULL.Start();
                                        //processHULL.WaitForExit();
                                        //// cut
                                        //Process processCUT = new Process();
                                        //ProcessStartInfo startInfoCUT = new ProcessStartInfo();
                                        //startInfoCUT.FileName = "cmd.exe";
                                        //startInfoCUT.WorkingDirectory = @"CSVTIFF";
                                        //startInfoCUT.Arguments = $"/c gdalwarp -cutline MaskExtent.shp -crop_to_cutline -dstalpha {layerName}.tiff OUTPUT.tif";
                                        //processCUT.StartInfo = startInfoCUT;

                                        //processCUT.StartInfo.UseShellExecute = false;
                                        //processCUT.StartInfo.RedirectStandardOutput = false;
                                        //processCUT.StartInfo.RedirectStandardError = false;
                                        //processCUT.StartInfo.CreateNoWindow = true;

                                        //processCUT.Start();
                                        //processCUT.WaitForExit();
                                        // delete csv, shp
                                        File.Delete(@"CSVTIFF/CSV.csv");
                                        var dir = new DirectoryInfo("CSVTIFF");
                                        foreach (var file in dir.EnumerateFiles("SHP.*"))
                                        {
                                            file.Delete();
                                        }
                                        // copy tiff to GeoServer
                                        File.Move($"CSVTIFF/{layerName}.tiff", $"{GSDataDir}/{layerName}.tiff");
                                        // publish layer
                                        Process processGS1 = CurlExecuteFalse($" -u " +
                                            $"{GSUser}:" +
                                            $"{GSPassword}" +
                                            $" -POST -H" +
                                            $" \"Content-type: text/xml\"" +
                                            $" -d \"<coverageStore><name>{layerName}</name><type>GeoTIFF</type><enabled>true</enabled><workspace>{GSWorkspace}</workspace>" +
                                            $"<url>/data/{GSWorkspace}/{LayerNameTemplate}/{layerName}.tiff</url></coverageStore>\"" +
                                            $" http://{GSAddress}:" +
                                            $"{GSPort}/geoserver/rest/workspaces/{GSWorkspace}/coveragestores?configure=all");
                                        processGS1.WaitForExit();

                                        Process processGS2 = CurlExecuteFalse($" -u " +
                                            $"{GSUser}:" +
                                            $"{GSPassword}" +
                                            $" -PUT -H" +
                                            $" \"Content-type: text/xml\"" +
                                            $" -d \"<coverage><name>{layerName}</name><title>{layerName}</title>" +
                                            $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></coverage>\"" +
                                            $" http://{GSAddress}:" +
                                            $"{GSPort}/geoserver/rest/workspaces/{GSWorkspace}/coveragestores/{layerName}/coverages?recalculate=nativebbox");
                                        //processGS2.Start();
                                        processGS2.WaitForExit();

                                        Process processGS3 = CurlExecuteFalse($" -u " +
                                            $"{GSUser}:" +
                                            $"{GSPassword}" +
                                            $" -X PUT -H" +
                                            $" \"Content-type: text/xml\"" +
                                            $" -d \"<layer><defaultStyle><name>{GSWorkspace}:{GSStyle}</name></defaultStyle></layer>\"" +
                                            $" http://{GSAddress}:" +
                                            $"{GSPort}/geoserver/rest/layers/{GSWorkspace}:{layerName}");
                                        //processGS3.Start();
                                        processGS3.WaitForExit();
                                        // Add log
                                        DateTime dateTimeServer = DateTime.Now,
                                            dateTimeLayer = dateTime;
                                        int group = layersCount;
                                        Console.WriteLine($"{DateTime.Now.ToString()} >> Layer {layerName}.tiff was published! {Environment.NewLine}");
                                        using (var connection = new NpgsqlConnection("Host=localhost;Database=Layers;Username=postgres;Password=postgres"))
                                        {
                                            connection.Open();
                                            string execute = $"INSERT INTO public.\"Log\"(" +
                                                $"\"DateTimeServer\"," +
                                                $"\"DateTimeLayer\"," +
                                                $"\"LayerName\"," +
                                                $"\"Group\"," +
                                                $"\"Data\"," +
                                                $"\"MeasuredParameterId\") " +
                                                $"VALUES (" +
                                                $"make_timestamptz(" +
                                                    $"{dateTimeServer.Year.ToString()}, " +
                                                    $"{dateTimeServer.Month.ToString()}, " +
                                                    $"{dateTimeServer.Day.ToString()}, " +
                                                    $"{dateTimeServer.Hour.ToString()}, " +
                                                    $"{dateTimeServer.Minute.ToString()}, " +
                                                    $"{dateTimeServer.Second.ToString()})," +
                                                $"make_timestamptz(" +
                                                    $"{dateTimeLayer.Year.ToString()}, " +
                                                    $"{dateTimeLayer.Month.ToString()}, " +
                                                    $"{dateTimeLayer.Day.ToString()}, " +
                                                    $"{dateTimeLayer.Hour.ToString()}, " +
                                                    $"{dateTimeLayer.Minute.ToString()}, " +
                                                    $"{dateTimeLayer.Second.ToString()})," +
                                                $"'{layerName}'," +
                                                $"{group}," +
                                                $"'{data}'," +
                                                $"'{measuredParameter.Id}');";
                                            connection.Execute(execute);
                                            connection.Close();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                Thread.Sleep(60000);
            }
        }

        private static Process CurlExecute(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = CurlFullPath;
                process.StartInfo.Arguments = Arguments;
                process.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        private static Process CurlExecuteFalse(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.FileName = CurlFullPath;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.CreateNoWindow = true;
                //process.OutputDataReceived += (s, e) => Test(e.Data);
                process.Start();
                //process.BeginOutputReadLine();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        public static void Test(string input)
        {
            //input.Dump();
        }

        public static bool LayerExists(string Layer)
        {
            try
            {
                //Process process = CurlExecute($" -u " +
                //    $"{GSUser}:" +
                //    $"{GSPassword}" +
                //    $" -XGET" +
                //    $" http://{GSAddress}:" +
                //    $"{GSPort}/geoserver/rest/layers/{GSWorkspace}:{Layer}.json");
                //string json = process.StandardOutput.ReadToEnd();
                //process.WaitForExit();

                //if(json.Contains("No such layer"))
                //{
                //    return false;
                //}
                //return true;
                return File.Exists(Path.Combine(GSDataDir, Path.ChangeExtension(Layer, ".tiff")));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public static void NewLog(string Log)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} >> {Log}{Environment.NewLine}");
            using (var connection = new NpgsqlConnection("Host=localhost;Database=Layers;Username=postgres;Password=postgres"))
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
    }
}

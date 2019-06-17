using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPListener
{
    public class MonitoringPost
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public decimal NorthLatitude { get; set; }
        public decimal EastLongitude { get; set; }
        public string AdditionalInformation { get; set; }

        public string MN { get; set; }

        public int DataProviderId { get; set; }

        public int PollutionEnvironmentId { get; set; }
    }
    public class MeasuredParameter
    {
        public int Id { get; set; }
        public string NameKK { get; set; }
        public string NameRU { get; set; }
        public string NameEN { get; set; }
        public int? EcomonCode { get; set; }
        public string OceanusCode { get; set; }
        public decimal? MPC { get; set; } // maximum permissible concentration
    }
    class Program
    {
        const int PORT_NO = 8088;
        const int EcoserviceDataProviderId = 3;

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT_NO);
            TcpListener listener = new TcpListener(iPEndPoint);

            while (true)
            {
                try
                {
                    Console.WriteLine("Listening...");
                    listener.Start();
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                    string dataReceived = client.Client.RemoteEndPoint.ToString(),
                        dataReceivedLog = DateTime.Now.ToString()
                        + " >> "
                        + dataReceived
                        + " >> "
                        + Encoding.ASCII.GetString(buffer, 0, bytesRead)
                        + Environment.NewLine;
                    Console.WriteLine("Received : " + dataReceived);
                    dataReceived += Environment.NewLine;
                    client.Close();
                    listener.Stop();
                    File.AppendAllText("log.txt", dataReceived);

                    //string dataReceived = "##0265ST=27;CN=2011;QN=20400101191000000;PW=123456;MN=ESBEIJINAQI002;Flag=5;CP=&&DataTime=20400101191000;o31104-Rtd=0;a21004-Rtd=0;a21026-Rtd=0;a21005-Rtd=0;a34004-Rtd=0;a34002-Rtd=0;a34001-Rtd=0;a01001-Rtd=0;a01002-Rtd=0;LA-Rtd=0;a01006-Rtd=0;a01007-Rtd=0;a01008-Rtd=0&&2700";

                    string[] data = dataReceived.Split(';');
                    string MN = data
                        .FirstOrDefault(d => d.Contains("MN"))
                        ?.Replace("MN=", "");
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
                    {
                        connection.Open();
                        var monitoringPosts = connection.Query<MonitoringPost>($"SELECT \"Id\", \"Number\", \"Name\", \"NorthLatitude\", \"EastLongitude\", \"AdditionalInformation\", \"DataProviderId\", \"PollutionEnvironmentId\", \"MN\" " +
                            $"FROM public.\"MonitoringPost\" " +
                            $"WHERE \"DataProviderId\" = {EcoserviceDataProviderId.ToString()} AND \"MN\" = '{MN}';");
                        MonitoringPost monitoringPost = monitoringPosts.FirstOrDefault();

                        var measuredParameters = connection.Query<MeasuredParameter>("SELECT \"Id\", \"NameKK\", \"NameRU\", \"NameEN\", \"OceanusCode\" FROM public.\"MeasuredParameter\";");

                        foreach (string dataString in data.Where(d => d.Contains("-Rtd")))
                        {
                            string OceanusCode = dataString.Remove(dataString.IndexOf("-Rtd"));
                            MeasuredParameter measuredParameter = measuredParameters
                                .FirstOrDefault(m => m.OceanusCode == OceanusCode);
                            if (measuredParameter != null)
                            {
                                string value = dataString.Split('=')[1].Split("&&")[0],
                                    datetime = data.FirstOrDefault(d => d.Contains("DataTime"))?.Split('=').Last(),
                                    year = datetime.Substring(0, 4),
                                    month = datetime.Substring(4, 2),
                                    day = datetime.Substring(6, 2),
                                    hour = datetime.Substring(8, 2),
                                    minute = datetime.Substring(10, 2),
                                    second = datetime.Substring(12, 2);
                                string insertQuery = $"INSERT INTO public.\"MeasuredData\"(" +
                                    $"\"MeasuredParameterId\", " +
                                    $"\"DateTime\", " +
                                    $"\"Value\", " +
                                    $"\"MonitoringPostId\") " +
                                    $"VALUES ({measuredParameter.Id.ToString()}, " +
                                    $"make_timestamptz(" +
                                        $"{year}, " +
                                        $"{month}, " +
                                        $"{day}, " +
                                        $"{hour}, " +
                                        $"{minute}, " +
                                        $"{second}), " +
                                    $"{value}, " +
                                    $"{monitoringPost.Id.ToString()});";
                                connection.Execute(insertQuery);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error : " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }
        }
    }
}

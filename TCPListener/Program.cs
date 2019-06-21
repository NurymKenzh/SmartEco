//using Dapper;
//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace TCPListener
//{
//    public class MonitoringPost
//    {
//        public int Id { get; set; }
//        public int Number { get; set; }
//        public string Name { get; set; }
//        public decimal NorthLatitude { get; set; }
//        public decimal EastLongitude { get; set; }
//        public string AdditionalInformation { get; set; }

//        public string MN { get; set; }

//        public int DataProviderId { get; set; }

//        public int PollutionEnvironmentId { get; set; }
//    }
//    public class MeasuredParameter
//    {
//        public int Id { get; set; }
//        public string NameKK { get; set; }
//        public string NameRU { get; set; }
//        public string NameEN { get; set; }
//        public int? EcomonCode { get; set; }
//        public string OceanusCode { get; set; }
//        public decimal? MPC { get; set; } // maximum permissible concentration
//    }
//    class Program
//    {
//        const int PORT_NO = 8088;
//        const int EcoserviceDataProviderId = 3;

//        static void Main(string[] args)
//        {
//            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT_NO);
//            TcpListener listener = new TcpListener(iPEndPoint);

//            while (true)
//            {
//                try
//                {
//                    Console.WriteLine("Listening...");
//                    listener.Start();
//                    TcpClient client = listener.AcceptTcpClient();
//                    NetworkStream nwStream = client.GetStream();
//                    byte[] buffer = new byte[client.ReceiveBufferSize];
//                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
//                    string dataReceived = client.Client.RemoteEndPoint.ToString(),
//                        dataReceivedLog = DateTime.Now.ToString()
//                        + " >> "
//                        + dataReceived
//                        + " >> "
//                        + Encoding.ASCII.GetString(buffer, 0, bytesRead)
//                        + Environment.NewLine;
//                    Console.WriteLine("Received : " + dataReceivedLog);
//                    dataReceivedLog += Environment.NewLine;
//                    client.Close();
//                    listener.Stop();
//                    File.AppendAllText("log.txt", dataReceivedLog);

//                    //string dataReceived = "##0265ST=27;CN=2011;QN=20400101191000000;PW=123456;MN=ESBEIJINAQI002;Flag=5;CP=&&DataTime=20400101191000;o31104-Rtd=0;a21004-Rtd=0;a21026-Rtd=0;a21005-Rtd=0;a34004-Rtd=0;a34002-Rtd=0;a34001-Rtd=0;a01001-Rtd=0;a01002-Rtd=0;LA-Rtd=0;a01006-Rtd=0;a01007-Rtd=0;a01008-Rtd=0&&2700";

//                    string[] data = dataReceived.Split(';');
//                    string MN = data
//                        .FirstOrDefault(d => d.Contains("MN"))
//                        ?.Replace("MN=", "");
//                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoAPI;Username=postgres;Password=postgres"))
//                    {
//                        connection.Open();
//                        var monitoringPosts = connection.Query<MonitoringPost>($"SELECT \"Id\", \"Number\", \"Name\", \"NorthLatitude\", \"EastLongitude\", \"AdditionalInformation\", \"DataProviderId\", \"PollutionEnvironmentId\", \"MN\" " +
//                            $"FROM public.\"MonitoringPost\" " +
//                            $"WHERE \"DataProviderId\" = {EcoserviceDataProviderId.ToString()} AND \"MN\" = '{MN}';");
//                        MonitoringPost monitoringPost = monitoringPosts.FirstOrDefault();

//                        var measuredParameters = connection.Query<MeasuredParameter>("SELECT \"Id\", \"NameKK\", \"NameRU\", \"NameEN\", \"OceanusCode\" FROM public.\"MeasuredParameter\";");

//                        foreach (string dataString in data.Where(d => d.Contains("-Rtd")))
//                        {
//                            string OceanusCode = dataString.Remove(dataString.IndexOf("-Rtd"));
//                            MeasuredParameter measuredParameter = measuredParameters
//                                .FirstOrDefault(m => m.OceanusCode == OceanusCode);
//                            if (measuredParameter != null)
//                            {
//                                string value = dataString.Split('=')[1].Split("&&")[0],
//                                    datetime = data.FirstOrDefault(d => d.Contains("DataTime"))?.Split('=').Last(),
//                                    year = datetime.Substring(0, 4),
//                                    month = datetime.Substring(4, 2),
//                                    day = datetime.Substring(6, 2),
//                                    hour = datetime.Substring(8, 2),
//                                    minute = datetime.Substring(10, 2),
//                                    second = datetime.Substring(12, 2);
//                                string insertQuery = $"INSERT INTO public.\"MeasuredData\"(" +
//                                    $"\"MeasuredParameterId\", " +
//                                    $"\"DateTime\", " +
//                                    $"\"Value\", " +
//                                    $"\"MonitoringPostId\") " +
//                                    $"VALUES ({measuredParameter.Id.ToString()}, " +
//                                    $"make_timestamptz(" +
//                                        $"{year}, " +
//                                        $"{month}, " +
//                                        $"{day}, " +
//                                        $"{hour}, " +
//                                        $"{minute}, " +
//                                        $"{second}), " +
//                                    $"{value}, " +
//                                    $"{monitoringPost.Id.ToString()});";
//                                connection.Execute(insertQuery);
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("Error : " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
//                }
//            }
//        }
//    }
//}

using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPIPTestServer
{
    class Program
    {
        const int PORT_NO = 8088;

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT_NO);
            TcpListener serverSocket = new TcpListener(iPEndPoint);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            NewLog("Server Started");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                string[] omitPosts = File.ReadAllLines("Omit Posts.txt");
                string ip = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();
                if (Array.IndexOf(omitPosts, ip) < 0)
                {
                    counter += 1;
                    NewLog($"Client No: {Convert.ToString(counter)} (IP {ip}) connected!");
                    handleClinet client = new handleClinet();
                    client.startClient(clientSocket, Convert.ToString(counter));
                }
            }
        }

        public class handleClinet
        {
            TcpClient clientSocket;
            string clNo;
            public void startClient(TcpClient inClientSocket, string clineNo)
            {
                this.clientSocket = inClientSocket;
                this.clNo = clineNo;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }
            private void doChat()
            {
                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;

                Byte[] sendBytes = null;
                string serverResponse = null;
                string rCount = null;

                bool ok = true;
                while ((ok))
                {
                    try
                    {
                        NetworkStream networkStream = clientSocket.GetStream();
                        int bytesRead = networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        dataFromClient = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead);
                        if (dataFromClient.Contains("MN="))
                        {
                            NewData(dataFromClient, ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString(), clNo);
                        }

                        serverResponse = "Server to clinet(" + clNo + ") ";
                        sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " >> " + ex.ToString() + Environment.NewLine);
                        ok = false;
                    }
                }
            }
        }

        public static void NewLog(string Log)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} >> {Log}{Environment.NewLine}");
            using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
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

        public static void NewData(string Data, string IP, string ClientNumber)
        {
            NewLog($"Data from {ClientNumber} (IP {IP}) >> {Data}");
            using (var connection = new NpgsqlConnection("Host=localhost;Database=PostsData;Username=postgres;Password=postgres"))
            {
                connection.Open();
                DateTime now = DateTime.Now;
                string[] data = Data.Split(';');
                string datetime = data.FirstOrDefault(d => d.Contains("DataTime"))?.Split('=').Last(),
                    year = datetime.Substring(0, 4),
                    month = datetime.Substring(4, 2),
                    day = datetime.Substring(6, 2),
                    hour = datetime.Substring(8, 2),
                    minute = datetime.Substring(10, 2),
                    second = datetime.Substring(12, 2),
                MN = data
                    .FirstOrDefault(d => d.Contains("MN"))?
                    .Replace("MN=", ""),
                dateTimePostS = "null";
                DateTime? dateTimePost = null;
                try
                {
                    dateTimePost = new DateTime(Convert.ToInt32(year),
                    Convert.ToInt32(month),
                    Convert.ToInt32(day),
                    Convert.ToInt32(hour),
                    Convert.ToInt32(minute),
                    Convert.ToInt32(second));
                }
                catch
                {

                }
                if (dateTimePost != null)
                {
                    dateTimePostS = $"make_timestamptz({dateTimePost.Value.Year}," +
                        $"{dateTimePost.Value.Month}," +
                        $"{dateTimePost.Value.Day}," +
                        $"{dateTimePost.Value.Hour}," +
                        $"{dateTimePost.Value.Minute}," +
                        $"{dateTimePost.Value.Second})";
                }

                string execute = $"INSERT INTO public.\"Data\"(" +
                    $"\"Data\"," +
                    $"\"DateTimeServer\"," +
                    $"\"DateTimePost\"," +
                    $"\"MN\"," +
                    $"\"IP\") " +
                    $"VALUES ('{Data}'," +
                    $"make_timestamptz(" +
                        $"{now.Year.ToString()}, " +
                        $"{now.Month.ToString()}, " +
                        $"{now.Day.ToString()}, " +
                        $"{now.Hour.ToString()}, " +
                        $"{now.Minute.ToString()}, " +
                        $"{now.Second.ToString()})," +
                    $"{dateTimePostS}," +
                    $"'{MN}'," +
                    $"'{IP}'" +
                    $");";
                connection.Execute(execute);
                connection.Close();
            }
        }
    }
}

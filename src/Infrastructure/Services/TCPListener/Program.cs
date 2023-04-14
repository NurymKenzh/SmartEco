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
                        //Console.WriteLine(DateTime.Now.ToString() + " >> " + ex.ToString() + Environment.NewLine);
                        NewLog($"Error - {clNo} (IP {((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString()}) >> {ex.ToString()}");
                        ok = false;
                    }
                }
            }
        }

        public static void NewLog(string Log)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} >> {Log}{Environment.NewLine}");
            try
            {
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
            catch
            {

            }
        }

        public static void NewData(string Data, string IP, string ClientNumber)
        {
            NewLog($"Data from {ClientNumber} (IP {IP}) >> {Data}");
            try
            {
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
            catch
            {

            }
        }
    }
}

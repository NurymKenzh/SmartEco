using System;
using System.IO;
using System.Net;

namespace YandexWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://api.weather.yandex.ru/v1/forecast?lat=55.75396&lon=37.620393"));
            WebReq.Method = "GET";
            WebReq.Headers.Add("X-Yandex-API-Key", "8997ff67-00e3-4f0a-8c88-f7b72f90f2c6");
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            Console.WriteLine(WebResp.StatusCode);
            Console.WriteLine(WebResp.Server);
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            Console.WriteLine(jsonString);

            Console.ReadLine();
        }
    }
}

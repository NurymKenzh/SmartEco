using NDbfReader;
using System;
using System.Text;
using System.Threading;

namespace AutoEcoPostsData
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Program started!");
            while (true)
            {
                //var path = "D:/Job/KAZ/Files/AutoEcoPosts/AUTOTEST/Db/data.dbf";
                var path = "D:/Job/KAZ/Files/AutoEcoPosts/Dymomer/Db/data.dbf";
                using (var table = Table.Open(path))
                {
                    var reader = table.OpenReader(Encoding.GetEncoding(1251));
                    while (reader.Read())
                    {
                        foreach (var column in table.Columns)
                        {
                            var col = column.Name;
                            var value = reader.GetValue(column);
                        }
                    }
                }

                Thread.Sleep(60000);
            }
        }
    }
}

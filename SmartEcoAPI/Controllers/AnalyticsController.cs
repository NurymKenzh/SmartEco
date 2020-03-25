using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OfficeOpenXml;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        const string Heading = "Аналитика по постам",
            Theme = "SmartEco",
            FromEmail = "smartecokz@gmail.com",
            Password = "Qwerty123_",
            SMTPServer = "smtp.gmail.com";
        const int SMTPPort = 465;

        //public int COMPCDivide = 1; // было 10  // Id = 7
        //public decimal? PValueMultiply = 0.750063755419211m, // Id = 1
        //    NO2ValueMultiply = 0.001m, // Id = 13
        //    SO2ValueMultiply = 0.001m, // Id = 9
        //    H2SValueMultiply = 0.001m, // Id = 20
        //    PM10ValueMultiply = 0.001m, // Id = 2
        //    PM25ValueMultiply = 0.001m; // Id = 3
        public string sName = "PostsAnalytics",
            PathExcelFile = "C:\\inetpub\\wwwroot";

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Kazakhtelecom,Arys")]
        public async Task<ActionResult> ExcelFormation(
            DateTime DateTimeFrom,
            DateTime DateTimeTo,
            bool Server)
        {
            var measuredDatas = GetMeasuredData(DateTimeFrom, DateTimeTo, true).ToList();
            measuredDatas.Where(m => !string.IsNullOrEmpty(m.MonitoringPost.MN) && !string.IsNullOrEmpty(m.MeasuredParameter.OceanusCode)).ToList();

            string sFileName = $"{sName}.xlsx";
            FileInfo file = Server == true ? new FileInfo(Path.Combine(PathExcelFile, sFileName)) : new FileInfo(Path.Combine(sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sName);
                worksheet.Cells[1, 1].Value = "Дата";
                worksheet.Cells["A1:B1"].Merge = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = "C";
                worksheet.Cells[2, 2].Value = "По";
                worksheet.Cells[3, 1].Value = DateTimeFrom.ToShortDateString();
                worksheet.Cells[3, 2].Value = DateTimeTo.ToShortDateString();

                worksheet.Cells[5, 1].Value = "Количество данных по постам";
                worksheet.Cells[5, 2].Value = measuredDatas.Count;

                worksheet.Cells[7, 1].Value = "Превышения ПДК";
                worksheet.Cells["A7:E7"].Merge = true;
                worksheet.Cells[7, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[8, 1].Value = "Пост";
                worksheet.Cells[8, 2].Value = "Измеряемый параметр";
                worksheet.Cells[8, 3].Value = "Дата и время";
                worksheet.Cells[8, 4].Value = "Значение";
                worksheet.Cells[8, 5].Value = "ПДК";

                int row = 9;
                foreach (var measuredData in measuredDatas)
                {
                    if (measuredData.MeasuredParameter.MPCMaxSingle != null)
                    {
                        if (measuredData.Value > measuredData.MeasuredParameter.MPCMaxSingle)
                        {
                            worksheet.Cells[row, 1].Value = measuredData.MonitoringPost.AdditionalInformation;
                            worksheet.Cells[row, 2].Value = measuredData.MeasuredParameter.NameRU;
                            worksheet.Cells[row, 3].Value = measuredData.DateTime.ToString();
                            worksheet.Cells[row, 4].Value = measuredData.Value;
                            worksheet.Cells[row, 5].Value = measuredData.MeasuredParameter.MPCMaxSingle;

                            row++;
                        }
                    }
                }
                row++;

                worksheet.Cells[row, 1].Value = "Средние значения";
                worksheet.Cells[$"A{row}:C{row}"].Merge = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                row++;

                worksheet.Cells[row, 1].Value = "Пост";
                worksheet.Cells[row, 2].Value = "Измеряемый параметр";
                worksheet.Cells[row, 3].Value = "Среднее значение";
                row++;

                var monitoringPosts = _context.MonitoringPost
                    .Include(m => m.DataProvider)
                    .Include(m => m.PollutionEnvironment)
                    .Where(m => true)
                    .ToList();
                monitoringPosts = monitoringPosts.Where(m => !string.IsNullOrEmpty(m.MN)).ToList();

                var measuredParameters = _context.MeasuredParameter
                    .Include(m => m.MeasuredParameterUnit)
                    .Where(m => true)
                    .ToList();
                measuredParameters = measuredParameters.Where(m => !string.IsNullOrEmpty(m.OceanusCode)).ToList();

                List<decimal> values;
                foreach (var monitoringPost in monitoringPosts)
                {
                    foreach (var measuredParameter in measuredParameters)
                    {
                        values = new List<decimal>();
                        foreach (var measuredData in measuredDatas)
                        {
                            if (monitoringPost.Id == measuredData.MonitoringPostId && measuredParameter.Id == measuredData.MeasuredParameterId)
                            {
                                values.Add(Convert.ToDecimal(measuredData.Value));
                            }
                        }
                        decimal sum = 0;
                        foreach (var val in values)
                        {
                            sum += val;
                        }
                        if (sum != 0)
                        {
                            decimal value = sum / values.Count;
                            worksheet.Cells[row, 1].Value = monitoringPost.AdditionalInformation;
                            worksheet.Cells[row, 2].Value = measuredParameter.NameRU;
                            worksheet.Cells[row, 3].Value = value;
                            row++;
                        }
                    }
                }

                for (int i = 1; i < 6; i++)
                {
                    //worksheet.Cells[1, i].Style.Font.Bold = true;
                    worksheet.Column(i).AutoFit();
                }
                package.Save();

                string userEmail = User.Identity.Name;
                Task.WaitAll(SendExcel(userEmail, Server));
                System.IO.File.Delete(Server == true ? Path.Combine(PathExcelFile, sFileName) : Path.Combine(sFileName));
            }
            return null;
        }

        // POST: api/Analytics/ExcelFormationAlmaty
        [HttpPost("ExcelFormationAlmaty")]
        [Authorize(Roles = "admin,moderator,Almaty")]
        public async Task<ActionResult> ExcelFormationAlmaty(
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            [FromQuery(Name = "MonitoringPostsId")] List<string> MonitoringPostsId,
            bool Server)
        {
            string sFileName = $"{sName}.xlsx";
            FileInfo file = Server == true ? new FileInfo(Path.Combine(PathExcelFile, sFileName)) : new FileInfo(Path.Combine(sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sName);

                //Заголовок
                worksheet.Cells[3, 1].Value = $"Отчёт по мониторингу качества атмосферного воздуха";
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[3, 1].Style.Font.Size = 16;

                worksheet.Cells[4, 1].Value = $"За период с {DateTimeFrom.Value.ToShortDateString()} по {DateTimeTo.Value.ToShortDateString()}";
                worksheet.Cells["A4:I4"].Merge = true;
                worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[4, 1].Style.Font.Bold = true;
                worksheet.Cells[4, 1].Style.Font.Size = 16;

                //Запись информации каждого поста
                int row = 5;
                List<MeasuredData> measuredDatasTotal = new List<MeasuredData>(); //Список для итоговой таблицы
                foreach (var monitoringPostId in MonitoringPostsId)
                {
                    row++;
                    var monitoringPost = _context.MonitoringPost
                        .Where(m => m.Id == Convert.ToInt32(monitoringPostId))
                        .ToList();

                    worksheet.Cells[row, 1].Value = $"Станция № {monitoringPost[0].Number}";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row}:D{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    row++;

                    worksheet.Cells[row, 1].Value = $"Название станции: {monitoringPost[0].Name}";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row}:D{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    row++;

                    worksheet.Cells[row, 1].Value = $"Место установки: {monitoringPost[0].AdditionalInformation}";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row}:D{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    row++;

                    worksheet.Cells[row, 1].Value = $"Координаты: {monitoringPost[0].NorthLatitude} с.ш. {monitoringPost[0].EastLongitude} в.д.";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[$"A{row}:D{row}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    row++;

                    row++;
                    worksheet.Cells[row, 1].Value = $"Параметр";
                    worksheet.Cells[row, 2].Value = $"ПДКмр";
                    worksheet.Cells[row, 3].Value = $"ПДКсс";
                    worksheet.Cells[row, 4].Value = $"Среднее";
                    worksheet.Cells[row, 5].Value = $"Максимальное значение";
                    worksheet.Cells[row, 5].Style.WrapText = true; //Перенос текста
                    worksheet.Cells[row, 6].Value = $"Минимальное";
                    worksheet.Cells[row, 7].Value = $"Количесво превышений ПДК мр";
                    worksheet.Cells[row, 7].Style.WrapText = true;
                    //worksheet.Cells[row, 8].Value = $"Количесво превышений ПДК сс";
                    //worksheet.Cells[row, 8].Style.WrapText = true;
                    worksheet.Cells[row, 8].Value = $"Количество измерений";
                    worksheet.Cells[row, 8].Style.WrapText = true;
                    worksheet.Cells[row, 9].Value = $"Оценка";

                    //Установка стилей для "шапки" таблицы
                    for (int i = 1; i < 10; i++)
                    {
                        worksheet.Cells[row, i].Style.Font.Bold = true;
                        worksheet.Cells[row, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    row++;

                    //Параметры, которые включены у постов
                    var monitoringPostMeasuredParameters = _context.MonitoringPostMeasuredParameters
                        .Where(m => m.MonitoringPostId == monitoringPost[0].Id)
                        .OrderBy (m => m.MeasuredParameter.NameRU)
                        .Include(m => m.MeasuredParameter)
                        .ToList();
                    foreach (var monitoringPostMeasuredParameter in monitoringPostMeasuredParameters)
                    {
                        var measuredDatasv = _context.MeasuredData
                            .Where(m => m.MonitoringPostId == monitoringPost[0].Id && m.MeasuredParameterId == monitoringPostMeasuredParameter.MeasuredParameterId);
                        measuredDatasv = measuredDatasv.Where(m => (m.DateTime != null && m.DateTime >= DateTimeFrom) ||
                            (m.Year != null && m.Month == null && m.Year >= DateTimeFrom.Value.Year) ||
                            (m.Year != null && m.Month != null && m.Year >= DateTimeFrom.Value.Year && m.Month >= DateTimeFrom.Value.Month));
                        measuredDatasv = measuredDatasv.Where(m => (m.DateTime != null && m.DateTime <= DateTimeTo) ||
                            (m.Year != null && m.Month == null && m.Year <= DateTimeTo.Value.Year) ||
                            (m.Year != null && m.Month != null && m.Year <= DateTimeTo.Value.Year && m.Month <= DateTimeTo.Value.Month));
                        var measuredDatas = measuredDatasv.ToList();
                        measuredDatasTotal.AddRange(measuredDatas); //Запись данных в общий список

                        worksheet.Cells[row, 1].Value = $"{monitoringPostMeasuredParameter.MeasuredParameter.NameRU}";
                        worksheet.Cells[row, 2].Value = $"{monitoringPostMeasuredParameter.MeasuredParameter.MPCMaxSingle}";
                        worksheet.Cells[row, 3].Value = $"{monitoringPostMeasuredParameter.MeasuredParameter.MPCDailyAverage}";

                        //Запись среднего, максимального, минимального
                        if (measuredDatas.Count != 0)
                        {
                            worksheet.Cells[row, 4].Value = $"{Math.Round(Convert.ToDecimal(measuredDatas.Sum(m => m.Value) / measuredDatas.Count()), 3)}";
                            //Если параметр не "Направление ветра"
                            if (monitoringPostMeasuredParameter.MeasuredParameter.Id != 6)
                            {
                                worksheet.Cells[row, 5].Value = $"{Math.Round(Convert.ToDecimal(measuredDatas.Max(m => m.Value)), 3)}";
                                worksheet.Cells[row, 6].Value = $"{Math.Round(Convert.ToDecimal(measuredDatas.Min(m => m.Value)), 3)}";
                            }
                        }

                        //Количетсво превышений ПДКмр
                        if (monitoringPostMeasuredParameter.MeasuredParameter.MPCMaxSingle != null && measuredDatas.Count != 0)
                        {
                            int numberExcess = 0;
                            foreach (var measuredDataValue in measuredDatas.Select(m => m.Value))
                            {
                                if (measuredDataValue > monitoringPostMeasuredParameter.MeasuredParameter.MPCMaxSingle)
                                {
                                    numberExcess++;
                                }
                            }
                            worksheet.Cells[row, 7].Value = $"{numberExcess}";
                        }

                        //Количетсво превышений ПДКсс
                        //if (monitoringPostMeasuredParameter.MeasuredParameter.MPCDailyAverage != null && measuredDatas.Count != 0)
                        //{
                        //    int numberExcess = 0;
                        //    foreach (var measuredDataValue in measuredDatas.Select(m => m.Value))
                        //    {
                        //        if (measuredDataValue > monitoringPostMeasuredParameter.MeasuredParameter.MPCDailyAverage)
                        //        {
                        //            numberExcess++;
                        //        }
                        //    }
                        //    worksheet.Cells[row, 8].Value = $"{numberExcess}";
                        //}

                        //Количество измерений
                        if (measuredDatas.Count != 0)
                        {
                            worksheet.Cells[row, 8].Value = $"{measuredDatas.Count}";
                        }

                        //Установка стиля для заполненного ряда
                        for (int i = 1; i < 10; i++)
                        {
                            worksheet.Cells[row, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                        row++;
                    }
                }

                //Итог
                row += 2;
                worksheet.Cells[row, 1].Value = $"Итого по городу Алматы";
                worksheet.Cells[$"A{row}:I{row}"].Merge = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Size = 16;

                //Выбираем различные параметры из общего количества
                List<MeasuredParameter> measuredParametersTotal = new List<MeasuredParameter>();
                measuredParametersTotal.AddRange(measuredDatasTotal
                    .OrderBy(m => m.MeasuredParameter.NameRU)
                    .Select(m => m.MeasuredParameter)
                    .Distinct());

                row += 2;
                worksheet.Cells[row, 1].Value = $"Параметр";
                worksheet.Cells[row, 2].Value = $"ПДКмр";
                worksheet.Cells[row, 3].Value = $"ПДКсс";
                worksheet.Cells[row, 4].Value = $"Среднее";
                worksheet.Cells[row, 5].Value = $"Максимальное значение";
                worksheet.Cells[row, 5].Style.WrapText = true;
                worksheet.Cells[row, 6].Value = $"Минимальное";
                worksheet.Cells[row, 7].Value = $"Количесво превышений ПДК мр";
                worksheet.Cells[row, 7].Style.WrapText = true;
                //worksheet.Cells[row, 8].Value = $"Количесво превышений ПДК сс";
                //worksheet.Cells[row, 8].Style.WrapText = true;
                worksheet.Cells[row, 8].Value = $"Количество измерений";
                worksheet.Cells[row, 8].Style.WrapText = true;
                worksheet.Cells[row, 9].Value = $"Оценка";

                for (int i = 1; i < 10; i++)
                {
                    worksheet.Cells[row, i].Style.Font.Bold = true;
                    worksheet.Cells[row, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
                row++;

                foreach (var measuredParameter in measuredParametersTotal)
                {
                    var measuredDatasForParameter = measuredDatasTotal.Where(m => m.MeasuredParameterId == measuredParameter.Id).ToList();

                    worksheet.Cells[row, 1].Value = $"{measuredParameter.NameRU}";
                    worksheet.Cells[row, 2].Value = $"{measuredParameter.MPCMaxSingle}";
                    worksheet.Cells[row, 3].Value = $"{measuredParameter.MPCDailyAverage}";

                    //Запись среднего, максимального, минимального
                    if (measuredDatasForParameter.Count != 0)
                    {
                        worksheet.Cells[row, 4].Value = $"{Math.Round(Convert.ToDecimal(measuredDatasForParameter.Sum(m => m.Value) / measuredDatasForParameter.Count()), 3)}";
                        //Если параметр не "Направление ветра"
                        if (measuredParameter.Id != 6)
                        {
                            worksheet.Cells[row, 5].Value = $"{Math.Round(Convert.ToDecimal(measuredDatasForParameter.Max(m => m.Value)), 3)}";
                            worksheet.Cells[row, 6].Value = $"{Math.Round(Convert.ToDecimal(measuredDatasForParameter.Min(m => m.Value)), 3)}";
                        }

                    }

                    //Количетсво превышений ПДКмр
                    if (measuredParameter.MPCMaxSingle != null && measuredDatasForParameter.Count != 0)
                    {
                        int numberExcess = 0;
                        foreach (var measuredDataValue in measuredDatasForParameter.Select(m => m.Value))
                        {
                            if (measuredDataValue > measuredParameter.MPCMaxSingle)
                            {
                                numberExcess++;
                            }
                        }
                        worksheet.Cells[row, 7].Value = $"{numberExcess}";
                    }

                    //Количетсво превышений ПДКсс
                    //if (measuredParameter.MPCDailyAverage != null && measuredDatasForParameter.Count != 0)
                    //{
                    //    int numberExcess = 0;
                    //    foreach (var measuredDataValue in measuredDatasForParameter.Select(m => m.Value))
                    //    {
                    //        if (measuredDataValue > measuredParameter.MPCDailyAverage)
                    //        {
                    //            numberExcess++;
                    //        }
                    //    }
                    //    worksheet.Cells[row, 8].Value = $"{numberExcess}";
                    //}

                    //Количество измерений
                    if (measuredDatasForParameter.Count != 0)
                    {
                        worksheet.Cells[row, 8].Value = $"{measuredDatasForParameter.Count}";
                    }

                    //Установка стиля для заполненного ряда
                    for (int i = 1; i < 10; i++)
                    {
                        worksheet.Cells[row, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    row++;
                }

                //Автовыравнивание по ширине
                for (int i = 1; i < 10; i++)
                {
                    worksheet.Column(i).AutoFit();
                }

                //Задание ширины для текста, который переносится (значение = ширина в пикселях * 7)
                worksheet.Column(5).Width = 16;
                worksheet.Column(7).Width = 20;
                worksheet.Column(8).Width = 20;
                package.Save();

                string userEmail = User.Identity.Name;
                Task.WaitAll(SendExcel(userEmail, Server));
                System.IO.File.Delete(Server == true ? Path.Combine(PathExcelFile, sFileName) : Path.Combine(sFileName));
            }

            return null;
        }

        private IEnumerable<MeasuredData> GetMeasuredData(
            DateTime? DateTimeFrom,
            DateTime? DateTimeTo,
            bool? Averaged = true)
        {
            var measuredDatas = _context.MeasuredData
                .Include(m => m.MeasuredParameter)
                .Include(m => m.MonitoringPost)
                .Include(m => m.PollutionSource)
                .Where(m => true);

            if (DateTimeFrom != null)
            {
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime >= DateTimeFrom) ||
                    (m.Year != null && m.Month == null && m.Year >= DateTimeFrom.Value.Year) ||
                    (m.Year != null && m.Month != null && m.Year >= DateTimeFrom.Value.Year && m.Month >= DateTimeFrom.Value.Month));
            }
            if (DateTimeTo != null)
            {
                measuredDatas = measuredDatas.Where(m => (m.DateTime != null && m.DateTime <= DateTimeTo) ||
                    (m.Year != null && m.Month == null && m.Year <= DateTimeTo.Value.Year) ||
                    (m.Year != null && m.Month != null && m.Year <= DateTimeTo.Value.Year && m.Month <= DateTimeTo.Value.Month));
            }
            if (Averaged == true)
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == Averaged);
            }
            else
            {
                measuredDatas = measuredDatas.Where(m => m.Averaged == null || m.Averaged == false);
            }

            List<MeasuredData> measuredDatasR = measuredDatas.ToList();
            //measuredDatasR = measuredDatasR
            //    .Select(m =>
            //    {
            //        m.Value =
            //            m.MeasuredParameterId == 7 ? m.Value / COMPCDivide :
            //            m.MeasuredParameterId == 1 ? m.Value * PValueMultiply :
            //            m.MeasuredParameterId == 13 ? m.Value * NO2ValueMultiply :
            //            m.MeasuredParameterId == 9 ? m.Value * SO2ValueMultiply :
            //            m.MeasuredParameterId == 20 ? m.Value * H2SValueMultiply :
            //            m.MeasuredParameterId == 2 ? m.Value * PM10ValueMultiply :
            //            m.MeasuredParameterId == 3 ? m.Value * PM25ValueMultiply :
            //            m.Value;
            //        return m;
            //    })
            //    .ToList();

            return measuredDatasR;
        }

        private async Task SendExcel(string mailTo, bool Server)
        {
            var path = Server == true ? Path.Combine(PathExcelFile, $"{sName}.xlsx") : Path.Combine($"{sName}.xlsx");
            var file = System.IO.File.OpenRead(path);
            var emailMessage = new MimeMessage();

            try
            {
                emailMessage.From.Add(new MailboxAddress(Theme, FromEmail));
                emailMessage.To.Add(new MailboxAddress("", mailTo));
                emailMessage.Subject = Heading;

                // create an pdf attachment for the file located at path
                var attachment = new MimePart("application", "vnd.ms-excel")
                {
                    Content = new MimeContent(file),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(path)
                };

                // create the multipart/mixed container to hold the message text and the pdf attachment
                var multipart = new Multipart("mixed");
                multipart.Add(attachment);

                // set the multipart/mixed as the message body
                emailMessage.Body = multipart;

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SMTPServer, SMTPPort, true);
                    await client.AuthenticateAsync(FromEmail, Password);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }

                file.Close();
            }
            catch (Exception ex)
            {
                file.Close();
            }
        }
    }
}
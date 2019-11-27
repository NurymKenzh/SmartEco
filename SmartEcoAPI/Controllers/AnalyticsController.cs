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

        public int COMPCDivide = 1; // было 10  // Id = 7
        public decimal? PValueMultiply = 0.750063755419211m, // Id = 1
            NO2ValueMultiply = 0.001m, // Id = 13
            SO2ValueMultiply = 0.001m, // Id = 9
            H2SValueMultiply = 0.001m, // Id = 20
            PM10ValueMultiply = 0.001m, // Id = 2
            PM25ValueMultiply = 0.001m; // Id = 3
        public string sName = "PostsAnalytics";

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator,KaragandaRegion,Kazakhtelecom,Arys")]
        public async Task<ActionResult> ExcelFormation(
            DateTime DateTimeFrom,
            DateTime DateTimeTo)
        {
            var measuredDatas = GetMeasuredData(DateTimeFrom, DateTimeTo, true).ToList();
            measuredDatas.Where(m => !string.IsNullOrEmpty(m.MonitoringPost.MN) && !string.IsNullOrEmpty(m.MeasuredParameter.OceanusCode)).ToList();

            string sFileName = $"{sName}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sFileName));
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
                    if (measuredData.MeasuredParameter.MPC != null)
                    {
                        if (measuredData.Value > measuredData.MeasuredParameter.MPC)
                        {
                            worksheet.Cells[row, 1].Value = measuredData.MonitoringPost.AdditionalInformation;
                            worksheet.Cells[row, 2].Value = measuredData.MeasuredParameter.NameRU;
                            worksheet.Cells[row, 3].Value = measuredData.DateTime.ToString();
                            worksheet.Cells[row, 4].Value = measuredData.Value;
                            worksheet.Cells[row, 5].Value = measuredData.MeasuredParameter.MPC;

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
                Task.WaitAll(SendExcel(userEmail));
                System.IO.File.Delete(Path.Combine($"{sName}.xlsx"));
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
            measuredDatasR = measuredDatasR
                .Select(m =>
                {
                    m.Value =
                        m.MeasuredParameterId == 7 ? m.Value / COMPCDivide :
                        m.MeasuredParameterId == 1 ? m.Value * PValueMultiply :
                        m.MeasuredParameterId == 13 ? m.Value * NO2ValueMultiply :
                        m.MeasuredParameterId == 9 ? m.Value * SO2ValueMultiply :
                        m.MeasuredParameterId == 20 ? m.Value * H2SValueMultiply :
                        m.MeasuredParameterId == 2 ? m.Value * PM10ValueMultiply :
                        m.MeasuredParameterId == 3 ? m.Value * PM25ValueMultiply :
                        m.Value;
                    return m;
                })
                .ToList();

            return measuredDatasR;
        }

        private async Task SendExcel(string mailTo)
        {
            try
            {
                var file = System.IO.File.OpenRead($"{sName}.xlsx");
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(Theme, FromEmail));
                emailMessage.To.Add(new MailboxAddress("", mailTo));
                emailMessage.Subject = Heading;

                // create an pdf attachment for the file located at path
                var attachment = new MimePart("application", "vnd.ms-excel")
                {
                    Content = new MimeContent(file),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName($"{sName}.xlsx")
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
            catch
            {

            }
        }
    }
}
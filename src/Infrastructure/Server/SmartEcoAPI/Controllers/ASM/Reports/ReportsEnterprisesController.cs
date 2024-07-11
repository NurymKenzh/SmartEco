using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Reports;
using SmartEcoAPI.Models.ASM.Requests;
using SmartEcoAPI.Models.ASM.Responses.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "admin,moderator,ASM")]
    public class ReportsEnterprisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _baseFileName = "ReportEnterprise",
            _pathExcelFiles = "C:\\Reports\\ASM\\ReportsEnterprises";

        public ReportsEnterprisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ReportsEnterprises
        [HttpGet]
        public async Task<ActionResult<ReportsEnterprisesResponse>> GetReportsEnterprises([FromBody] ReportsEnterpisesRequest request)
        {
            var reportEnterprises = _context.ReportEnterprise
                .Where(m => true);

            if (request.Name != null)
            {
                reportEnterprises = reportEnterprises.Where(r => r.Name.ToLower().Contains(request.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.KatoComplex))
            {
                reportEnterprises = reportEnterprises.Where(r => $"{r.KatoCode} {r.KatoName}".ToLower().Contains(request.KatoComplex.ToLower()));
            }
            if (request.CreatedDate != null)
            {
                reportEnterprises = reportEnterprises.Where(r => r.CreatedDate == request.CreatedDate);
            }

            switch (request.SortOrder)
            {
                case "Name":
                    reportEnterprises = reportEnterprises.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    reportEnterprises = reportEnterprises.OrderByDescending(m => m.Name);
                    break;
                case "CreatedDate":
                    reportEnterprises = reportEnterprises.OrderBy(m => m.CreatedDate);
                    break;
                case "CreatedDateDesc":
                    reportEnterprises = reportEnterprises.OrderByDescending(m => m.CreatedDate);
                    break;
                default:
                    reportEnterprises = reportEnterprises.OrderBy(m => m.Id);
                    break;
            }

            var count = await reportEnterprises.CountAsync();
            if (request.PageSize != null && request.PageNumber != null)
            {
                reportEnterprises = reportEnterprises.Skip(((int)request.PageNumber - 1) * (int)request.PageSize).Take((int)request.PageSize);
            }
            var response = new ReportsEnterprisesResponse(await reportEnterprises.ToListAsync(), count);

            return response;
        }
        
        [HttpGet("{reportEnterpriseId:int}")]
        public async Task<ActionResult<ReportEnterprise>> GetReportEnterprise(int reportEnterpriseId)
        {
            var reportEnterprise = await _context.ReportEnterprise
                .FirstOrDefaultAsync(m => m.Id == reportEnterpriseId);

            if (reportEnterprise == null)
            {
                return NotFound();
            }

            return reportEnterprise;
        }

        [HttpPost("Download/{reportEnterpriseId:int}")]
        public async Task<ActionResult<ReportDownloadResponse>> Download(int reportEnterpriseId)
        {
            var reportEnterprise = await _context.ReportEnterprise.FindAsync(reportEnterpriseId);
            var fileName = reportEnterprise.Name;

            var reportPath = Path.Combine(_pathExcelFiles, fileName);
            var bytes = await System.IO.File.ReadAllBytesAsync(reportPath);

            return new ReportDownloadResponse(bytes, fileName, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }


        // POST: api/ReportsEnterprises
        [HttpPost("Parameters")]
        public async Task<ActionResult> PostParameters(ReportEnterprise reportEnterprise)
        {
            var createdDate = DateTime.Now;
            string fileName = $"{_baseFileName}_Parameters_{createdDate:yyyy-MM-dd_HH-mm-ss}.xlsx";

            reportEnterprise.CreatedDate = createdDate;
            reportEnterprise.Name = fileName;
            CreateReportParametersFile(createdDate, fileName, reportEnterprise.KatoCode);
            _context.ReportEnterprise.Add(reportEnterprise);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Enterprises/5
        [HttpDelete("{reportEnterpriseId:int}")]
        public async Task<ActionResult<ReportEnterprise>> DeleteReportEnterprise(int reportEnterpriseId)
        {
            var reportEnterprise = await _context.ReportEnterprise.FindAsync(reportEnterpriseId);
            if (reportEnterprise == null)
            {
                return NotFound();
            }

            FileInfo file = new FileInfo(Path.Combine(_pathExcelFiles, reportEnterprise.Name));
            if (file.Exists)
                file.Delete();

            _context.ReportEnterprise.Remove(reportEnterprise);
            await _context.SaveChangesAsync();

            return reportEnterprise;
        }

        private void CreateReportParametersFile(DateTime createdDate, string fileName, string katoCode)
        {
            var kato = _context.KatoCatalog
                .FirstOrDefault(k => k.Code == katoCode);

            EnsureDirectoryExists(_pathExcelFiles);
            var file = CreateExcelFile(fileName);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(_baseFileName);

                // Заголовок "SmartEco"
                worksheet.Cells["A1:I1"].Merge = true;
                worksheet.Cells[1, 1].Value = "SmartEco";

                // Заголовок "Параметры выбросов загрязняющих веществ..."
                worksheet.Cells["A2:L2"].Merge = true;
                worksheet.Cells[2, 1].Value = $"Параметры выбросов загрязняющих веществ в атмосферу для расчета нормативов допустимых выбросов на {createdDate.Year}";
                worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Заголовок "общая по предприятим"
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells[3, 1].Value = $"общая по предприятиям {kato?.Name}";

                // Header таблицы
                var headers = new[]
                {
                    ("A4", "A4:A6", "Производство"),
                    ("B4", "B4:B6", "Цех"),
                    ("C4", "C4:D5", "Источник выделения загрязняющих веществ"),
                    ("E4", "E4:E6", "Число часов  работы в году"),
                    ("F4", "F4:F6", "Наименование источника выброса вредных веществ"),
                    ("G4", "G4:G6", "Номер источника выбросов на карте-схеме"),
                    ("H4", "H4:H6", "Высота источника выбросов, м"),
                    ("I4", "I4:I6", "Диаметр устья трубы, м"),
                    ("J4", "J4:L5", "Параметры газовоздушной смеси на выходе из трубы при максимально разовой нагрузке"),
                    ("M4", "M4:P4", "Координаты источника на карте-схеме, м."),
                    ("Q4", "Q4:Q6", "Hаименование газоочистных установок, тип и мероприятия по сокращению выбросов"),
                    ("R4", "R4:R6", "Вещество, по которому производится газоочистка"),
                    ("S4", "S4:S6", "Коэффициент обеспеченности газоочисткой, %"),
                    ("T4", "T4:T6", "Среднеэксплуатационная степень очистки/максимальная степень очистки, %"),
                    ("U4", "U4:U6", "Код вещества"),
                    ("V4", "V4:V6", "Hаименование вещества"),
                    ("W4", "W4:Y5", "Выбросы загрязняющего вещества"),
                    ("Z4", "Z4:Z6", "Год достижения НДВ")
                };

                foreach (var header in headers)
                {
                    worksheet.Cells[header.Item1].Value = header.Item3;
                    worksheet.Cells[header.Item2].Merge = true;
                }

                //Детализация источника выделения загрязняющих веществ
                worksheet.Cells["C6"].Value = $"Наименование";
                worksheet.Cells["D6"].Value = $"Количество, шт.";

                // Детализация параметров газовоздушной смеси
                worksheet.Cells["J6"].Value = "Скорость, м/с";
                worksheet.Cells["K6"].Value = "Объем смеси, м3/с";
                worksheet.Cells["L6"].Value = "Температура смеси, °С";

                // Детализация координат источника на карте-схеме
                worksheet.Cells["M5"].Value = "точ.ист. / 1-го конца линейного источника / центра площадного источника";
                worksheet.Cells["O5"].Value = "2-го конца линейного источника / длина, ширина площадного источника";
                worksheet.Cells["M5:N5"].Merge = true;
                worksheet.Cells["O5:P5"].Merge = true;

                worksheet.Cells["M6"].Value = $"Х1";
                worksheet.Cells["N6"].Value = $"Y1";
                worksheet.Cells["O6"].Value = $"X2";
                worksheet.Cells["P6"].Value = $"Y2";

                // Детализация выбросов загрязняющего вещества
                worksheet.Cells["W6"].Value = "г/с";
                worksheet.Cells["X6"].Value = "мг/нм3";
                worksheet.Cells["Y6"].Value = "т/год";

                //Header style
                worksheet.Cells["A4:Z6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:Z6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells["A4:Z6"].Style.WrapText = true;
                worksheet.Row(4).Height = 30;
                worksheet.Row(5).Height = 60;
                worksheet.Row(6).Height = 60;

                //Header нумерация
                for (int i = 1; i <= 26; i++)
                {
                    worksheet.Cells[7, i].Value = $"{i}";
                }

                var airPollutionSources = _context.AirPollutionSource
                    .Include(a => a.Type)
                    .Include(a => a.SourceInfo)
                    .Include(a => a.SourceIndSite.IndSiteEnterprise.Enterprise.Kato)
                    .Include(a => a.SourceWorkshop.Workshop.IndSiteEnterprise.Enterprise.Kato)
                    .Include(a => a.SourceArea.Area.Workshop.IndSiteEnterprise.Enterprise.Kato)
                    .Include(a => a.OperationModes)
                        .ThenInclude(mode => mode.GasAirMixture)
                    .Include(a => a.OperationModes)
                        .ThenInclude(mode => mode.Emissions)
                            .ThenInclude(e => e.Pollutant)
                    .Where(a => a.SourceIndSite.IndSiteEnterprise.Enterprise.Kato.Code == katoCode
                        || a.SourceWorkshop.Workshop.IndSiteEnterprise.Enterprise.Kato.Code == katoCode
                        || a.SourceArea.Area.Workshop.IndSiteEnterprise.Enterprise.Kato.Code == katoCode)
                    .ToList();

                var indSiteEnterprises = _context.IndSiteEnterprise
                    .Include(site => site.Enterprise.Kato)
                    .Where(site => site.Enterprise.Kato.Code == katoCode)
                    .ToList();

                var enterprises = indSiteEnterprises
                    .Select(site => site.Enterprise)
                    .GroupBy(e => e.Bin)
                    .Select(e => e.First())
                    .ToList();


                //Data
                var row = 8;
                foreach (var enterprise in enterprises)
                {
                    foreach (var indSiteEnterprise in indSiteEnterprises
                        .Where(site => site.EnterpriseId == enterprise.Id))
                    {
                        var indSiteAirPollutionSources = airPollutionSources
                            .Where(a => a.SourceIndSite?.IndSiteEnterpriseId == indSiteEnterprise.Id ||
                                a.SourceWorkshop?.Workshop.IndSiteEnterpriseId == indSiteEnterprise.Id ||
                                a.SourceArea?.Area.Workshop.IndSiteEnterpriseId == indSiteEnterprise.Id);

                        if (!indSiteAirPollutionSources.Any())
                            continue;

                        worksheet.Cells[$"A{row}:Z{row}"].Merge = true;
                        worksheet.Cells[$"A{row}"].Value = $"{enterprise.Name}: Площадка \"{indSiteEnterprise.Name}\"";
                        worksheet.Cells[$"A{row}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[$"A{row}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[$"A{row}"].Style.Font.Bold = true;

                        row++;
                        foreach (var airSource in indSiteAirPollutionSources)
                        {
                            var operationMode = airSource.OperationModes
                                .FirstOrDefault(mode => (mode.WorkedTime > 0 && (mode.GasAirMixture?.Volume > 0 || mode.GasAirMixture?.Temperature > 0 || mode.GasAirMixture?.Speed > 0))
                                    || mode.WorkedTime > 0);

                            var emissions = airSource.OperationModes
                                .Where(mode => mode.Emissions?.Any() is true)
                                .Select(mode => mode.Emissions)
                                .FirstOrDefault();
                            emissions = emissions ?? new List<Models.ASM.PollutionSources.AirEmission>();

                            var coordinateArray = airSource.SourceInfo.Coordinate?.Split(',');
                            var endRow = row + (emissions?.Count > 1 ? emissions.Count - 1 : 0);

                            //Data таблицы
                            var airSourceData = new[]
                            {
                                ($"A{row}", $"A{row}:A{endRow}", enterprise.Bin),
                                ($"B{row}", $"B{row}:B{endRow}", airSource.SourceWorkshop?.Workshop?.Name),
                                ($"C{row}", $"C{row}:C{endRow}", string.Empty),
                                ($"D{row}", $"D{row}:D{endRow}", "1"),
                                ($"E{row}", $"E{row}:E{endRow}", operationMode?.WorkedTime.ToString()),
                                ($"F{row}", $"F{row}:F{endRow}", airSource.Name),
                                ($"G{row}", $"G{row}:G{endRow}", airSource.Number),
                                ($"H{row}", $"H{row}:H{endRow}", airSource.SourceInfo?.Hight.ToString()),
                                ($"I{row}", $"I{row}:I{endRow}", airSource.SourceInfo?.Diameter.ToString()),
                                ($"J{row}", $"J{row}:J{endRow}", operationMode?.GasAirMixture?.Speed.ToString()),
                                ($"K{row}", $"K{row}:K{endRow}", operationMode?.GasAirMixture?.Volume.ToString()),
                                ($"L{row}", $"L{row}:L{endRow}", operationMode?.GasAirMixture?.Temperature.ToString()),
                                ($"M{row}", $"M{row}:M{endRow}", coordinateArray?.Length == 2 ? coordinateArray[0] : null),
                                ($"N{row}", $"N{row}:N{endRow}", coordinateArray?.Length == 2 ? coordinateArray[1] : null),
                                ($"O{row}", $"O{row}:O{endRow}", airSource.SourceInfo.Length?.ToString()),
                                ($"P{row}", $"P{row}:P{endRow}", airSource.SourceInfo.Width?.ToString()),
                                ($"Q{row}", $"Q{row}:Q{endRow}", string.Empty),
                                ($"R{row}", $"R{row}:R{endRow}", string.Empty),
                                ($"S{row}", $"S{row}:S{endRow}", string.Empty),
                                ($"T{row}", $"T{row}:T{endRow}", string.Empty),
                            };

                            foreach (var data in airSourceData)
                            {
                                worksheet.Cells[data.Item1].Value = data.Item3;
                                worksheet.Cells[data.Item2].Merge = true;
                                worksheet.Cells[data.Item1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[data.Item1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            }

                            //Emissions
                            foreach (var emission in emissions)
                            {
                                worksheet.Cells[$"U{row}"].Value = emission.Pollutant.Code;
                                worksheet.Cells[$"V{row}"].Value = emission.Pollutant.Name;
                                worksheet.Cells[$"W{row}"].Value = emission.MaxGramSec;
                                worksheet.Cells[$"X{row}"].Value = emission.MaxMilligramMeter;
                                worksheet.Cells[$"Y{row}"].Value = emission.GrossTonYear;

                                worksheet.Row(row).Height = 30;
                                worksheet.Cells[$"V{row}"].Style.WrapText = true;
                                worksheet.Cells[$"W{row}"].Style.WrapText = true;
                                worksheet.Cells[$"X{row}"].Style.WrapText = true;
                                worksheet.Cells[$"Y{row}"].Style.WrapText = true;
                                worksheet.Column(22).Width = 30;
                                row++;
                            }
                            row = endRow + 1;
                        }
                    }
                }

                //Установка стиля
                for (int rowStyle = 4; rowStyle <= row; rowStyle++)
                {
                    for (int colStyle = 1; colStyle <= 26; colStyle++)
                    {
                        worksheet.Cells[rowStyle, colStyle].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStyle, colStyle].Style.Font.Size = 10;
                    }
                }

                for (int colPollutants = 23; colPollutants <= 25; colPollutants++)
                    worksheet.Column(colPollutants).AutoFit();

                worksheet.View.ZoomScale = 80;

                package.Save();
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private FileInfo CreateExcelFile(string fileName)
        {
            FileInfo file = new FileInfo(Path.Combine(_pathExcelFiles, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(fileName));
            }
            return file;
        }
    }
}

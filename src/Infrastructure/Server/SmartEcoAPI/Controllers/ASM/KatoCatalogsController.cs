using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers.ASM
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class KatoCatalogsController : Controller
    {
        private readonly string _katoFileDir = @"C:\Users";
        //private readonly string _katoFileDir = @"C:\Users\Administrator\source\repos\Download\AlmatyPollution.txt";
        private readonly Regex _dateRegex = new Regex(@"\d{2}.\d{2}.\d{4}");

        private readonly ApplicationDbContext _context;

        public KatoCatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Date")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public ActionResult<string> GetDateFile()
        {
            var file = Directory.GetFiles(_katoFileDir, "*.csv").FirstOrDefault();
            if (file is null)
                return NotFound("File .csv is empty in directory");

            var date = Convert.ToString(_dateRegex.Match(Path.GetFileName(file)));
            if (string.IsNullOrEmpty(date))
                return StatusCode(StatusCodes.Status500InternalServerError, $"Can't parse file name\nName: {Path.GetFileName(file)}");

            return Ok(date);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = false)]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<IActionResult> PostKatoCatalog(IFormFile katoExcel)
        {
            try
            {
                if (!IsValidExtension(katoExcel, new List<string> { ".csv" }))
                    return BadRequest("Unsupported file type");

                var katoExcelList = await ReadFileAsStringAsync(katoExcel);
                var katoData = katoExcelList
                    .Skip(1)
                    .Select(col => col.Split(';'))
                    .Where(col => int.TryParse(col[0], out int result))
                    .Select(col => new KatoCatalog
                    {
                        Id = int.Parse(col[0]),
                        ParentId = int.Parse(col[1]),
                        Code = col[2],
                        Name = col[3]
                    })
                    .ToList();

                //Cheking current date new file
                var file = Directory.GetFiles(_katoFileDir, "*.csv").FirstOrDefault();
                if (!string.IsNullOrEmpty(file))
                {
                    var dateFile = Convert.ToString(_dateRegex.Match(Path.GetFileName(file)));
                    var errorNewDate = ErrorNewFileDate(dateFile, katoExcel);
                    if (!string.IsNullOrEmpty(errorNewDate))
                        return BadRequest(errorNewDate);

                    System.IO.File.Delete(file);
                }
                
                //Update file and data
                string newFilePath = Path.Combine(_katoFileDir, katoExcel.FileName);
                using (var stream = System.IO.File.Create(newFilePath))
                {
                    await katoExcel.CopyToAsync(stream);
                }

                _context.RemoveRange(_context.KatoCatalog);
                await _context.SaveChangesAsync();

                _context.AddRange(katoData);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private bool IsValidExtension(IFormFile formFile, List<string> extensions)
        {
            var extension = Path.GetExtension(formFile.FileName);
            if (extensions.Any() && !extensions.Contains(extension))
                return false;
            return true;
        }

        private static async Task<List<string>> ReadFileAsStringAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString().Split("\r\n").ToList();
        }

        private string ErrorNewFileDate(string oldFileDate, IFormFile katoExcel)
        {
            if (!string.IsNullOrEmpty(oldFileDate))
            {
                var dateNewFile = Convert.ToString(_dateRegex.Match(katoExcel.FileName));
                if (!DateTime.TryParse(dateNewFile, out var dateNew))
                    return "Incorrect file name! The name must contain a date in the format 'dd.MM.yyyy'";

                var isOldDateCurrent = DateTime.TryParse(oldFileDate, out var oldDate);
                if (isOldDateCurrent && oldDate >= dateNew)
                    return "The new file date is less than or equal to the current one";
            }
            return string.Empty;
        }
    }
}

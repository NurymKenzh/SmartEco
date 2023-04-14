using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartEcoAPI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvisionDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProvisionDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("almaty/download")]
        public async Task<IActionResult> AlmatyDownload()
        {
            string path = @"C:\Users\Administrator\source\repos\Download\AlmatyPollution.txt";
            if (System.IO.File.Exists(path))
            {
                FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(path), "text/plain")
                {
                    FileDownloadName = "AlmatyPollution.txt"
                };

                return result;
            }

            return NotFound();
        }
    }
}

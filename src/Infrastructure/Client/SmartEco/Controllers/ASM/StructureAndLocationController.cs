using Microsoft.AspNetCore.Mvc;
using SmartEco.Models.ASM;

namespace SmartEco.Controllers.ASM
{
    public class StructureAndLocationController : Controller
    {
        [HttpPost]
        public IActionResult CreateIndSiteEnterprise(IndSiteEnterprise indSiteEnterprise)
        {
            return RedirectToAction("Details", "Enterprises", new { id = indSiteEnterprise.EnterpriseId });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace SmartEco.Web.Controllers
{
    public class BaseController : Controller
    {
        #region View subfolders
        private protected static string Auth => "Auth";
        private protected static string Directories => "Directories";
        #endregion

        public string GetName<C>() where C : Controller
            => typeof(C).Name.Replace("Controller", string.Empty);
    }
}

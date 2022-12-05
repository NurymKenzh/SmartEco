using SmartEco.Web.Services.Providers;

namespace SmartEco.Web.Services
{
    public class CrudService
    {
        private readonly ISmartEcoApi _smartEcoApi;

        public CrudService(ISmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }
    }
}

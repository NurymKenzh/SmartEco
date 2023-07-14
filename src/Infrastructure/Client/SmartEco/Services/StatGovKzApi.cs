using SmartEco.Models.ASM.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Services
{
    public class StatGovKzApi
    {
        private readonly HttpClient _client;

        public StatGovKzApi(HttpClient client)
            => _client = client;

        public async Task<JuridicalAccountResponse> GetEnterpriseByBin(long bin)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"api/juridical/counter/api/?bin={bin}&lang=ru");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<JuridicalAccountResponse>();
            }
            catch
            {
                return new JuridicalAccountResponse();
            }
        }
    }
}

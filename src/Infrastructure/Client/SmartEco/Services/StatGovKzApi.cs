using SmartEco.Models.ASM.Responses;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Services
{
    public class StatGovKzApi
    {
        private readonly HttpClient _client;

        public StatGovKzApi(HttpClient client)
            => _client = client;

        public async Task<JuridicalAccountResponse> GetEnterpriseByBin(string bin)
        {
            try
            {
                await Task.Delay(2000);
                HttpResponseMessage response = await _client.GetAsync($"api/juridical/counter/api/?bin={bin}&lang=ru");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<JuridicalAccountResponse>();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("429"))
                    return new JuridicalAccountResponse() {  Description = "429: Too Many Requests" };
                else
                    return new JuridicalAccountResponse();
            }
        }
    }
}

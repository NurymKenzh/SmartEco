using System.Net.Http;
using System.Threading.Tasks;
using SmartEco.Models.ASM.Requests.Uprza;
using Newtonsoft.Json;
using SmartEco.Models.ASM.Responses.Uprza;
using Microsoft.AspNetCore.Hosting;

namespace SmartEco.Services.ASM
{
    public class UprzaApi
    {
        private readonly HttpClient _client;
        private readonly IHostingEnvironment _env;

        private readonly bool _isTest = true;
        private readonly string jsonResp200Moq = "{\"id\":946,\"type\":\"dispersion\",\"status\":\"in_queue\",\"created_at\":\"2024-01-17T16:56:43.283647+00:00\",\"started_at\":\"0001-01-01T00:00:00+00:00\",\"updated_at\":\"0001-01-01T00:00:00+00:00\",\"diagnostic_data\":{\"count_of_points\":0,\"avg_time\":0,\"count_of_threads\":0,\"count_of_busts\":0,\"error_info\":null,\"progress\":0,\"calc_started_at\":\"0001-01-01T00:00:00+00:00\",\"calc_finished_at\":\"0001-01-01T00:00:00+00:00\",\"transform_started_at\":\"0001-01-01T00:00:00+00:00\",\"transform_finished_at\":\"0001-01-01T00:00:00+00:00\",\"timers\":null},\"message\":null,\"description\":null,\"daemon_id\":null}";
        private readonly string jsonResp400Moq = "{\"id\":0,\"type\":null,\"status\":null,\"created_at\":\"0001-01-01T00:00:00+00:00\",\"started_at\":\"0001-01-01T00:00:00+00:00\",\"updated_at\":\"0001-01-01T00:00:00+00:00\",\"diagnostic_data\":null,\"message\":\"Bad request\",\"description\":[\"Не задана температура окружающей среды\"],\"daemon_id\":null}";
        private readonly string jsonResp400Moq2 = "{\"id\":0,\"type\":null,\"status\":null,\"created_at\":\"0001-01-01T00:00:00+00:00\",\"started_at\":\"0001-01-01T00:00:00+00:00\",\"updated_at\":\"0001-01-01T00:00:00+00:00\",\"diagnostic_data\":null,\"message\":\"Bad request\",\"description\":[\"Не заданы загрязняющие вещества\",\"Не задана температура окружающей среды\"],\"daemon_id\":null}";
        private readonly string jsonResp404Moq = "Bad request";

        private readonly string jsonRespStatusReadyMoq = "{\"id\":936,\"type\":\"dispersion\",\"status\":\"ready\",\"description\":null,\"created_at\":\"2024-01-17T11:04:23.128348\",\"started_at\":\"2024-01-17T11:04:23.897493\",\"updated_at\":\"2024-01-17T11:05:22.269014\",\"diagnostic_data\":{\"count_of_points\":40401,\"avg_time\":0,\"count_of_threads\":1,\"count_of_busts\":1,\"error_info\":null,\"progress\":100,\"calc_started_at\":\"2024-01-17T11:04:23.8979816+00:00\",\"calc_finished_at\":\"2024-01-17T11:05:21.3869026+00:00\",\"transform_started_at\":\"2024-01-17T11:05:21.7392588+00:00\",\"transform_finished_at\":\"2024-01-17T11:05:22.2690071+00:00\",\"timers\":{\"save_data\":3189,\"calculate\":21816,\"AddJobPoints\":2035,\"AddJobPointPollutantsResults\":821,\"AddContributorsResults\":49626,\"AddJobPointSummationGroupsResults\":14,\"ResolveJobPointsMaxConcentrations\":18}},\"daemon_id\":null}";
        private readonly string jsonRespStatusProccessMoq = "{\"id\":948,\"type\":\"dispersion\",\"status\":\"in_progress\",\"description\":null,\"created_at\":\"2024-01-18T09:53:33.758772\",\"started_at\":\"2024-01-18T09:53:34.582892\",\"updated_at\":\"2024-01-18T09:53:34.583404\",\"diagnostic_data\":{\"count_of_points\":0,\"avg_time\":0,\"count_of_threads\":0,\"count_of_busts\":0,\"error_info\":null,\"progress\":0,\"calc_started_at\":\"2024-01-18T09:53:34.583404+00:00\",\"calc_finished_at\":\"0001-01-01T00:00:00\",\"transform_started_at\":\"0001-01-01T00:00:00\",\"transform_finished_at\":\"0001-01-01T00:00:00\",\"timers\":null},\"daemon_id\":null}";
        private readonly string jsonRespStatusErrorMoq = "{\"id\":935,\"type\":\"dispersion\",\"status\":\"error\",\"description\":null,\"created_at\":\"2024-01-17T11:01:59.637313\",\"started_at\":\"2024-01-17T11:02:00.019001\",\"updated_at\":\"2024-01-17T11:03:18.260441\",\"diagnostic_data\":{\"count_of_points\":40401,\"avg_time\":0,\"count_of_threads\":1,\"count_of_busts\":1,\"error_info\":\"One or more errors occurred. (Exception while reading from stream)\\n StackTrace:\\n    at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)\\n   at System.Threading.Tasks.Task.Wait(CancellationToken cancellationToken)\\n   at UprzaKernel.Daemon.Services.JobHandler.Calculate(CancellationToken cancellationToken) in /app/UprzaKernel.Daemon/Services/JobHandler.cs:line 142\",\"progress\":100,\"calc_started_at\":\"2024-01-17T11:02:00.0207721+00:00\",\"calc_finished_at\":\"0001-01-01T00:00:00\",\"transform_started_at\":\"0001-01-01T00:00:00\",\"transform_finished_at\":\"0001-01-01T00:00:00\",\"timers\":null},\"daemon_id\":null}";

        public UprzaApi(HttpClient client, IHostingEnvironment env)
            => (_client, _env) = (client, env);

        public async Task<UprzaCalcStatusResponse> CreateCalculation(UprzaRequest uprzaRequest)
        {
            try
            {
                if (_env.IsDevelopment() && _isTest)
                    return GetMoqResponse();

                var jsonRequest = JsonConvert.SerializeObject(uprzaRequest, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                var content = new StringContent(jsonRequest);
                HttpResponseMessage response = await _client.PostAsync($"calculation/create", content);
                var uprzaResp = await response.Content.ReadAsAsync<UprzaCalcStatusResponse>();
                return uprzaResp;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UprzaCalcStatusResponse> StatusCalculation(int jobId)
        {
            try
            {
                if (_env.IsDevelopment() && _isTest)
                    return GetMoqStatusResponse();

                HttpResponseMessage response = await _client.GetAsync($"calculation/status?id={jobId}");
                var uprzaResp = await response.Content.ReadAsAsync<UprzaCalcStatusResponse>();
                return uprzaResp;
            }
            catch
            {
                return null;
            }
        }

        private UprzaCalcStatusResponse GetMoqResponse()
            => JsonConvert.DeserializeObject<UprzaCalcStatusResponse>(jsonResp200Moq);

        private UprzaCalcStatusResponse GetMoqStatusResponse()
            => JsonConvert.DeserializeObject<UprzaCalcStatusResponse>(jsonRespStatusReadyMoq);
    }
}

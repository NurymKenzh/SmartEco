using SmartEco.Models.ASM.Requests.Uprza;
using SmartEco.Models.ASM.Responses.Uprza;
using SmartEco.Models.ASM.Uprza;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Services.ASM
{
    public class UprzaService : IUprzaService
    {
        private readonly UprzaApi _uprzaApi;

        public UprzaService(UprzaApi uprzaApi)
        {
            _uprzaApi = uprzaApi;
        }

        public async Task<StateCalculation> SendCalculation(UprzaRequest request, Calculation calculation)
        {
            var response = await _uprzaApi.CreateCalculation(request) ?? new UprzaCalcStatusResponse()
            {
                Status = "error",
                Message = "Internal Server Error",
                Description = new[] { "Не удалось выполнить запрос в УПРЗА. Повторите попытку позже." }
            };
            var partialCalc = MapResponse(response, calculation);
            partialCalc.Calculation.StatusId = GetCalcStatus(response.Status);
            return partialCalc;
        }

        public async Task<StateCalculation> GetStatusCalculation(int jobId, Calculation calculation)
        {
            var response = await _uprzaApi.StatusCalculation(jobId);
            if (response is null)
                return null;

            var partialCalc = MapResponse(response, calculation);
            partialCalc.Calculation.StatusId = GetCalcStatus(response.Status);
            return partialCalc;
        }

        private StateCalculation MapResponse(UprzaCalcStatusResponse respone, Calculation calculation)
            => new StateCalculation
            {
                CalculationId = calculation.Id,
                Calculation = calculation,
                InitializedOn = DateTime.Now,
                JobId = respone.Id,
                ErrorMessage = respone.Message,
                DiagnosticInfo = new DiagnosticInfo()
                {
                    Progress = respone.DiagnosticData?.Progress ?? 0,
                    CalculationTime = GetCalculaionTime(respone.DiagnosticData?.CalcStartedAt, respone.DiagnosticData?.CalcFinishedAt) ?? 0,
                    AverageTime = respone.DiagnosticData?.AvgTime ?? 0,
                    NumberOfPoints = respone.DiagnosticData?.CountOfPoints ?? 0,
                    NumberOfThreads = respone.DiagnosticData?.CountOfThreads ?? 0,
                    NumberOfIterations = respone.DiagnosticData?.CountOfBusts ?? 0
                },
                Description = respone.DiagnosticData?.ErrorInfo is null
                    ? respone.Description?.ToList()
                    : new List<string> { respone.DiagnosticData?.ErrorInfo },
            };

        private double? GetCalculaionTime(DateTimeOffset? startedAt, DateTimeOffset? finishedAt)
        {
            if(startedAt is null || finishedAt is null)
                return null;

            var totalSeconds = (finishedAt - startedAt).Value.TotalSeconds;
            if(totalSeconds < 0)
                return null;

            return totalSeconds;
        }

        private int GetCalcStatus(string status)
        {
            switch (status)
            {
                case "init":
                case "in_queue":
                case "in_progress":
                case "pause":
                    return (int)CalculationStatuses.Initiated;
                case "ready":
                    return (int)CalculationStatuses.Done;
                case "error":
                default: 
                    return (int)CalculationStatuses.Error;
            }
        }
    }

    public interface IUprzaService
    {
        Task<StateCalculation> SendCalculation(UprzaRequest request, Calculation calculation);
        Task<StateCalculation> GetStatusCalculation(int jobId, Calculation calculation);
    }
}

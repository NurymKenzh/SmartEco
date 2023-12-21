using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ServiceManager.Enums;
using ServiceManager.Models;
using ServiceManager.Services.Logging;
using SmartEco.Common.Enums;
using SmartEco.Common.Services.Proto;

namespace ServiceManager.GrpcServices
{
    internal class ServiceManagerGrpcService(
        ILoggerService _loggerService) : ServiceManagerGrpc.ServiceManagerGrpcBase
    {
        public override Task<Empty> SendInfoLog(LogInfoRequest request, ServerCallContext context)
        {
            _loggerService.AddInfoLog(MapService(request.WorkerType), request.LogMessage, Bindings.ReporterTab, MapColor(request.WorkerType));
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> SendErrorLog(LogErrorRequest request, ServerCallContext context)
        {
            _loggerService.AddErrorLog("", $"{request.LogMessage}\n{request.StackTrace}", Bindings.ReporterTab);
            return Task.FromResult(new Empty());
        }

        private static string MapService(int worker)
            => (WorkerType)worker switch
            {
                WorkerType.Reporter => nameof(WorkerType.Reporter),
                WorkerType.ReporterCheckData => nameof(WorkerType.ReporterCheckData).CutText(nameof(WorkerType.Reporter)),
                _ => string.Empty
            };

        private static ColorType MapColor(int worker)
            => (WorkerType)worker switch
            {
                WorkerType.Reporter => ColorType.DarkGreen,
                WorkerType.ReporterCheckData => ColorType.DarkBlue,
                _ => ColorType.Gray
            };
    }

    file static class StringExtension
    {
        public static string CutText(this string value, string remove)
            => value.Replace(remove, string.Empty);
    }
}

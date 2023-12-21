using SmartEco.Common.Enums;
using SmartEco.Common.Services.Proto;

namespace SmartEco.Common.Services.GrpcClients
{
    public class ServiceManagerGrpcClient(
        ServiceManagerGrpc.ServiceManagerGrpcClient _serviceManager) : IServiceManagerGrpcClient
    {
        public async Task SendInfoLog(WorkerType workerType, string message)
        {
            try
            {
                await _serviceManager.SendInfoLogAsync(new LogInfoRequest() { WorkerType = (int)workerType, LogMessage = message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public async Task SendErrorLog(WorkerType workerType, string message, string? stackTrace)
        {
            try
            {
                await _serviceManager.SendErrorLogAsync(new LogErrorRequest() { WorkerType = (int)workerType, LogMessage = message, StackTrace = stackTrace });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }

    public interface IServiceManagerGrpcClient
    {
        public Task SendInfoLog(WorkerType workerType, string message);
        public Task SendErrorLog(WorkerType workerType, string message, string? stackTrace);
    }
}

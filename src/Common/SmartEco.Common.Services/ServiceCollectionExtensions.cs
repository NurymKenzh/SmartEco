using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using SmartEco.Common.Services.GrpcClients;
using SmartEco.Common.Services.Proto;

namespace SmartEco.Common.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceManagerGrpcClient(this IServiceCollection services)
        {
            services.AddSingleton<IServiceManagerGrpcClient, ServiceManagerGrpcClient>();
            services.AddGrpcClient<ServiceManagerGrpc.ServiceManagerGrpcClient>(o =>
            {
                o.Address = new Uri("https://localhost:7227");
            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });
            return services;
        }
    }
}

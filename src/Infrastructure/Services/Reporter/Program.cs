using Reporter.Repositories;
using Reporter.Services;
using SmartEco.Common;
using SmartEco.Common.Data;
using SmartEco.Common.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Reporter Service";
});

builder.Services.AddSmartEcoApiDbContext(builder.Configuration);
builder.Services.AddSmartEcoServicesDbContext(builder.Configuration);
builder.Services.AddEmailSender(builder.Configuration);
builder.Services.AddServicesPublicRepository();
builder.Services.AddServiceManagerGrpcClient();

builder.Services.AddScoped<ISmartEcoApiRepository, SmartEcoApiRepository>();
builder.Services.AddScoped<ISmartEcoServicesRepository, SmartEcoServicesRepository>();

builder.Services.AddScoped<ICheckDataService, CheckDataService>();

builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddHostedService<ScopedBackgroundService>();

IHost host = builder.Build();
host.Run();

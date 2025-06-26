using Infrastructure.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Get Service Bus connection string from configuration
var serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBus") 
                                 ?? throw new InvalidOperationException("ServiceBus connection string is not configured.");

// Register services from extensions
builder.Services.AddServices(serviceBusConnectionString);
builder.Services.AddRepositories(builder.Configuration);

builder.Build().Run();
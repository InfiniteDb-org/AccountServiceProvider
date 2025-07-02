using Application.Providers;
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

var serviceBusConnectionString = builder.Configuration.GetConnectionString("ASB_ConnectionString")
                                 ?? throw new InvalidOperationException("ASB_ConnectionString is not configured.");

// Register services from extensions
builder.Services.AddServices(serviceBusConnectionString);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddHttpClient<IEmailVerificationProvider, EmailVerificationProvider>();

var app = builder.Build();

var envName = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");
await app.Services.EnsureCosmosContainersCreatedAsync(envName);

app.Run();
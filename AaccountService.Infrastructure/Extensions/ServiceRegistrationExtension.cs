using Application.Interfaces;
using Application.Services;
using Azure.Messaging.ServiceBus;
using Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Application.Configuration;

namespace Infrastructure.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, string serviceBusConnectionString, IConfiguration configuration)
    {
        // Configure password policy options from appsettings.json
        services.Configure<PasswordPolicyOptions>(
            configuration.GetSection(PasswordPolicyOptions.SectionName));

        // Register application services
        services.AddScoped<IAccountService, Application.Services.AccountService>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IPasswordValidator, PasswordValidator>();
        
        // Register Service Bus client
        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));
        
        return services;
    }
}

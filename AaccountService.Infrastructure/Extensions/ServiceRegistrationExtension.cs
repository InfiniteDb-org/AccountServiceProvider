using Application.Interfaces;
using Application.Services;
using Azure.Messaging.ServiceBus;
using Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, string serviceBusConnectionString)
    {
        // Register application services
        services.AddScoped<IAccountService, Application.Services.AccountService>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        
        // Register Service Bus client
        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));
        
        return services;
    }
}

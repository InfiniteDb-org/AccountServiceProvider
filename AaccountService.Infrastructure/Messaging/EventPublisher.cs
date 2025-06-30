using AccountService.Contracts.Requests;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Messaging;

public class EventPublisher(ServiceBusClient serviceBusClient, ILogger<EventPublisher> logger) : IEventPublisher, IAsyncDisposable
{
    private const string QueueName = "email-verification";
    private readonly ServiceBusSender _sender = serviceBusClient.CreateSender(QueueName);
    

    public async Task PublishVerificationCodeSentEventAsync(string userId, string? email, string code)
    {
        var emailRequest = EmailRequestFactory.CreateVerificationEmail(email!, code);
        await PublishEmailRequestAsync(emailRequest, userId, "verification code");
    }

    private async Task PublishEmailRequestAsync(EmailSendRequest request, string userId, string mailType)
    {
        var message = JsonConvert.SerializeObject(request);
        var serviceBusMessage = new ServiceBusMessage(message);
        logger.LogInformation("Publishing EmailSendRequest to ServiceBus: {Payload}", message);
        await _sender.SendMessageAsync(serviceBusMessage);
        logger.LogInformation($"Published {mailType} email for user {userId}");
    }

    public async Task PublishAccountCreatedEventAsync(string userId, string? email)
    {
        var emailRequest = EmailRequestFactory.CreateWelcomeEmail(email!);
        await PublishEmailRequestAsync(emailRequest, userId, "welcome");
    }

    public async Task PublishPasswordResetRequestedEventAsync(string userId, string? email, string token)
    {
        var emailRequest = EmailRequestFactory.CreatePasswordResetEmail(email!, token);
        await PublishEmailRequestAsync(emailRequest, userId, "password reset");
    }

    public async Task PublishAccountDeletedEventAsync(string userId, string? email)
    {
        var emailRequest = EmailRequestFactory.CreateAccountDeletedEmail(email!);
        await PublishEmailRequestAsync(emailRequest, userId, "account deleted");
    }

    public async ValueTask DisposeAsync() => await _sender.DisposeAsync();
}

using AccountService.Contracts.Events;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Messaging;

public class EventPublisher(ServiceBusClient serviceBusClient, ILogger<EventPublisher> logger) : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger = logger;
    private const string QueueName = "account-events";
    private readonly ServiceBusSender _sender = serviceBusClient.CreateSender(QueueName);

    private async Task PublishEventAsync(AccountEventMessage evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        var serviceBusMessage = new ServiceBusMessage(message);
        await _sender.SendMessageAsync(serviceBusMessage);
        _logger.LogInformation($"Published event: {evt.EventType} for user {evt.UserId}");
    }

    public async Task PublishVerificationCodeSentEventAsync(string userId, string? email, string code)
    {
        var evt = new AccountEventMessage
        {
            EventType = "VerificationCodeSent",
            UserId = userId,
            Email = email,
            Code = code,
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishAccountCreatedEventAsync(string userId, string? email)
    {
        var evt = new AccountEventMessage
        {
            EventType = "AccountCreated",
            UserId = userId,
            Email = email,
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishPasswordResetRequestedEventAsync(string userId, string? email, string token)
    {
        var evt = new AccountEventMessage
        {
            EventType = "PasswordResetRequested",
            UserId = userId,
            Email = email,
            Token = token,
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishAccountDeletedEventAsync(string userId, string? email)
    {
        var evt = new AccountEventMessage
        {
            EventType = "AccountDeleted",
            UserId = userId,
            Email = email,
        };
        await PublishEventAsync(evt);
    }
}

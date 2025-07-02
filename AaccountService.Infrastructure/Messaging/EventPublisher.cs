using AccountService.Contracts.Events;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Messaging;

public class EventPublisher(ServiceBusClient serviceBusClient, ILogger<EventPublisher> logger) : IEventPublisher, IAsyncDisposable
{
    private const string QueueName = "account-events";
    private const string VerificationRequestsQueue = "verification-requests";
    private readonly ServiceBusSender _sender = serviceBusClient.CreateSender(QueueName);
    private readonly ServiceBusSender _verificationSender = serviceBusClient.CreateSender(VerificationRequestsQueue);

    public async Task PublishVerificationCodeSentEventAsync(string userId, string? email, string code)
    {
        var evt = new AccountMessageEvent
        {
            EventType = "VerificationCodeSent",
            UserId = userId,
            Email = email,
            Code = code
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishAccountCreatedEventAsync(string userId, string? email)
    {
        var evt = new AccountMessageEvent
        {
            EventType = "AccountCreated",
            UserId = userId,
            Email = email
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishPasswordResetRequestedEventAsync(string userId, string? email, string token)
    {
        var evt = new AccountMessageEvent
        {
            EventType = "PasswordResetRequested",
            UserId = userId,
            Email = email,
            Token = token
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishAccountDeletedEventAsync(string userId, string? email)
    {
        var evt = new AccountMessageEvent
        {
            EventType = "AccountDeleted",
            UserId = userId,
            Email = email
        };
        await PublishEventAsync(evt);
    }

    public async Task PublishVerificationCodeRequestedAsync(string userId, string email)
    {
        var evt = new VerificationCodeRequestedEvent
        {
            UserId = userId,
            Email = email
        };
        var message = JsonConvert.SerializeObject(evt);
        logger.LogInformation("Publishing VerificationCodeRequestedEvent: {Payload}", message);
        await _verificationSender.SendMessageAsync(new ServiceBusMessage(message));
    }

    private async Task PublishEventAsync(AccountMessageEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        var serviceBusMessage = new ServiceBusMessage(message);
        logger.LogInformation("Publishing AccountEventMessage to ServiceBus: {Payload}", message);
        await _sender.SendMessageAsync(serviceBusMessage);
        logger.LogInformation($"Published {evt.EventType} event for user {evt.UserId}");
    }

    public async ValueTask DisposeAsync() 
    {
        await _sender.DisposeAsync();
        await _verificationSender.DisposeAsync();
    }
}


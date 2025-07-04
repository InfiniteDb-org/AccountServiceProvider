using AccountService.Contracts.Events;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Infrastructure.Messaging;

public class EventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusSender _verificationSender;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(ServiceBusClient serviceBusClient, ILogger<EventPublisher> logger, IConfiguration config)
    {
        var accountEventsQueue = config["ASB_AccountEventsQueue"];
        if (string.IsNullOrWhiteSpace(accountEventsQueue))
            throw new InvalidOperationException("ASB_AccountEventsQueue is not configured.");

        var verificationRequestsQueue = config["ASB_VerificationRequestsQueue"];
        if (string.IsNullOrWhiteSpace(verificationRequestsQueue))
            throw new InvalidOperationException("ASB_VerificationRequestsQueue is not configured.");

        _sender = serviceBusClient.CreateSender(accountEventsQueue);
        _verificationSender = serviceBusClient.CreateSender(verificationRequestsQueue);
        _logger = logger;
    }

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
        _logger.LogInformation("Publishing VerificationCodeRequestedEvent to verification-code-requests queue: {Payload}", message);
        await _verificationSender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent VerificationCodeRequestedEvent to verification-code-requests queue for user {UserId}", userId);
    }

    private async Task PublishEventAsync(AccountMessageEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        var serviceBusMessage = new ServiceBusMessage(message);
        _logger.LogInformation("Publishing {EventType} to account-lifecycle-events queue: {Payload}", evt.EventType, message);
        await _sender.SendMessageAsync(serviceBusMessage);
        _logger.LogInformation("Successfully sent {EventType} event to account-lifecycle-events queue for user {UserId}", evt.EventType, evt.UserId);
    }

    public async ValueTask DisposeAsync() 
    {
        await _sender.DisposeAsync();
        await _verificationSender.DisposeAsync();
    }
}

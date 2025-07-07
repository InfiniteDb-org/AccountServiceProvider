using AccountService.Contracts.Events;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Infrastructure.Messaging;

// Publishes domain events to Azure Service Bus queues
public class EventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusSender _verificationSender;
    private readonly ILogger<EventPublisher> _logger;

    // Initializes Service Bus senders for account and verification events
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

    public async Task PublishAccountCreatedEventAsync(AccountCreatedEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        _logger.LogInformation("Publishing AccountCreatedEvent to account-lifecycle-events queue: {Payload}", message);
        await _sender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent AccountCreatedEvent to account-lifecycle-events queue for user {UserId}", evt.UserId);
    }

    public async Task PublishAccountDeletedEventAsync(AccountDeletedEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        _logger.LogInformation("Publishing AccountDeletedEvent to account-lifecycle-events queue: {Payload}", message);
        await _sender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent AccountDeletedEvent to account-lifecycle-events queue for user {UserId}", evt.UserId);
    }

    public async Task PublishPasswordResetRequestedEventAsync(PasswordResetRequestedEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        _logger.LogInformation("Publishing PasswordResetRequestedEvent to account-lifecycle-events queue: {Payload}", message);
        await _sender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent PasswordResetRequestedEvent to account-lifecycle-events queue for user {UserId}", evt.UserId);
    }

    public async Task PublishVerificationCodeRequestedEventAsync(VerificationCodeRequestedEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        _logger.LogInformation("Publishing VerificationCodeRequestedEvent to verification-code-requests queue: {Payload}", message);
        await _verificationSender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent VerificationCodeRequestedEvent to verification-code-requests queue for user {UserId}", evt.UserId);
    }

    public async Task PublishVerificationCodeSentEventAsync(VerificationCodeSentEvent evt)
    {
        var message = JsonConvert.SerializeObject(evt);
        _logger.LogInformation("Publishing VerificationCodeSentEvent to account-lifecycle-events queue: {Payload}", message);
        await _sender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation("Successfully sent VerificationCodeSentEvent to account-lifecycle-events queue for user {UserId}", evt.UserId);
    }

    public async ValueTask DisposeAsync() 
    {
        await _sender.DisposeAsync();
        await _verificationSender.DisposeAsync();
    }
}

using AccountService.Contracts.Events;

namespace Application.Interfaces;

public interface IEventPublisher
{
    Task PublishVerificationCodeSentEventAsync(VerificationCodeSentEvent evt);
    Task PublishAccountCreatedEventAsync(AccountCreatedEvent evt);
    Task PublishPasswordResetRequestedEventAsync(PasswordResetRequestedEvent evt);
    Task PublishAccountDeletedEventAsync(AccountDeletedEvent evt);
    Task PublishVerificationCodeRequestedEventAsync(VerificationCodeRequestedEvent evt);
}
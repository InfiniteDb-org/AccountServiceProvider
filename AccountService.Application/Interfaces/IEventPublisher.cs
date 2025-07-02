namespace Application.Interfaces;

public interface IEventPublisher
{
    Task PublishVerificationCodeSentEventAsync(string userId, string? email, string code);
    Task PublishAccountCreatedEventAsync(string userId, string? email);
    Task PublishPasswordResetRequestedEventAsync(string userId, string? email, string token);
    Task PublishAccountDeletedEventAsync(string userId, string? email);
    Task PublishVerificationCodeRequestedAsync(string userId, string email);
}
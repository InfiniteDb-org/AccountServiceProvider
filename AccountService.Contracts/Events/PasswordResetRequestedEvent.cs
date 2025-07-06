namespace AccountService.Contracts.Events;

public class PasswordResetRequestedEvent
{
    public string EventType { get; set; } = "PasswordResetRequested";
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Code { get; set; }
    public string ResetToken { get; set; } = null!;
}

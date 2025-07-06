namespace AccountService.Contracts.Events;

public class VerificationCodeSentEvent
{
    public string EventType { get; set; } = "VerificationCodeSent";
    public string UserId { get; set; } = null!;
    public string? Email { get; set; }
    public string Code { get; set; } = null!;
}

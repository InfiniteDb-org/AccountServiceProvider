namespace AccountService.Contracts.Events;

public class AccountEventMessage
{
    public string EventType { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string? Token { get; set; }
}
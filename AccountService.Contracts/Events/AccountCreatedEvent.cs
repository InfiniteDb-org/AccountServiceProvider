namespace AccountService.Contracts.Events;

public class AccountCreatedEvent
{
    public string EventType { get; set; } = "AccountCreated";
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
}

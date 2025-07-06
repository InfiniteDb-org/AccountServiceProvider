namespace AccountService.Contracts.Events;

public class AccountDeletedEvent
{
    public string EventType { get; set; } = "AccountDeleted";
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
}

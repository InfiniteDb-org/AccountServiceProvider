namespace AccountService.Contracts.Requests;

public class ConfirmEmailRequest
{
    public string Token { get; set; } = null!;
}
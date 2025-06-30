namespace AccountService.Contracts.Requests;

public class ConfirmEmailCodeRequest
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}

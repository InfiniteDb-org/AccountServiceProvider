namespace AccountService.Contracts.Requests;

public class ValidateCredentialsRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
namespace AccountService.Contracts.Requests;

public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? EmailConfirmed { get; set; }
}

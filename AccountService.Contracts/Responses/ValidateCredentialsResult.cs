using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class ValidateCredentialsResult : ResponseResult
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool EmailConfirmed { get; set; }
    public UserAccountDto? User { get; set; }
}
using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class ValidateCredentialsResult : ResponseResult
{
    public UserAccountDto? User { get; set; }
}
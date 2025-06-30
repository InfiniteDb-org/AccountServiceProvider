using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class CompleteRegistrationResult : ResponseResult
{
    public UserAccountDto? User { get; set; }
}

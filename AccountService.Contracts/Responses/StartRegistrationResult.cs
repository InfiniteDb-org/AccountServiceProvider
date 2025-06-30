using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class StartRegistrationResult : ResponseResult
{
    public UserAccountDto? User { get; set; }
}

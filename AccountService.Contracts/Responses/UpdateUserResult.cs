using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class UpdateUserResult : ResponseResult<UserAccountDto>
{
    public UpdateUserResult(UserAccountDto? data, string? message = "")
    {
        Data = data;
        Message = message;
        Succeeded = true;
    }
}

using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class GetAccountResult : ResponseResult<UserAccountDto>
{
    public GetAccountResult(UserAccountDto? data, string? message = "")
    {
        Data = data;
        Message = message;
        Succeeded = true;
    }
}
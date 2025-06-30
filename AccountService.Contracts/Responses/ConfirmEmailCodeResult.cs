using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class ConfirmEmailCodeResult : ResponseResult
{
    public UserAccountDto? User { get; set; }
}

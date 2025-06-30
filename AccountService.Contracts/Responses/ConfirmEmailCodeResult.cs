using AccountService.Contracts.DTOs;

namespace AccountService.Contracts.Responses;

public class ConfirmEmailCodeResult : ResponseResult
{
    public Guid? UserId { get; set; } 
    public UserAccountDto? User { get; set; }
}

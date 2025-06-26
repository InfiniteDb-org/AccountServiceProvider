namespace AccountService.Contracts.Responses;

public class CreateAccountResult : ResponseResult
{
    public Guid? UserId { get; set; }
}
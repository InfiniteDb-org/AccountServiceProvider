namespace AccountService.Contracts.Responses;

public class PasswordValidationResult : ResponseResult
{
    public List<string> Errors { get; set; } = [];

    public static PasswordValidationResult Success()
    {
        return new PasswordValidationResult 
        { 
            Succeeded = true,
            Message = "Password is valid"
        };
    }

    public static PasswordValidationResult Failure(string errorMessage, List<string>? errors = null)
    {
        return new PasswordValidationResult 
        { 
            Succeeded = false, 
            Message = errorMessage,
            Errors = errors ?? []
        };
    }
}

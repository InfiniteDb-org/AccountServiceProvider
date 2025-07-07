using AccountService.Contracts.DTOs;
using AccountService.Contracts.Responses;
using Application.Models;

namespace Application.Mappers;

// Provides mapping methods to convert User domain entities to DTOs and API result objects.
public static class AccountMapper
{
    
    public static UserAccountDto? ToUserAccountDto(this User? user)
    {
        if (user == null)
            return null;
            
        return new UserAccountDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            Role = user.Role
        };
    }
    
    public static GetAccountResult ToGetAccountResult(this User user, string? message = "Account retrieved successfully.")
    {
        return new GetAccountResult(user.ToUserAccountDto(), message);
    }
    
    public static ValidateCredentialsResult ToValidateCredentialsResult(this User user, string message = "Login successful.")
    {
        return new ValidateCredentialsResult
        {
            Succeeded = true,
            Message = message,
            User = user.ToUserAccountDto()
        };
    }
    
    public static StartRegistrationResult ToStartRegistrationResult(this User user, string? message = "Verification code sent.")
    {
        return new StartRegistrationResult
        {
            Succeeded = true,
            Message = message,
            User = user.ToUserAccountDto()
        };
    }
    
    public static CompleteRegistrationResult ToCompleteRegistrationResult(this User user, string? message = "Registration complete.")
    {
        return new CompleteRegistrationResult
        {
            Succeeded = true,
            Message = message,
            User = user.ToUserAccountDto()
        };
    }
    
    public static ConfirmEmailCodeResult ToConfirmEmailCodeResult(this User user, string? message = "Email confirmed.")
    {
        return new ConfirmEmailCodeResult
        {
            Succeeded = true,
            Message = message,
            User = user.ToUserAccountDto()
        };
    }
    
    public static UpdateUserResult ToUpdateUserResult(this User user, string? message = "User updated successfully.")
    {
        return new UpdateUserResult(user.ToUserAccountDto(), message);
    }
}
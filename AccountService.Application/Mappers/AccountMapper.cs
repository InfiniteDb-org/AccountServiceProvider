using AccountService.Contracts.DTOs;
using AccountService.Contracts.Responses;
using Application.Models;

namespace Application.Mappers;

public static class AccountMapper
{
    // Maps a User entity to a UserAccountDto
    public static UserAccountDto? ToUserAccountDto(this User? user)
    {
        if (user == null)
            return null;
            
        return new UserAccountDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed
        };
    }
    
    // Maps a User entity to a GetAccountResult
    public static GetAccountResult ToGetAccountResult(this User user, string? message = "Account retrieved successfully.")
    {
        return new GetAccountResult(user.ToUserAccountDto(), message);
    }
    
    // Maps a User entity to a ValidateCredentialsResult
    public static ValidateCredentialsResult ToValidateCredentialsResult(this User user, string message = "Login successful.")
    {
        return new ValidateCredentialsResult
        {
            Succeeded = true,
            Message = message,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            User = user.ToUserAccountDto()
        };
    }
    
    // Maps a User entity to an UpdateUserResult
    public static UpdateUserResult ToUpdateUserResult(this User user, string? message = "User updated successfully.")
    {
        return new UpdateUserResult(user.ToUserAccountDto(), message);
    }
}
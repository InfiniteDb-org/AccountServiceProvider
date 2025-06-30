using AccountService.Contracts.Requests;
using AccountService.Contracts.Responses;
using Application.Interfaces;
using Application.Mappers;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAccountService
{
    Task<ResponseResult<StartRegistrationResult>> StartRegistrationAsync(StartRegistrationRequest request);
    Task<ResponseResult<ConfirmEmailCodeResult>> ConfirmEmailCodeAsync(ConfirmEmailCodeRequest request);
    Task<ResponseResult<CompleteRegistrationResult>> CompleteRegistrationAsync(CompleteRegistrationRequest request);
    Task<ResponseResult<ValidateCredentialsResult>> ValidateCredentialsAsync(ValidateCredentialsRequest request);
    Task<ResponseResult<GetAccountResult>> GetAccountByIdAsync(Guid userId);
    Task<ResponseResult<GetAccountResult>> GetAccountByEmailAsync(string email);
    Task<ResponseResult<GenerateTokenResult>> GenerateNewEmailConfirmationCodeAsync(Guid userId);
    Task<ResponseResult<UpdateUserResult>> UpdateUserAsync(Guid userId, UpdateUserRequest? request);
    Task<ResponseResult<bool>> DeleteAccountAsync(Guid userId);
    Task<ResponseResult<ForgotPasswordResult>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ResponseResult<ResetPasswordResult>> ResetPasswordAsync(ResetPasswordRequest request);
}

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, IEventPublisher eventPublisher, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }


    public async Task<ResponseResult<StartRegistrationResult>> StartRegistrationAsync(StartRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return ResponseResult<StartRegistrationResult>.Failure("Email is required.");

        var existingUser = await _accountRepository.GetByEmailAsync(request.Email);
        if (existingUser.Succeeded && existingUser.Result != null)
            return ResponseResult<StartRegistrationResult>.Failure("Account already exists.");

        var user = new User { Email = request.Email };
        var createResult = await _accountRepository.CreateAsync(user, "");
        if (!createResult.Succeeded)
            return ResponseResult<StartRegistrationResult>.Failure(createResult.Message ?? "Failed to create user.");

        var code = GenerateSixDigitCode();
        var saveCodeResult = await _accountRepository.SaveVerificationCodeAsync(user, code);
        if (!saveCodeResult.Succeeded)
            return ResponseResult<StartRegistrationResult>.Failure(saveCodeResult.Message ?? "Failed to save verification code.");

        await _eventPublisher.PublishVerificationCodeSentEventAsync(user.Id.ToString(), user.Email, code);
        var result = new StartRegistrationResult
        {
            Succeeded = true,
            Message = "Verification code sent.",
            UserId = user.Id,
            User = user.ToUserAccountDto()
        };
        return ResponseResult<StartRegistrationResult>.Success(result, result.Message);
    }

    public async Task<ResponseResult<ConfirmEmailCodeResult>> ConfirmEmailCodeAsync(ConfirmEmailCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
            return ResponseResult<ConfirmEmailCodeResult>.Failure("Email and code are required.");

        var userResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!userResult.Succeeded || userResult.Result == null)
            return ResponseResult<ConfirmEmailCodeResult>.Failure("User not found.");

        var codeResult = await _accountRepository.GetSavedVerificationCodeAsync(userResult.Result);
        if (!codeResult.Succeeded || codeResult.Result == null)
            return ResponseResult<ConfirmEmailCodeResult>.Failure("Verification code not found.");

        if (codeResult.Result != request.Code)
            return ResponseResult<ConfirmEmailCodeResult>.Failure("Invalid verification code.");

        var confirmResult = await _accountRepository.ConfirmEmailAsync(userResult.Result, request.Code);
        if (!confirmResult.Succeeded)
            return ResponseResult<ConfirmEmailCodeResult>.Failure(confirmResult.Message ?? "Failed to confirm email.");

        var result = new ConfirmEmailCodeResult
        {
            Succeeded = true,
            Message = "Email confirmed.",
            UserId = userResult.Result.Id,
            User = userResult.Result.ToUserAccountDto()
        };
        return ResponseResult<ConfirmEmailCodeResult>.Success(result, result.Message);
    }

    public async Task<ResponseResult<CompleteRegistrationResult>> CompleteRegistrationAsync(CompleteRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return ResponseResult<CompleteRegistrationResult>.Failure("Email and password are required.");

        var userResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!userResult.Succeeded || userResult.Result == null)
            return ResponseResult<CompleteRegistrationResult>.Failure("User not found.");

        var user = userResult.Result;
        if (!user.EmailConfirmed)
            return ResponseResult<CompleteRegistrationResult>.Failure("Email not confirmed.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _accountRepository.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return ResponseResult<CompleteRegistrationResult>.Failure(updateResult.Message ?? "Failed to complete registration.");

        await _eventPublisher.PublishAccountCreatedEventAsync(user.Id.ToString(), user.Email);

        var result = new CompleteRegistrationResult
        {
            Succeeded = true,
            Message = "Registration complete.",
            UserId = user.Id,
            User = user.ToUserAccountDto()
        };
        return ResponseResult<CompleteRegistrationResult>.Success(result, result.Message);
    }


    public async Task<ResponseResult<ValidateCredentialsResult>> ValidateCredentialsAsync(ValidateCredentialsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return ResponseResult<ValidateCredentialsResult>.Failure("Email and password must be provided.");

        var userResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!userResult.Succeeded)
            return ResponseResult<ValidateCredentialsResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<ValidateCredentialsResult>.Failure("Invalid credentials.");

        var user = userResult.Result;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ResponseResult<ValidateCredentialsResult>.Failure("Invalid credentials.");

        if (!user.EmailConfirmed)
            return ResponseResult<ValidateCredentialsResult>.Failure("Email not confirmed.");

        var result = user.ToValidateCredentialsResult();
        return ResponseResult<ValidateCredentialsResult>.Success(result);
    }

    public async Task<ResponseResult<GetAccountResult>> GetAccountByIdAsync(Guid userId)
    {
        var userResult = await _accountRepository.GetByIdAsync(userId);
        if (!userResult.Succeeded)
            return ResponseResult<GetAccountResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<GetAccountResult>.Failure("User not found.");

        return ResponseResult<GetAccountResult>.Success(userResult.Result.ToGetAccountResult());
    }

    public async Task<ResponseResult<GetAccountResult>> GetAccountByEmailAsync(string email)
    {
        var userResult = await _accountRepository.GetByEmailAsync(email);
        if (!userResult.Succeeded)
            return ResponseResult<GetAccountResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<GetAccountResult>.Failure("User not found.");

        return ResponseResult<GetAccountResult>.Success(userResult.Result.ToGetAccountResult());
    }

    public async Task<ResponseResult<GenerateTokenResult>> GenerateNewEmailConfirmationCodeAsync(Guid userId)
    {
        var userResult = await _accountRepository.GetByIdAsync(userId);
        if (!userResult.Succeeded)
            return ResponseResult<GenerateTokenResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<GenerateTokenResult>.Failure("User not found.");

        var user = userResult.Result;
        var code = GenerateSixDigitCode();
        var saveCodeResult = await _accountRepository.SaveVerificationCodeAsync(user, code);
        if (!saveCodeResult.Succeeded)
            return ResponseResult<GenerateTokenResult>.Failure(saveCodeResult.Message ?? "Failed to save verification code.");

        var response = new GenerateTokenResult
        {
            Succeeded = true,
            Token = code,
            Message = "New email confirmation code generated."
        };
        return ResponseResult<GenerateTokenResult>.Success(response);
    }

    public async Task<ResponseResult<UpdateUserResult>> UpdateUserAsync(Guid userId, UpdateUserRequest? request)
    {
        if (request == null)
            return ResponseResult<UpdateUserResult>.Failure("Invalid request body.");

        var userResult = await _accountRepository.GetByIdAsync(userId);
        if (!userResult.Succeeded)
            return ResponseResult<UpdateUserResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<UpdateUserResult>.Failure("User not found.");

        var user = userResult.Result;
        if (request.FirstName != null) user.FirstName = request.FirstName;
        if (request.LastName != null) user.LastName = request.LastName;
        if (request.Email != null) user.Email = request.Email;
        if (request.EmailConfirmed.HasValue) user.EmailConfirmed = request.EmailConfirmed.Value;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _accountRepository.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return ResponseResult<UpdateUserResult>.Failure(updateResult.Message ?? "Failed to update user.");

        return ResponseResult<UpdateUserResult>.Success(user.ToUpdateUserResult());
    }

    public async Task<ResponseResult<ForgotPasswordResult>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return ResponseResult<ForgotPasswordResult>.Failure("Email is required.");

        var userResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!userResult.Succeeded)
            return ResponseResult<ForgotPasswordResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null || !userResult.Result.EmailConfirmed)
        {
            // Don't reveal that the user does not exist or is not confirmed
            return ResponseResult<ForgotPasswordResult>.Success(new ForgotPasswordResult { Succeeded = true, Message = "If your email is registered, a password reset link will be sent." });
        }

        var user = userResult.Result;
        var tokenResult = await _accountRepository.GeneratePasswordResetTokenAsync(user);
        if (!tokenResult.Succeeded)
            return ResponseResult<ForgotPasswordResult>.Failure(tokenResult.Message ?? "Failed to generate password reset token.");

        if (tokenResult.Result != null)
            await _eventPublisher.PublishPasswordResetRequestedEventAsync(user.Id.ToString(), user.Email, tokenResult.Result);
        return ResponseResult<ForgotPasswordResult>.Success(new ForgotPasswordResult { Succeeded = true, Message = "If your email is registered, a password reset link will be sent." });
    }

    public async Task<ResponseResult<ResetPasswordResult>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Token) || 
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return ResponseResult<ResetPasswordResult>.Failure("Email, token, and new password are required.");
        }

        var userResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!userResult.Succeeded)
            return ResponseResult<ResetPasswordResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<ResetPasswordResult>.Failure("Password reset failed.");

        var user = userResult.Result;
        var resetResult = await _accountRepository.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!resetResult.Succeeded)
            return ResponseResult<ResetPasswordResult>.Failure(resetResult.Message ?? "Password reset failed.");

        var resetResponse = new ResetPasswordResult
        {
            Succeeded = true,
            Message = "Password has been reset successfully."
        };
        return ResponseResult<ResetPasswordResult>.Success(resetResponse);
    }

    public async Task<ResponseResult<bool>> DeleteAccountAsync(Guid userId)
    {
        var userResult = await _accountRepository.GetByIdAsync(userId);
        if (!userResult.Succeeded)
            return ResponseResult<bool>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<bool>.Failure("User not found.");

        var user = userResult.Result;
        var deleteResult = await _accountRepository.DeleteAsync(user.Id);
        if (!deleteResult.Succeeded)
            return ResponseResult<bool>.Failure(deleteResult.Message ?? "Failed to delete account.");

        await _eventPublisher.PublishAccountDeletedEventAsync(user.Id.ToString(), user.Email);
        return ResponseResult<bool>.Success(true);
    }

    private static string GenerateSixDigitCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}

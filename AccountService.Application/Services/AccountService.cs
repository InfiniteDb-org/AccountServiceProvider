using AccountService.Contracts.Requests;
using AccountService.Contracts.Responses;
using Application.Interfaces;
using Application.Mappers;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAccountService
{
    Task<ResponseResult<CreateAccountResult>> CreateAccountAsync(CreateAccountRequest request);
    Task<ResponseResult<ValidateCredentialsResult>> ValidateCredentialsAsync(ValidateCredentialsRequest request);
    Task<ResponseResult<GetAccountResult>> GetAccountByIdAsync(Guid userId);
    Task<ResponseResult<GetAccountResult>> GetAccountByEmailAsync(string email);
    Task<ResponseResult<ConfirmEmailResult>> ConfirmEmailAsync(Guid userId, ConfirmEmailRequest request);
    Task<ResponseResult<GenerateTokenResult>> GenerateNewEmailConfirmationCodeAsync(Guid userId);
    Task<ResponseResult<UpdateUserResult>> UpdateUserAsync(Guid userId, UpdateUserRequest? request);
    Task<ResponseResult<bool>> DeleteAccountAsync(Guid userId);
    Task<ResponseResult<ForgotPasswordResult>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ResponseResult<ResetPasswordResult>> ResetPasswordAsync(ResetPasswordRequest request);
}

public class AccountService( IAccountRepository accountRepository, IEventPublisher eventPublisher, ILogger<AccountService> logger) : IAccountService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<AccountService> _logger = logger;

    public async Task<ResponseResult<CreateAccountResult>> CreateAccountAsync(CreateAccountRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return ResponseResult<CreateAccountResult>.Failure("Email and password are required.");

        var existingUserResult = await _accountRepository.GetByEmailAsync(request.Email);
        if (!existingUserResult.Succeeded)
            return ResponseResult<CreateAccountResult>.Failure(existingUserResult.Message ?? "Failed to check for existing user.");

        if (existingUserResult.Result != null)
            return ResponseResult<CreateAccountResult>.Failure("An account with this email already exists.");

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        // Passera lösenordet som andra argument till CreateAsync pga repository-signaturen
        var createResult = await _accountRepository.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogWarning("CreateAsync failed for email: {Email}. Reason: {Reason}", request.Email, createResult.Message);
            return ResponseResult<CreateAccountResult>.Failure(createResult.Message ?? "Failed to create account. Please try again.");
        }

        // Generate email confirmation code and publish VerificationCodeSent event
        var code = GenerateSixDigitCode();
        var saveCodeResult = await _accountRepository.SaveVerificationCodeAsync(user, code);
        if (!saveCodeResult.Succeeded)
            return ResponseResult<CreateAccountResult>.Failure(saveCodeResult.Message ?? "Failed to save verification code.");

        await _eventPublisher.PublishVerificationCodeSentEventAsync(user.Id.ToString(), user.Email, code);

        var successResponse = new CreateAccountResult
        {
            Succeeded = true,
            Message = "Account created successfully.",
            UserId = user.Id
        };
        return ResponseResult<CreateAccountResult>.Success(successResponse);
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

        return ResponseResult<ValidateCredentialsResult>.Success(user.ToValidateCredentialsResult());
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

    public async Task<ResponseResult<ConfirmEmailResult>> ConfirmEmailAsync(Guid userId, ConfirmEmailRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
            return ResponseResult<ConfirmEmailResult>.Failure("Token is required.");

        var userResult = await _accountRepository.GetByIdAsync(userId);
        if (!userResult.Succeeded)
            return ResponseResult<ConfirmEmailResult>.Failure(userResult.Message ?? "Failed to retrieve user.");

        if (userResult.Result == null)
            return ResponseResult<ConfirmEmailResult>.Failure("User not found.");

        var user = userResult.Result;
        var codeResult = await _accountRepository.GetSavedVerificationCodeAsync(user);
        if (!codeResult.Succeeded)
            return ResponseResult<ConfirmEmailResult>.Failure(codeResult.Message ?? "Failed to retrieve verification code.");

        if (codeResult.Result == null || codeResult.Result != request.Token)
            return ResponseResult<ConfirmEmailResult>.Failure("Invalid or expired verification code.");

        var confirmResult = await _accountRepository.ConfirmEmailAsync(user, request.Token);
        if (!confirmResult.Succeeded)
            return ResponseResult<ConfirmEmailResult>.Failure(confirmResult.Message ?? "Failed to confirm email.");

        await _eventPublisher.PublishAccountCreatedEventAsync(user.Id.ToString(), user.Email);
        var confirmResponse = new ConfirmEmailResult
        {
            Succeeded = true,
            Message = "Email confirmed successfully."
        };
        return ResponseResult<ConfirmEmailResult>.Success(confirmResponse);
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

using AccountService.Api.Helpers;
using AccountService.Contracts.Requests;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AccountService.Api.Functions;

public class AccountFunctions(ILogger<AccountFunctions> logger, IAccountService accountService)
{
    private readonly ILogger<AccountFunctions> _logger = logger;
    private readonly IAccountService _accountService = accountService;
    
    [Function("StartRegistration")]
    public async Task<IActionResult> StartRegistration(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/start-registration")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<StartRegistrationRequest>(req, _logger);
            if (!requestResult.Succeeded)
                return ActionResultHelper.CreateResponse(requestResult);

            var response = await _accountService.StartRegistrationAsync(requestResult.Data!);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StartRegistration");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("ConfirmEmailCode")]
    public async Task<IActionResult> ConfirmEmailCode(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/confirm-email-code")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<ConfirmEmailCodeRequest>(req, _logger);
            if (!requestResult.Succeeded)
                return ActionResultHelper.CreateResponse(requestResult);

            var response = await _accountService.ConfirmEmailCodeAsync(requestResult.Data!);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ConfirmEmailCode");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("CompleteRegistration")]
    public async Task<IActionResult> CompleteRegistration(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/complete-registration")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<CompleteRegistrationRequest>(req, _logger);
            if (!requestResult.Succeeded)
                return ActionResultHelper.CreateResponse(requestResult);

            var response = await _accountService.CompleteRegistrationAsync(requestResult.Data!);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CompleteRegistration");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }
    
    [Function("ValidateCredentials")]
    public async Task<IActionResult> ValidateCredentials(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/validate")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<ValidateCredentialsRequest>(req, _logger);
            if (!requestResult.Succeeded)
            {
                return ActionResultHelper.CreateResponse(requestResult);
            }

            var response = await _accountService.ValidateCredentialsAsync(requestResult.Data!);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("GetAccountById")]
    public async Task<IActionResult> GetAccountById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{userId:guid}")] HttpRequest req,
        Guid userId)
    {
        try
        {
            var response = await _accountService.GetAccountByIdAsync(userId);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by ID");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("GetAccountByEmail")]
    public async Task<IActionResult> GetAccountByEmail(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/by-email/{email}")] HttpRequest req,
        string email)
    {
        try
        {
            var response = await _accountService.GetAccountByEmailAsync(email);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by email");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("GenerateNewEmailConfirmationToken")]
    public async Task<IActionResult> GenerateEmailConfirmationToken(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/{userId:guid}/email-confirmation-token")] HttpRequest req,
        Guid userId)
    {
        try
        {
            var response = await _accountService.GenerateNewEmailConfirmationCodeAsync(userId);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating email confirmation token");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("UpdateUser")]
    public async Task<IActionResult> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{userId:guid}")] HttpRequest req,
        Guid userId)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<UpdateUserRequest>(req, _logger);
            if (!requestResult.Succeeded)
            {
                return ActionResultHelper.CreateResponse(requestResult);
            }

            var response = await _accountService.UpdateUserAsync(userId, requestResult.Data!);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/forgot-password")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<ForgotPasswordRequest>(req, _logger);
            if (!requestResult.Succeeded)
            {
                return ActionResultHelper.CreateResponse(requestResult);
            }

            var response = await _accountService.ForgotPasswordAsync(requestResult.Data!);
            
            // Always return success for security reasons, even if there's an error
            // This prevents user enumeration attacks
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }

    [Function("ResetPassword")]
    public async Task<IActionResult> ResetPassword(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/reset-password")] HttpRequest req)
    {
        try
        {
            var requestResult = await RequestBodyHelper.ReadAndValidateRequestBody<ResetPasswordRequest>(req, _logger);
            if (!requestResult.Succeeded)
            {
                return ActionResultHelper.CreateResponse(requestResult);
            }

            var response = await _accountService.ResetPasswordAsync(requestResult.Data!);
            
            if (!response.Succeeded)
            {
                // For security reasons, don't reveal too much information
                return ActionResultHelper.BadRequest("Password reset failed.");
            }

            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }
    
    [Function("DeleteAccount")]
    public async Task<IActionResult> DeleteAccount(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "accounts/{userId:guid}")] HttpRequest req,
        Guid userId)
    {
        try
        {
            var response = await _accountService.DeleteAccountAsync(userId);
            return ActionResultHelper.CreateResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account");
            return ActionResultHelper.BadRequest("Internal server error");
        }
    }
    
}
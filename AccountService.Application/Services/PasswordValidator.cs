using System.Text.RegularExpressions;
using AccountService.Contracts.Responses;
using Application.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Services;

public interface IPasswordValidator
{
    PasswordValidationResult Validate(string password);
}

public partial class PasswordValidator(IOptions<PasswordPolicyOptions> policy) : IPasswordValidator
{
    private readonly PasswordPolicyOptions _policy = policy.Value;

    public PasswordValidationResult Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordValidationResult.Failure("Password is required.");
        }

        var errors = new List<string>();

        // Check minimum length
        if (password.Length < _policy.MinLength)
        {
            errors.Add($"Password must be at least {_policy.MinLength} characters long.");
        }

        // Check maximum length
        if (password.Length > _policy.MaxLength)
        {
            errors.Add($"Password must not exceed {_policy.MaxLength} characters.");
        }

        // Check for lowercase letter
        if (_policy.RequireLowercase && !LowercaseRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one lowercase letter (a-z).");
        }

        // Check for uppercase letter
        if (_policy.RequireUppercase && !UppercaseRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one uppercase letter (A-Z).");
        }

        // Check for digit
        if (_policy.RequireDigit && !DigitRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one number (0-9).");
        }

        // Check for special character
        if (_policy.RequireSpecialChar && !SpecialCharRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one special character.");
        }

        if (errors.Count != 0)
        {
            var errorMessage = "Password does not meet requirements: " + string.Join(" ", errors);
            return PasswordValidationResult.Failure(errorMessage, errors);
        }

        return PasswordValidationResult.Success();
    }

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex DigitRegex();
    
    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowercaseRegex();
    
    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UppercaseRegex();
    
    [GeneratedRegex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]")]
    private static partial Regex SpecialCharRegex();
}

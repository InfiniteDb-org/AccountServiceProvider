using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.Providers;

public interface IEmailVerificationProvider
{
    Task<bool> VerifyCodeAsync(string email, string code);
}

// Verifies email codes using EmailVerificationService
public class EmailVerificationProvider(HttpClient httpClient, IConfiguration configuration) : IEmailVerificationProvider
{
    private readonly HttpClient _httpClient = httpClient;
    // URL for the verification endpoint, from config
    private readonly string _verificationUrl = configuration["VerificationService:VerifyUrl"] 
                                               ?? throw new ArgumentNullException("VerificationService:VerifyUrl");

    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        // Send email/code to verification service
        var verifyRequest = new { Email = email, Code = code };
        var response = await _httpClient.PostAsJsonAsync(_verificationUrl, verifyRequest);
        var resultString = await response.Content.ReadAsStringAsync();
        return resultString.Contains("valid", StringComparison.OrdinalIgnoreCase) && !resultString.Contains("invalid", StringComparison.OrdinalIgnoreCase);
    }
}
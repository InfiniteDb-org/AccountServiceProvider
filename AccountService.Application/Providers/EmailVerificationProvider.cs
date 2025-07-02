using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.Providers;

public interface IEmailVerificationProvider
{
    Task<bool> VerifyCodeAsync(string email, string code);
}

public class EmailVerificationProvider(HttpClient httpClient, IConfiguration configuration) : IEmailVerificationProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _verificationUrl = configuration["VerificationService:VerifyUrl"] 
                                               ?? throw new ArgumentNullException("VerificationService:VerifyUrl");

    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        var verifyRequest = new { Email = email, Code = code };
        var response = await _httpClient.PostAsJsonAsync(_verificationUrl, verifyRequest);
        var resultString = await response.Content.ReadAsStringAsync();
        return resultString.Contains("valid", StringComparison.OrdinalIgnoreCase) && !resultString.Contains("invalid", StringComparison.OrdinalIgnoreCase);
    }
}
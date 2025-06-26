using AccountService.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace AccountService.Api.Helpers;

// Helper class for reading and validating HTTP request bodies
public static class RequestBodyHelper
{
    // Reads and validates the request body, deserializing it to the specified type
    public static async Task<ResponseResult<T>> ReadAndValidateRequestBody<T>(HttpRequest req, ILogger logger)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        if (string.IsNullOrEmpty(body))
        {
            logger.LogWarning("Request body is empty.");
            return ResponseResult<T>.Failure("Request body is empty.");
        }

        try
        {
            var request = JsonConvert.DeserializeObject<T>(body);
            if (request == null)
            {
                return ResponseResult<T>.Failure("Invalid request format.");
            }
            
            return ResponseResult<T>.Success(request);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize request body.");
            return ResponseResult<T>.Failure("Invalid JSON format in request body.");
        }
    }
}
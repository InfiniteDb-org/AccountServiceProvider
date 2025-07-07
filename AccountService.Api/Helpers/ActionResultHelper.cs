using AccountService.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Api.Helpers;

// centralizes mapping of domain result to HTTP response for consistency
public static class ActionResultHelper
{
    // helpers to ensure all responses are shaped the same way
    private static OkObjectResult Ok<T>(ResponseResult<T> result) => new(result);
    private static OkObjectResult Ok(ResponseResult result) => new(result);
    public static BadRequestObjectResult BadRequest(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static NotFoundObjectResult NotFound(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static ConflictObjectResult Conflict(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static UnauthorizedObjectResult Unauthorized(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    
    // Returns HTTP status based on domain error semantics, not just success/failure
    public static IActionResult CreateResponse<T>(ResponseResult<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }
        
        var message = result.Message?.ToLowerInvariant();
        
        if (message?.Contains("not found") == true)
            return NotFound(result.Message);
        
        if (message?.Contains("invalid credentials") == true)
            return Unauthorized(result.Message);
        
        if (message?.Contains("already exists") == true)
            return Conflict(result.Message);
        
        return BadRequest(result.Message);
    }
    
    // Same as above, for non-generic ResponseResult
    public static IActionResult CreateResponse(ResponseResult result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }
        
        var message = result.Message?.ToLowerInvariant();
        
        if (message?.Contains("not found") == true)
            return NotFound(result.Message);
        
        if (message?.Contains("invalid credentials") == true)
            return Unauthorized(result.Message);
        
        if (message?.Contains("already exists") == true)
            return Conflict(result.Message);
        
        return BadRequest(result.Message);
    }
}

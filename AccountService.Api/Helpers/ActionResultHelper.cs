using AccountService.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Api.Helpers;

// Helper class for creating standardized IActionResult responses from ResponseResult objects
public static class ActionResultHelper
{
    // Helper methods for creating standardized responses
    private static OkObjectResult Ok<T>(ResponseResult<T> result) => new(result);
    private static OkObjectResult Ok(ResponseResult result) => new(result);
    public static BadRequestObjectResult BadRequest(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static NotFoundObjectResult NotFound(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static ConflictObjectResult Conflict(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    private static UnauthorizedObjectResult Unauthorized(string? message) => new(new SimpleResponseResult { Succeeded = false, Message = message });
    
    
    // Creates an appropriate IActionResult based on the ResponseResult status and message content
    public static IActionResult CreateResponse<T>(ResponseResult<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }
        
        // Map error messages to appropriate status codes
        var message = result.Message?.ToLowerInvariant();
        
        if (message?.Contains("not found") == true)
            return NotFound(result.Message);
        
        if (message?.Contains("invalid credentials") == true)
            return Unauthorized(result.Message);
        
        if (message?.Contains("already exists") == true)
            return Conflict(result.Message);
        
        return BadRequest(result.Message);
    }
    
    
    // Creates an appropriate IActionResult based on the ResponseResult status and message content
    public static IActionResult CreateResponse(ResponseResult result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }
        
        // Map error messages to appropriate status codes
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

namespace AccountService.Contracts.DTOs;

public class UserAccountDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public bool EmailConfirmed { get; set; }
}
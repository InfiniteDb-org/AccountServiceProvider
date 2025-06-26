namespace Application.Models;

public class EmailVerificationCode
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
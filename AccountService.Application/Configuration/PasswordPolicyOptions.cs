namespace Application.Configuration;

// configure password policy from configuration (appsettings.json)
public class PasswordPolicyOptions
{
    public const string SectionName = "PasswordPolicy";
    
    public int MinLength { get; set; } = 8;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = false;
    public int MaxLength { get; set; } = 64;
}

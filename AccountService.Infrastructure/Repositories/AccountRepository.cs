using AccountService.Contracts.Responses;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BCrypt.Net.BCrypt;

namespace Infrastructure.Repositories;

public class AccountRepository(AppDbContext dbContext, ILogger<AccountRepository> logger) : IAccountRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<AccountRepository> _logger = logger;

    public async Task<RepositoryResult<User>> CreateAsync(User user, string password)
    {
        user.PasswordHash = await Task.Run(() => HashPassword(password));
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return RepositoryResult<User>.Success(user, "User created");
    }

    public async Task<RepositoryResult<User>> GetByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        return user == null
            ? RepositoryResult<User>.Fail("User not found")
            : RepositoryResult<User>.Success(user, "User found");
    }

    public async Task<RepositoryResult<User>> GetByEmailAsync(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null
            ? RepositoryResult<User>.Fail("User not found")
            : RepositoryResult<User>.Success(user, "User found");
    }

    public async Task<IEnumerable<RepositoryResult<User>>> GetAllAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        return users.Select(u => RepositoryResult<User>.Success(u, "User found"));
    }

    public async Task<RepositoryResult<User>> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return RepositoryResult<User>.Success(user, "User updated");
    }

    public async Task<RepositoryResult<User>> DeleteAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
            return RepositoryResult<User>.Fail("User not found");
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return RepositoryResult<User>.Success(user, "User deleted");
    }

    public async Task<RepositoryResult<bool>> ConfirmEmailAsync(User user, string token)
    {
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return RepositoryResult<bool>.Success(true, "Email confirmed");
    }

    public async Task<RepositoryResult<string>> GeneratePasswordResetTokenAsync(User user)
    {
        try
        {
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddDays(1);
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
            return RepositoryResult<string>.Success(token, "Password reset token generated");
        }
        catch (Exception ex)
        {
            return RepositoryResult<string>.Fail(ex.Message);
        }
    }

    public async Task<RepositoryResult<bool>> ResetPasswordAsync(User user, string token, string newPassword)
    {
        try
        {
            _logger.LogWarning("[ResetPasswordAsync] Email={Email}, Request-Token={RequestToken}, DB-Token={DbToken}, DB-Expires={DbExpires}, Now={Now}",
                user.Email, token, user.PasswordResetToken, user.PasswordResetTokenExpires, DateTime.UtcNow);

            if (user.PasswordResetToken != token ||
                user.PasswordResetTokenExpires == null ||
                user.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                _logger.LogWarning("[ResetPasswordAsync] Token validation failed!");
                return RepositoryResult<bool>.Fail("Invalid password reset token");
            }
            
            user.PasswordHash = await Task.Run(() => HashPassword(newPassword));
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            user.UpdatedAt = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            _logger.LogWarning("[ResetPasswordAsync] Password reset successful!");
            return RepositoryResult<bool>.Success(true, "Password reset successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ResetPasswordAsync] Exception: {Message}", ex.Message);
            return RepositoryResult<bool>.Fail(ex.Message);
        }
    }
}
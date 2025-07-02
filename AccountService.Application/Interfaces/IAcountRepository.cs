using AccountService.Contracts.Responses;
using Application.Models;

namespace Application.Interfaces;

public interface IAccountRepository
{
    Task<RepositoryResult<User>> CreateAsync(User user, string password);
    Task<RepositoryResult<User>> GetByIdAsync(Guid id);
    Task<RepositoryResult<User>> GetByEmailAsync(string email);
    Task<IEnumerable<RepositoryResult<User>>> GetAllAsync();
    Task<RepositoryResult<User>>UpdateAsync(User user);
    Task<RepositoryResult<User>>DeleteAsync(Guid id);
    
    Task<RepositoryResult<bool>> ConfirmEmailAsync(User user, string token);
    
    Task<RepositoryResult<string>> GeneratePasswordResetTokenAsync(User user);
    Task<RepositoryResult<bool>> ResetPasswordAsync(User user, string token, string newPassword);
}
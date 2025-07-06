using AccountService.Contracts.Events;
using AccountService.Contracts.Requests;
using AccountService.Contracts.Responses;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Moq;

namespace AccountService_Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _repoMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<Application.Providers.IEmailVerificationProvider> _emailVerificationProviderMock = new();
    private readonly Mock<IPasswordValidator> _passwordValidatorMock = new();
    private readonly Application.Services.AccountService _service;

    public AccountServiceTests()
    {
        _service = new Application.Services.AccountService(
            _repoMock.Object, 
            _eventPublisherMock.Object, 
            Mock.Of<Microsoft.Extensions.Logging.ILogger<Application.Services.AccountService>>(), 
            _emailVerificationProviderMock.Object,
            _passwordValidatorMock.Object);
    }

    [Fact]
    public async Task StartRegistrationAsync_ShouldReturnSuccess_ForNewUser()
    {
        // Arrange
        var email = "test@example.com";
        var request = new StartRegistrationRequest { Email = email };
        _repoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = false });
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "")).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = new User { Email = email } });

        // Act
        var result = await _service.StartRegistrationAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        _eventPublisherMock.Verify(e => e.PublishVerificationCodeRequestedEventAsync(
                It.Is<VerificationCodeRequestedEvent>(evt => evt.Email == email)), Times.Once
            );
    }

    [Fact]
    public async Task StartRegistrationAsync_ShouldFail_ForExistingUser()
    {
        // Arrange
        var email = "existing@example.com";
        var request = new StartRegistrationRequest { Email = email };
        _repoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = new User { Email = email } });

        // Act
        var result = await _service.StartRegistrationAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Account already exists.", result.Message);
        // Senior: Ensure no event is published for duplicate registration
        _eventPublisherMock.Verify(e =>
                e.PublishVerificationCodeRequestedEventAsync(It.IsAny<VerificationCodeRequestedEvent>()),
            Times.Never
        );
    }

    [Fact]
    public async Task StartRegistrationAsync_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new StartRegistrationRequest { Email = "" };
        // Act
        var result = await _service.StartRegistrationAsync(request);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Email is required.", result.Message);
    }

    [Fact]
    public async Task StartRegistrationAsync_ShouldHandle_RepositoryFailure()
    {
        // Arrange
        var email = "fail@example.com";
        var request = new StartRegistrationRequest { Email = email };
        _repoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = false });
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "")).ReturnsAsync(new RepositoryResult<User> { Succeeded = false, Message = "DB error" });
        // Act
        var result = await _service.StartRegistrationAsync(request);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("DB error", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailCodeAsync_ShouldFail_WhenEmailOrCodeMissing()
    {
        // Arrange
        var request = new ConfirmEmailCodeRequest { Email = "", Code = "" };
        // Act
        var result = await _service.ConfirmEmailCodeAsync(request);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Email and code are required.", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailCodeAsync_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var request = new ConfirmEmailCodeRequest { Email = "nouser@example.com", Code = "123456" };
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = null });
        // Act
        var result = await _service.ConfirmEmailCodeAsync(request);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailCodeAsync_ShouldFail_WhenCodeIsWrong()
    {
        // Arrange
        var request = new ConfirmEmailCodeRequest { Email = "user@example.com", Code = "wrongcode" };
        var user = new User { Email = request.Email };
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = user });
        _emailVerificationProviderMock.Setup(x => x.VerifyCodeAsync(request.Email, request.Code)).ReturnsAsync(false);
        // Act
        var result = await _service.ConfirmEmailCodeAsync(request);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid verification code.", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailCodeAsync_ShouldSucceed_WithCorrectCode()
    {
        // Arrange
        var request = new ConfirmEmailCodeRequest { Email = "user@example.com", Code = "123456" };
        var user = new User { Email = request.Email };
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = user });
        _repoMock.Setup(r => r.ConfirmEmailAsync(user, "123456")).ReturnsAsync(new RepositoryResult<bool> { Succeeded = true });
        _emailVerificationProviderMock.Setup(x => x.VerifyCodeAsync(request.Email, request.Code)).ReturnsAsync(true);
        // Act
        var result = await _service.ConfirmEmailCodeAsync(request);
        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WithInvalidPassword()
    {
        // Arrange
        var request = new CompleteRegistrationRequest { Email = "user@example.com", Password = "weak" };
        var passwordValidationResult = new PasswordValidationResult 
        { 
            Succeeded = false, 
            Message = "Password does not meet requirements" 
        };
        _passwordValidatorMock.Setup(x => x.Validate(request.Password)).Returns(passwordValidationResult);

        // Act
        var result = await _service.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Password does not meet requirements", result.Message);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldSucceed_WithValidPassword()
    {
        // Arrange
        var request = new CompleteRegistrationRequest { Email = "user@example.com", Password = "ValidPass123" };
        var user = new User { Email = request.Email, EmailConfirmed = true };
        var passwordValidationResult = new PasswordValidationResult { Succeeded = true };
        
        _passwordValidatorMock.Setup(x => x.Validate(request.Password)).Returns(passwordValidationResult);
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = user });
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = user });

        // Act
        var result = await _service.CompleteRegistrationAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        _passwordValidatorMock.Verify(x => x.Validate(request.Password), Times.Once);
    }

    [Theory]
    [InlineData("short", "Password does not meet requirements")]
    [InlineData("nouppercase123", "Password does not meet requirements")]
    [InlineData("NOLOWERCASE123", "Password does not meet requirements")]
    [InlineData("NoNumbers", "Password does not meet requirements")]
    public async Task CompleteRegistrationAsync_ShouldFail_WithInvalidPasswords(string password, string expectedError)
    {
        // Arrange
        var request = new CompleteRegistrationRequest { Email = "user@example.com", Password = password };
        var passwordValidationResult = new PasswordValidationResult 
        { 
            Succeeded = false, 
            Message = expectedError 
        };
        _passwordValidatorMock.Setup(x => x.Validate(request.Password)).Returns(passwordValidationResult);

        // Act
        var result = await _service.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(expectedError, result.Message);
        _passwordValidatorMock.Verify(x => x.Validate(request.Password), Times.Once);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WithEmptyPassword()
    {
        // Arrange
        var request = new CompleteRegistrationRequest { Email = "user@example.com", Password = "" };

        // Act
        var result = await _service.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Email and password are required.", result.Message);
        // PasswordValidator should NOT be called for empty password
        _passwordValidatorMock.Verify(x => x.Validate(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldFail_WithInvalidPassword()
    {
        // Arrange
        var request = new ResetPasswordRequest { Email = "user@example.com", ResetToken = "valid-token", NewPassword = "weak" };
        var passwordValidationResult = new PasswordValidationResult 
        { 
            Succeeded = false, 
            Message = "Password does not meet requirements" 
        };
        _passwordValidatorMock.Setup(x => x.Validate(request.NewPassword)).Returns(passwordValidationResult);

        // Act
        var result = await _service.ResetPasswordAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Password does not meet requirements", result.Message);
        _passwordValidatorMock.Verify(x => x.Validate(request.NewPassword), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldSucceed_WithValidPassword()
    {
        // Arrange
        var request = new ResetPasswordRequest { Email = "user@example.com", ResetToken = "valid-token", NewPassword = "StrongPass123" };
        var user = new User { Email = request.Email };
        var passwordValidationResult = new PasswordValidationResult { Succeeded = true };
        
        _passwordValidatorMock.Setup(x => x.Validate(request.NewPassword)).Returns(passwordValidationResult);
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = user });
        _repoMock.Setup(r => r.ResetPasswordAsync(user, request.ResetToken, request.NewPassword))
                 .ReturnsAsync(new RepositoryResult<bool> { Succeeded = true, Result = true });

        // Act
        var result = await _service.ResetPasswordAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        _passwordValidatorMock.Verify(x => x.Validate(request.NewPassword), Times.Once);
    }
}
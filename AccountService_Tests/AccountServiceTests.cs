using AccountService.Contracts.Requests;
using AccountService.Contracts.Responses;
using Application.Interfaces;
using Application.Models;
using Moq;

namespace AccountService_Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _repoMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Application.Services.AccountService _service;

    public AccountServiceTests()
    {
        _service = new Application.Services.AccountService(_repoMock.Object, _eventPublisherMock.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<Application.Services.AccountService>>());
    }

    [Fact]
    public async Task StartRegistrationAsync_ShouldReturnSuccess_ForNewUser()
    {
        // Arrange
        var email = "test@example.com";
        var request = new StartRegistrationRequest { Email = email };
        _repoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = false });
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "")).ReturnsAsync(new RepositoryResult<User> { Succeeded = true, Result = new User { Email = email } });
        _repoMock.Setup(r => r.SaveVerificationCodeAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(new RepositoryResult<bool> { Succeeded = true });

        // Act
        var result = await _service.StartRegistrationAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        _eventPublisherMock.Verify(e => e.PublishVerificationCodeSentEventAsync(It.IsAny<string>(), email, It.IsAny<string>()), Times.Once);
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
        _eventPublisherMock.Verify(e => e.PublishVerificationCodeSentEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
        _repoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new RepositoryResult<User> { Succeeded = false });
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
        _repoMock.Setup(r => r.GetSavedVerificationCodeAsync(user))!.ReturnsAsync(new RepositoryResult<string> { Succeeded = true, Result = "correctcode" });
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
        _repoMock.Setup(r => r.GetSavedVerificationCodeAsync(user))!.ReturnsAsync(new RepositoryResult<string> { Succeeded = true, Result = "123456" });
        _repoMock.Setup(r => r.ConfirmEmailAsync(user, "123456")).ReturnsAsync(new RepositoryResult<bool> { Succeeded = true });
        // Act
        var result = await _service.ConfirmEmailCodeAsync(request);
        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
    }
}
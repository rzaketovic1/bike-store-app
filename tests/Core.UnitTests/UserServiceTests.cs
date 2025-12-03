using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Moq;
using System.Text;

namespace Core.UnitTests;
public class UserServiceTests
{
    private readonly Mock<IUsersRepository> _repo;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repo = new Mock<IUsersRepository>();
        _service = new UserService(_repo.Object);
    }

    // ----------------------------------------------------
    // CreateAsync
    // ----------------------------------------------------

    [Fact]
    public async Task CreateUser_ReturnsUser()
    {
        // Arrange
        string email = "test@gmail.com";
        string password = "Pass123";
        string displayName = "Ramiz";

        // Act
        var result = await _service.CreateAsync(email, password, displayName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(displayName, result.DisplayName);

        _repo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ----------------------------------------------------
    // GetByEmailAsync
    // ----------------------------------------------------

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var email = "user@test.com";
        var user = new User
        {
            Email = email,
            DisplayName = "Test User",
            PasswordHash = "hash"
        };

        _repo.Setup(r => r.GetByEmailAsync(email))
             .ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result!.Email);
        Assert.Equal("Test User", result.DisplayName);

        _repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "missing@test.com";

        _repo.Setup(r => r.GetByEmailAsync(email))
             .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
        _repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    // ----------------------------------------------------
    // AuthenticateAsync
    // ----------------------------------------------------

    [Fact]
    public async Task AuthenticateAsync_ReturnsUser_WhenPasswordMatches()
    {
        // Arrange
        var email = "login@test.com";
        var password = "Secret123";

        var user = new User
        {
            Email = email,
            DisplayName = "Login User",
            PasswordHash = Hash(password)
        };

        _repo.Setup(r => r.GetByEmailAsync(email))
             .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result!.Email);
        Assert.Equal("Login User", result.DisplayName);

        _repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var email = "unknown@test.com";

        _repo.Setup(r => r.GetByEmailAsync(email))
             .ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync(email, "anything");

        // Assert
        Assert.Null(result);
        _repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var email = "login@test.com";

        var user = new User
        {
            Email = email,
            DisplayName = "User",
            PasswordHash = Hash("CORRECT_PASSWORD")
        };

        _repo.Setup(r => r.GetByEmailAsync(email))
             .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, "WRONG_PASSWORD");

        // Assert
        Assert.Null(result);
        _repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    private static string Hash(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return System.Convert.ToBase64String(hash);
    }
}
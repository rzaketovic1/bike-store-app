using Core.Entities;
using Core.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Moq;

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

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateUserAndSaveChanges()
    {
        // Arrange
        var email = "test@gmail.com";
        var password = "Pass123";
        var displayName = "Test User";

        // Act
        var result = await _service.CreateAsync(email, password, displayName);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.DisplayName.Should().Be(displayName);
        result.PasswordHash.Should().NotBeNullOrEmpty();

        _repo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region GetByEmailAsync Tests

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var email = "user@test.com";
        var user = new User { Email = email, DisplayName = "Test User", PasswordHash = "hash" };

        _repo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _repo.Setup(r => r.GetByEmailAsync("missing@test.com")).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByEmailAsync("missing@test.com");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region AuthenticateAsync Tests

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "login@test.com";
        var password = "Secret123";
        var user = new User
        {
            Email = email,
            DisplayName = "Login User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _repo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = "login@test.com";
        var user = new User
        {
            Email = email,
            DisplayName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CORRECT_PASSWORD")
        };

        _repo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, "WRONG_PASSWORD");

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
using API.Controllers;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.UnitTests;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _controller = new AuthController(_userServiceMock.Object, _tokenServiceMock.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyInUse()
    {
        // Arrange
        _userServiceMock
            .Setup(s => s.GetByEmailAsync("existing@user.com"))
            .ReturnsAsync(new User { Email = "existing@user.com" });

        var dto = new UserRegisterDto
        {
            Email = "existing@user.com",
            Password = "Pass123!",
            DisplayName = "Test User"
        };

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var conflictResult = result.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        conflictResult.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenRegistrationSuccessful()
    {
        // Arrange
        _userServiceMock
            .Setup(s => s.GetByEmailAsync("new@user.com"))
            .ReturnsAsync((User?)null);

        var createdUser = new User
        {
            Id = 1,
            Email = "new@user.com",
            DisplayName = "New User",
            PasswordHash = "hash"
        };

        _userServiceMock
            .Setup(s => s.CreateAsync("new@user.com", "Pass123!", "New User"))
            .ReturnsAsync(createdUser);

        _tokenServiceMock
            .Setup(t => t.CreateToken(createdUser))
            .Returns("fake-jwt-token");

        var dto = new UserRegisterDto
        {
            Email = "new@user.com",
            Password = "Pass123!",
            DisplayName = "New User"
        };

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(201);

        var userDto = objectResult.Value.Should().BeOfType<UserDto>().Subject;
        userDto.Email.Should().Be("new@user.com");
        userDto.DisplayName.Should().Be("New User");
        userDto.Token.Should().Be("fake-jwt-token");
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "user@test.com",
            DisplayName = "Test User",
            PasswordHash = "hash"
        };

        _userServiceMock
            .Setup(s => s.AuthenticateAsync("user@test.com", "Pass123!"))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.CreateToken(user))
            .Returns("valid-jwt-token");

        var dto = new UserLoginDto
        {
            Email = "user@test.com",
            Password = "Pass123!"
        };

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var userDto = okResult.Value.Should().BeOfType<UserDto>().Subject;

        userDto.Email.Should().Be("user@test.com");
        userDto.DisplayName.Should().Be("Test User");
        userDto.Token.Should().Be("valid-jwt-token");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        _userServiceMock
            .Setup(s => s.AuthenticateAsync("user@test.com", "WrongPassword"))
            .ReturnsAsync((User?)null);

        var dto = new UserLoginDto
        {
            Email = "user@test.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var unauthorizedResult = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Login_ShouldCallAuthenticateAsync_WithCorrectParameters()
    {
        // Arrange
        _userServiceMock
            .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var dto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "MyPassword123"
        };

        // Act
        await _controller.Login(dto);

        // Assert
        _userServiceMock.Verify(s => s.AuthenticateAsync("test@example.com", "MyPassword123"), Times.Once);
    }

    #endregion
}

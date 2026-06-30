using API.Controllers;
using Application.Dtos;
using Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.UnitTests;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private AuthController CreateController()
        => new(_userServiceMock.Object, _tokenServiceMock.Object);

    [Fact]
    public async Task Register_ShouldReturnCreatedUserDto_WhenRegistrationSuccessful()
    {
        var user = new User
        {
            Id = 1,
            Email = "new@user.com",
            DisplayName = "New User",
            PasswordHash = "hash"
        };

        var dto = new UserRegisterDto
        {
            Email = user.Email,
            Password = "Pass123!",
            DisplayName = user.DisplayName
        };

        _userServiceMock
            .Setup(s => s.CreateAsync(dto.Email, dto.Password, dto.DisplayName))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.CreateToken(user))
            .Returns("fake-jwt-token");

        var result = await CreateController().Register(dto);

        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);

        var response = objectResult.Value.Should().BeOfType<UserDto>().Subject;
        response.Email.Should().Be(dto.Email);
        response.DisplayName.Should().Be(dto.DisplayName);
        response.Token.Should().Be("fake-jwt-token");
    }

    [Fact]
    public async Task Login_ShouldReturnUserDto_WhenCredentialsAreValid()
    {
        var user = new User
        {
            Id = 1,
            Email = "user@test.com",
            DisplayName = "Test User",
            PasswordHash = "hash"
        };

        var dto = new UserLoginDto
        {
            Email = user.Email,
            Password = "Pass123!"
        };

        _userServiceMock
            .Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.CreateToken(user))
            .Returns("valid-jwt-token");

        var result = await CreateController().Login(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<UserDto>().Subject;

        response.Email.Should().Be(dto.Email);
        response.DisplayName.Should().Be(user.DisplayName);
        response.Token.Should().Be("valid-jwt-token");
    }
}

using Core.Entities;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.UnitTests;

public class TokenServiceTests
{
    private readonly IConfiguration _config;
    private readonly TokenService _service;

    public TokenServiceTests()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "super_secret_test_key_1234567890",
            ["Jwt:Issuer"] = "BikeStore",
            ["Jwt:Audience"] = "BikeStoreClient",
            ["Jwt:DurationInMinutes"] = "60"
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _service = new TokenService(_config);
    }

    #region CreateToken Tests

    [Fact]
    public void CreateToken_ShouldReturnValidJwt_WithCorrectClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 42,
            Email = "test@example.com",
            DisplayName = "Test User",
            PasswordHash = "hash"
        };

        // Act
        var tokenString = _service.CreateToken(user);

        // Assert
        tokenString.Should().NotBeNullOrWhiteSpace();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value.Should().Be("test@example.com");
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value.Should().Be("Test User");
    }

    [Fact]
    public void CreateToken_ShouldThrow_WhenKeyNotConfigured()
    {
        // Arrange
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "BikeStore",
            ["Jwt:Audience"] = "BikeStoreClient",
            ["Jwt:DurationInMinutes"] = "60"
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var service = new TokenService(config);
        var user = new User { Id = 1, Email = "test@example.com", DisplayName = "Test", PasswordHash = "hash" };

        // Act
        var act = () => service.CreateToken(user);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Key not configured.");
    }

    #endregion
}

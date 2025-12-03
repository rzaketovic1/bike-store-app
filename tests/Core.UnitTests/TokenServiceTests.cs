using Core.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Core.UnitTests;
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

    [Fact]
    public void CreateToken_ReturnsJwtWithExpectedClaims()
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
        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenString);

        // issuer / audience
        Assert.Equal("BikeStore", jwt.Issuer);
        Assert.Contains("BikeStoreClient", jwt.Audiences);

        // claims
        Assert.Equal("test@example.com",
            jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);

        Assert.Equal("Test User",
            jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);

        Assert.Equal("42",
            jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

        // expiry ~ now + 60 min (dovoljno je okvirno)
        var now = DateTime.UtcNow;
        Assert.True(jwt.ValidTo > now.AddMinutes(55));
        Assert.True(jwt.ValidTo <= now.AddMinutes(65));
    }

    [Fact]
    public void CreateToken_Throws_WhenKeyNotConfigured()
    {
        // Arrange: config bez Jwt:Key
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "BikeStore",
            ["Jwt:Audience"] = "BikeStoreClient",
            ["Jwt:DurationInMinutes"] = "60"
        };

        var badConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var service = new TokenService(badConfig);

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "User",
            PasswordHash = "hash"
        };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => service.CreateToken(user));
        Assert.Equal("JWT Key not configured.", ex.Message);
    }
}
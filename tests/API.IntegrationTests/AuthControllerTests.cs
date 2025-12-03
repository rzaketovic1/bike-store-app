using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Core.Dtos;

namespace API.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WithUserDto_WhenDataIsValid()
        {
            var email = $"new-{Guid.NewGuid():N}@example.com";

            var dto = new
            {
                email,
                password = "Pass123!",
                displayName = "New User"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Should().NotBeNull();
            user!.Email.Should().Be(email);
            user.DisplayName.Should().Be("New User");
            user.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            var email = $"test-{Guid.NewGuid():N}@example.com";

            var dto = new
            {
                email,
                password = "Pass123!",
                displayName = "Test User"
            };

            var first = await _client.PostAsJsonAsync("/api/auth/register", dto);
            first.StatusCode.Should().Be(HttpStatusCode.OK);

            var second = await _client.PostAsJsonAsync("/api/auth/register", dto);

            second.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var problem = await second.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Title.Should().Be("Conflict");
            problem.Detail.Should().Be("Email already in use");
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WithUserDto_WhenCredentialsAreValid()
        {
            var email = $"login-{Guid.NewGuid():N}@example.com";
            var password = "Pass123!";

            var registerDto = new
            {
                email,
                password,
                displayName = "Login User"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginDto = new
            {
                email,
                password
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var user = await loginResponse.Content.ReadFromJsonAsync<UserDto>();
            user.Should().NotBeNull();
            user!.Email.Should().Be(email);
            user.DisplayName.Should().Be("Login User");
            user.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var email = $"invalid-{Guid.NewGuid():N}@example.com";
            var correctPassword = "Pass123!";
            var wrongPassword = "Wrong123!";

            var registerDto = new
            {
                email,
                password = correctPassword,
                displayName = "Invalid Login User"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginDto = new
            {
                email,
                password = wrongPassword
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var problem = await loginResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
            problem!.Title.Should().Be("Unauthorized");
            problem.Detail.Should().Be("Invalid credentials");
        }
    }
}

using API.Middleware;
using Application.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.UnitTests;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldMapNotFoundException_ToProblemDetails()
    {
        var context = await InvokeMiddlewareAsync(new NotFoundException("Product with ID 99 not found"));

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        context.Response.ContentType.Should().Be("application/problem+json");

        var problem = await ReadProblemDetailsAsync(context);
        problem.Status.Should().Be(StatusCodes.Status404NotFound);
        problem.Title.Should().Be("Not Found");
        problem.Detail.Should().Be("Product with ID 99 not found");
        problem.Type.Should().Be("https://httpstatuses.com/404");
        problem.Instance.Should().Be("/api/products/99");
    }

    [Fact]
    public async Task InvokeAsync_ShouldMapConflictException_ToProblemDetails()
    {
        var context = await InvokeMiddlewareAsync(new ConflictException("Email already in use"));

        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var problem = await ReadProblemDetailsAsync(context);
        problem.Title.Should().Be("Conflict");
        problem.Detail.Should().Be("Email already in use");
    }

    [Fact]
    public async Task InvokeAsync_ShouldMapUnauthorizedAccessException_ToProblemDetails()
    {
        var context = await InvokeMiddlewareAsync(new UnauthorizedAccessException("Invalid credentials"));

        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

        var problem = await ReadProblemDetailsAsync(context);
        problem.Title.Should().Be("Unauthorized");
        problem.Detail.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task InvokeAsync_ShouldMapUnexpectedException_ToInternalServerErrorProblemDetails()
    {
        var context = await InvokeMiddlewareAsync(new Exception("Unexpected failure"));

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problem = await ReadProblemDetailsAsync(context);
        problem.Title.Should().Be("Internal Server Error");
        problem.Detail.Should().Be("Unexpected failure");
    }

    private static async Task<DefaultHttpContext> InvokeMiddlewareAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/products/99";
        context.Response.Body = new MemoryStream();

        var middleware = new ErrorHandlingMiddleware(_ => throw exception);
        await middleware.InvokeAsync(context);

        return context;
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problem.Should().NotBeNull();
        return problem!;
    }
}

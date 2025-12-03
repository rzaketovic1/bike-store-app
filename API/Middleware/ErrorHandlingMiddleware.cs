using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Decide the status dynamically based on the type of exception
            int statusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError // fallback
            };

            string title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status409Conflict => "Conflict",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "Error"
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = ex.Message,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(problem);
        }
    }
}

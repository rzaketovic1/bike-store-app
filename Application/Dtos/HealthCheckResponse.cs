namespace Application.Dtos;

public sealed class HealthCheckResponse
{
    public string Status { get; init; } = "healthy";
    public string Timestamp { get; init; } = string.Empty;
}

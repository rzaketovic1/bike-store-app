namespace Core.Common;

/// <summary>
/// Simple result container for paginated queries at the repository level
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
}

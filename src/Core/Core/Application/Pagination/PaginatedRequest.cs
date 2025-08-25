namespace Core.Application.Pagination;

/// <summary>
/// A basic pagination request with optional page index and page size.
/// </summary>
/// <param name="PageIndex">The zero-based index of the page to retrieve. Defaults to 0.</param>
/// <param name="PageSize">The number of items per page. Defaults to 10.</param>
/// <example>
/// <code>
/// var request = new PaginatedRequest(pageIndex: 2, pageSize: 20);
/// </code>
/// </example>
public record PaginatedRequest(int PageIndex = 0, int PageSize = 10);
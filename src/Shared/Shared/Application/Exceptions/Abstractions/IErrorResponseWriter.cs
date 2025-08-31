using System.Text.Json;
using _116.Shared.Application.Exceptions.Entities;
using Microsoft.AspNetCore.Http;

namespace _116.Shared.Application.Exceptions.Abstractions;

/// <summary>
/// Writes a standardized <see cref="ErrorResponseEntity"/> to the HTTP response.
/// </summary>
public interface IErrorResponseWriter
{
    /// <summary>
    /// Serializes and writes the <paramref name="errorResponse"/> to <paramref name="context"/>.
    /// </summary>
    Task WriteAsync(
        HttpContext context,
        ErrorResponseEntity errorResponse,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Default implementation for writing problem responses.
/// </summary>
public sealed class DefaultErrorResponseWriter : IErrorResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <inheritdoc />
    public async Task WriteAsync(
        HttpContext context,
        ErrorResponseEntity errorResponse,
        CancellationToken cancellationToken = default
    )
    {
        context.Response.StatusCode = errorResponse.Status;
        context.Response.ContentType = "application/json";

        string json = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await context.Response.WriteAsync(json, cancellationToken);
    }
}

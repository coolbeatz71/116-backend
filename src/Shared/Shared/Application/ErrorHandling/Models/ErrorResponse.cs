using System.Text.Json.Serialization;
using _116.Shared.Application.ErrorHandling.Enums;

namespace _116.Shared.Application.ErrorHandling.Models;

/// <summary>
/// Represents a standardized error response following RFC 7807 Problem Details specification.
/// Provides consistent error formatting across all application error scenarios.
/// </summary>
/// <remarks>
/// This model unifies error responses from exceptions, authentication failures, validation errors,
/// and any other error scenarios in the application. It ensures consistent client experience
/// and proper error correlation through trace identifiers.
/// </remarks>
public sealed class ErrorResponse
{
    /// <summary>
    /// A URI reference that identifies the problem type.
    /// </summary>
    /// <example>"https://tools.ietf.org/html/rfc7231#section-6.5.1"</example>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "about:blank";

    /// <summary>
    /// A short, human-readable summary of the problem type.
    /// </summary>
    /// <example>"Validation Error", "Unauthorized", "Not Found"</example>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// The HTTP status code for this occurrence of the problem.
    /// </summary>
    /// <example>400, 401, 404, 500</example>
    [JsonPropertyName("status")]
    public required int Status { get; init; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    /// <example>"The username field is required and cannot be empty."</example>
    [JsonPropertyName("detail")]
    public required string Detail { get; init; }

    /// <summary>
    /// A URI reference that identifies the specific occurrence of the problem.
    /// </summary>
    /// <example>"/api/users/create"</example>
    [JsonPropertyName("instance")]
    public required string Instance { get; init; }

    /// <summary>
    /// Unique identifier for request tracing and correlation.
    /// </summary>
    /// <example>"0HMVF3Q2S4V4D:00000001"</example>
    [JsonPropertyName("traceId")]
    public required string TraceId { get; init; }

    /// <summary>
    /// UTC timestamp when the error occurred.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Additional error-specific information.
    /// </summary>
    /// <remarks>
    /// This dictionary can contain validation errors, authentication details,
    /// or any other contextual information specific to the error type.
    /// </remarks>
    /// <example>
    /// For validation errors: { "errors": { "username": ["Required field"] } }
    /// For authentication: { "authenticationScheme": "Bearer" }
    /// </example>
    [JsonPropertyName("extensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Extensions { get; init; }

    /// <summary>
    /// Creates an error response with basic required information.
    /// </summary>
    /// <param name="title">The error title</param>
    /// <param name="status">HTTP status code</param>
    /// <param name="detail">Detailed error message</param>
    /// <param name="instance">Request path or identifier</param>
    /// <param name="traceId">Unique trace identifier</param>
    /// <returns>A new ErrorResponse instance</returns>
    public static ErrorResponse Create(
        string title,
        int status,
        string detail,
        string instance,
        string traceId
    )
    {
        return new ErrorResponse
        {
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response with additional extensions.
    /// </summary>
    /// <param name="title">The error title</param>
    /// <param name="status">HTTP status code</param>
    /// <param name="detail">Detailed error message</param>
    /// <param name="instance">Request path or identifier</param>
    /// <param name="traceId">Unique trace identifier</param>
    /// <param name="extensions">Additional error-specific data</param>
    /// <returns>A new ErrorResponse instance with extensions</returns>
    public static ErrorResponse CreateWithExtensions(
        string title,
        int status,
        string detail,
        string instance,
        string traceId,
        Dictionary<string, object> extensions)
    {
        return new ErrorResponse
        {
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow,
            Extensions = extensions
        };
    }
}

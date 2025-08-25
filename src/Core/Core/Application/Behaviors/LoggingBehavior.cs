using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace _116.Core.Application.Behaviors;

/// <summary>
/// A MediatR pipeline behavior that logs the lifecycle of a request and its response.
/// Also measures the request execution time and logs a warning if it exceeds 3 seconds.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the incoming request by logging its execution start and end,
    /// measuring execution time, and logging warnings for slow requests.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">The next delegate/middleware in the pipeline.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The response after executing the request.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        LogStart(request);

        var stopwatch = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        stopwatch.Stop();

        LogPerformanceWarning(request, stopwatch.Elapsed);
        LogEnd(request, response);

        return response;
    }

    /// <summary>
    /// Logs the start of the request processing.
    /// </summary>
    /// <param name="request">The request object.</param>
    private void LogStart(TRequest request)
    {
        logger.LogInformation(
            "[START] Handling {Request} - RequestData={RequestData}",
            typeof(TRequest).Name,
            request
        );
    }

    /// <summary>
    /// Logs a performance warning if the elapsed time exceeds the threshold.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="elapsed">The elapsed time for handling the request.</param>
    private void LogPerformanceWarning(TRequest request, TimeSpan elapsed)
    {
        // if the request is takes more than 3 seconds, then log the warnings
        if (elapsed.TotalSeconds > 3)
        {
            logger.LogWarning(
                "[PERFORMANCE] Request {Request} took {ElapsedSeconds:N2} seconds.",
                typeof(TRequest).Name,
                elapsed.TotalSeconds
            );
        }
    }

    /// <summary>
    /// Logs the end of request processing along with the response type.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="response">The response object.</param>
    private void LogEnd(TRequest request, TResponse response)
    {
        logger.LogInformation(
            "[END] Handled {Request} - Response={Response}",
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );
    }

}

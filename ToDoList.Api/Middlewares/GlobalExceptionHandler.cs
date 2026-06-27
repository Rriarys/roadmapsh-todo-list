using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BloggingPlatform.API.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle an exception and return a standardized JSON error response
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A value indicating whether the exception was successfully handled.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the unexpected failure details for internal tracking
        logger.LogError("An unhandled exception occurred: {Message}", exception.Message);

        // Define a consistent standard JSON error shape
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred on the server."
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // Return the failure response as JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to signal that this exception has been successfully handled
        return true;
    }
}
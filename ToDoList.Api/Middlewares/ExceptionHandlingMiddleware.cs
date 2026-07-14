using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log expected client errors without a full stack trace to avoid console clutter
            if (ex is UnauthorizedAccessException ||
                ex is KeyNotFoundException ||
                ex is InvalidOperationException ||
                (ex is ApplicationException appEx && appEx.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogWarning("Client error occurred: {Message}", ex.Message);
            }
            else
            {
                // Log unhandled critical exceptions with the full stack trace for debugging
                logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mapping various exception types to understandable HTTP statuses
        var (statusCode, title, detail) = exception switch
        {
            // 401 Unauthorized
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message),

            // 403 Forbidden
            ApplicationException appEx when appEx.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase) => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message),

            // 404 Not Found
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                exception.Message),

            // 409 Conflict
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message),

            // 500 Internal Server Error for unhandled exceptions
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred on the server.") // Do not expose exception details in production for security reasons
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Serialize the ProblemDetails object to JSON and write it to the response
        var result = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(result);
    }
}
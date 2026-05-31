namespace CourtManager.APIs.Middleware;

using CourtManager.Application.Exceptions;

/// <summary>
/// Global exception handling middleware for API.
/// Catches all unhandled exceptions and returns appropriate HTTP responses.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid input provided.";
                response.Details = exception.Message;
                break;

            case KeyNotFoundException:
            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "The requested resource was not found.";
                response.Details = exception.Message;
                break;

            case ValidationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = exception.Message;
                response.Details = exception.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Unauthorized access.";
                response.Details = exception.Message;
                break;

            case CourtManager.Application.Exceptions.ForbiddenException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "You do not have permission to perform this action.";
                response.Details = exception.Message;
                break;

            case CourtManager.Application.Exceptions.NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "The requested resource was not found.";
                response.Details = exception.Message;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An internal server error occurred.";
                response.Details = exception.Message;
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

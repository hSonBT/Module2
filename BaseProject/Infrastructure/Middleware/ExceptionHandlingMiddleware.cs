using System.Net;
using BaseProject.Core.Exceptions;

namespace BaseProject.Infrastructure.Middleware;

/// <summary>
/// Global exception handler middleware
/// Replace Laravel's exception handler
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new { success = false, message = "An error occurred", errors = (object?)null };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new { success = false, message = notFoundEx.Message, errors = (object?)null };
                break;
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { success = false, message = validationEx.Message, errors = (object?)null };
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { success = false, message = "Internal server error", errors = (object?)null };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
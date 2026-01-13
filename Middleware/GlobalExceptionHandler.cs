using System.Net;
using System.Text.Json;
using BlogPage.Domain.Exceptions;

namespace BlogPage.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFoundEx.Message;
                response.Type = "NotFound";
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;

            case BadRequestException badRequestEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = badRequestEx.Message;
                response.Type = "BadRequest";
                _logger.LogWarning(badRequestEx, "Bad request: {Message}", badRequestEx.Message);
                break;

            case UnauthorizedException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = unauthorizedEx.Message;
                response.Type = "Unauthorized";
                _logger.LogWarning(unauthorizedEx, "Unauthorized: {Message}", unauthorizedEx.Message);
                break;

            case ForbiddenException forbiddenEx:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Message = forbiddenEx.Message;
                response.Type = "Forbidden";
                _logger.LogWarning(forbiddenEx, "Forbidden: {Message}", forbiddenEx.Message);
                break;

            case KeyNotFoundException keyNotFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = keyNotFoundEx.Message;
                response.Type = "NotFound";
                _logger.LogWarning(keyNotFoundEx, "Key not found: {Message}", keyNotFoundEx.Message);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred";
                response.Type = "InternalServerError";
                
                if (_env.IsDevelopment())
                {
                    response.Details = exception.Message;
                    response.StackTrace = exception.StackTrace;
                }

                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}
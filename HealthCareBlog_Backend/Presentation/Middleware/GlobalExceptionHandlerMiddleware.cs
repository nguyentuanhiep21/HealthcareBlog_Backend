using System.Net;
using System.Text.Json;
using HealthCareBlog_Backend.Exceptions;

namespace HealthCareBlog_Backend.Presentation.Middleware;

/// <summary>
/// Global exception handler — bắt tất cả exception chưa được xử lý trong toàn bộ app.
/// Thay thế toàn bộ các try/catch lặp lại trong Controllers.
/// </summary>
public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                "Đã xảy ra lỗi. Vui lòng thử lại sau.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message,
            statusCode = (int)statusCode
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

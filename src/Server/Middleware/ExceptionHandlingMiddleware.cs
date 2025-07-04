using Core.Domain.Exceptions;
using Core.Messages.Responses;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Server.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            PlayerNotFoundException ex => new ErrorResponse("PLAYER_NOT_FOUND", ex.Message),
            InsufficientResourcesException ex => new ErrorResponse("INSUFFICIENT_RESOURCES", ex.Message),
            InvalidOperationException ex => new ErrorResponse("INVALID_OPERATION", ex.Message),
            ArgumentException ex => new ErrorResponse("INVALID_ARGUMENT", ex.Message),
            _ => new ErrorResponse("INTERNAL_ERROR", "An unexpected error occurred")
        };

        context.Response.StatusCode = exception switch
        {
            PlayerNotFoundException => StatusCodes.Status404NotFound,
            InsufficientResourcesException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(jsonResponse);
    }
}
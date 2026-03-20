using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Infrastructure.Common;

/// <summary>
/// Middleware der fanger alle ubehandlede exceptions og returnerer
/// en konsistent JSON-fejlrespons i ProblemDetails-format (RFC 7807).
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ubehandlet undtagelse for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/problem+json";

        // Differentier HTTP-statuskode baseret på exceptiontype
        context.Response.StatusCode = ex switch
        {
            ArgumentException       => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException    => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _                       => (int)HttpStatusCode.InternalServerError
        };

        // I production vises ikke intern fejldetalje til klienten (sikkerhed)
        var isProduction = !context.RequestServices
            .GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        var message = isProduction && context.Response.StatusCode == 500
            ? "Der opstod en intern serverfejl. Forsøg igen senere."
            : ex.Message;

        var problem = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = GetTitle(context.Response.StatusCode),
            Detail = message,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status404NotFound => "Not Found",
        _ => "Internal Server Error"
    };
}

/// <summary>
/// Extension method til nem registrering i Program.cs.
/// </summary>
public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}

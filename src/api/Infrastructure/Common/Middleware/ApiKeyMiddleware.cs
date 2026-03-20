using System.Net;
using System.Text.Json;
using FamilyHub.Api.Infrastructure.Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FamilyHub.Api.Infrastructure.Common.Middleware;

/// <summary>
/// Middleware der validerer API-nøgle i x-api-key header.
/// Tillader anonym adgang til /health og /api/v1/health endpoints.
/// </summary>
public sealed class ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static readonly string[] AnonymousPaths = ["/health", "/api/v1/health"];
    private readonly ILogger<ApiKeyMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, IOptionsMonitor<ApiKeyOptions> optionsMonitor)
    {
        var options = optionsMonitor.CurrentValue;

        // Hvis API key-validering er deaktiveret, fortsæt uden kontrol
        if (!options.Enabled)
        {
            await next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;

        // Tillad anonyme endpoints (health checks)
        if (IsAnonymousPath(path))
        {
            await next(context);
            return;
        }

        // Valider API-nøgle for alle andre endpoints
        if (!ValidateApiKey(context, options, _logger))
        {
            await WriteUnauthorizedResponseAsync(context);
            return;
        }

        await next(context);
    }

    private static bool ValidateApiKey(HttpContext context, ApiKeyOptions options, ILogger<ApiKeyMiddleware> logger)
    {
        // Tjek om header eksisterer
        if (!context.Request.Headers.TryGetValue(options.HeaderName, out var headerValue))
        {
            logger.LogWarning("API key-header '{HeaderName}' manglede for {Method} {Path}",
                options.HeaderName, context.Request.Method, context.Request.Path);
            return false;
        }

        var providedKey = headerValue.ToString();

        // Tjek om nøglerne matcher
        if (!string.Equals(providedKey, options.Key, StringComparison.Ordinal))
        {
            logger.LogWarning("Ugyldig API key for {Method} {Path}",
                context.Request.Method, context.Request.Path);
            return false;
        }

        return true;
    }

    private static bool IsAnonymousPath(string path)
    {
        return AnonymousPaths.Any(anonPath =>
            path.Equals(anonPath, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WriteUnauthorizedResponseAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var problem = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Title = "Unauthorized",
            Detail = "Ugyldig eller manglende API key.",
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}

/// <summary>
/// Extension method til nem registrering af ApiKeyMiddleware i Program.cs.
/// </summary>
public static class ApiKeyMiddlewareExtensions
{
    /// <summary>
    /// Registrerer og konfigurerer ApiKeyMiddleware med options fra "ApiKey"-sektion.
    /// </summary>
    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder app)
        => app.UseMiddleware<ApiKeyMiddleware>();

    /// <summary>
    /// Registrerer ApiKeyOptions i DI-containeren.
    /// </summary>
    public static IServiceCollection AddApiKeyValidation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApiKeyOptions>(configuration.GetSection("ApiKey"));
        return services;
    }
}

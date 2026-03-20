using FamilyHub.Api.Features.Calendar;
using FamilyHub.Api.Features.Catalog;
using FamilyHub.Api.Features.Orders;
using FamilyHub.Api.Features.Recipes;
using FamilyHub.Api.Features.Sync;
using FamilyHub.Api.Infrastructure.Common;
using FamilyHub.Api.Infrastructure.Common.Middleware;
using FamilyHub.Api.Infrastructure.Persistence;
using FamilyHub.Api.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// Swagger / OpenAPI med XML-kommentarer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "FamilyHub API",
        Version     = "v1",
        Description = "Backend API til FamilyHub touch-screen frontend"
    });

    // Inkludér XML-kommentarer i Swagger UI
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Gruppér endpoints i Swagger efter første domænesegment i ruten (calendar, catalog, recipes, sync, health)
    options.TagActionsBy(api =>
    {
        var path = api.RelativePath?.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (path is { Length: >= 3 } && string.Equals(path[0], "api", StringComparison.OrdinalIgnoreCase))
            return [ToTitleCase(path[2])];

        if (path is { Length: >= 2 } && string.Equals(path[0], "api", StringComparison.OrdinalIgnoreCase))
            return [ToTitleCase(path[1])];

        return ["General"];
    });

    // Definer API Key security scheme for Swagger
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Description = "API key skal sendes i x-api-key headeren"
    });

    // Applikér security requirement globalt (fra protected endpoints)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            []
        }
    });
});

// API Key validering
builder.Services.AddApiKeyValidation(builder.Configuration);

// SQLite via Entity Framework Core
var configuredConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var configuredDatabasePath = builder.Configuration["Database:Path"];

var effectiveConnectionString = !string.IsNullOrWhiteSpace(configuredConnectionString)
    ? configuredConnectionString
    : $"Data Source={configuredDatabasePath ?? "familyhub.db"}";

builder.Services.AddDbContext<FamilyHubDbContext>(options => options.UseSqlite(effectiveConnectionString));

// Health checks – inkluderer EF Core DB-check
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FamilyHubDbContext>();

// CORS – tillad localhost-baserede frontends under udvikling
builder.Services.AddCors(options =>
{
    options.AddPolicy("FamilyHubPolicy", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? ["http://localhost:3000", "http://localhost:5173"])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrér alle feature-services via extension methods
builder.Services.AddCalendarServices();
builder.Services.AddCatalogServices();
builder.Services.AddRecipeServices();
builder.Services.AddOrderServices();
builder.Services.AddSyncServices();

// ─── Pipeline ─────────────────────────────────────────────────────────────────

var app = builder.Build();

// Global exception handling – returnerer konsistent JSON-fejlrespons
app.UseGlobalExceptionHandler();

var enableSwaggerInProduction = builder.Configuration.GetValue("Swagger:EnableInProduction", true);
if (app.Environment.IsDevelopment() || enableSwaggerInProduction)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FamilyHub API v1");
        c.RoutePrefix = string.Empty; // Swagger som startside
        c.DisplayRequestDuration();
    });
}


// API Key validering – skal være før MapControllers
app.UseApiKeyValidation();

app.UseHttpsRedirection();
app.UseCors("FamilyHubPolicy");
app.UseAuthorization();
app.MapControllers();

// Standard health endpoint
app.MapHealthChecks("/health");

app.MapGet("/api/v1/health", async (HealthCheckService healthChecks, CancellationToken ct) =>
{
    var report = await healthChecks.CheckHealthAsync(ct);

    return Results.Ok(new
    {
        status = report.Status.ToString(),
        generatedAtUtc = DateTime.UtcNow,
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            durationMs = entry.Value.Duration.TotalMilliseconds
        })
    });
})
.WithTags("Health")
.WithSummary("Health status for API and dependencies")
.WithDescription("Returnerer samlet helbredstilstand for API og registrerede health checks.");

// Database bootstrap ved startup:
// - Migration: styres af Database:AutoMigrateOnStartup
// - Seed demo-data: styres af Database:SeedOnStartup (kun hvis databasen er tom)
var autoMigrateOnStartup = builder.Configuration.GetValue("Database:AutoMigrateOnStartup", true);
var seedOnStartup = builder.Configuration.GetValue("Database:SeedOnStartup", app.Environment.IsDevelopment());

if (autoMigrateOnStartup)
{
    await app.MigrateDatabaseAsync();
}

if (seedOnStartup)
{
    await app.SeedDevelopmentDataIfEmptyAsync();
}

app.Run();

static string ToTitleCase(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return "General";

    return char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
}

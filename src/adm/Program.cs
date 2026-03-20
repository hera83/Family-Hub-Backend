using FamilyHub.Adm.Configuration;
using FamilyHub.Adm.Infrastructure.Clients;
using FamilyHub.Adm.Services;
using FamilyHub.Adm.Services.ImportExport;
using FamilyHub.Adm.Services.ImportExport.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var familyHubApiSection = builder.Configuration.GetSection(FamilyHubApiOptions.SectionName);
builder.Services.Configure<FamilyHubApiOptions>(familyHubApiSection);

var configuredApiOptions = familyHubApiSection.Get<FamilyHubApiOptions>()
    ?? throw new InvalidOperationException($"Missing configuration section '{FamilyHubApiOptions.SectionName}'.");

if (!Uri.TryCreate(configuredApiOptions.BaseUrl, UriKind.Absolute, out _))
{
    throw new InvalidOperationException($"{FamilyHubApiOptions.SectionName}:BaseUrl must be an absolute URL.");
}

if (string.IsNullOrWhiteSpace(configuredApiOptions.ApiKeyHeaderName))
{
    throw new InvalidOperationException($"{FamilyHubApiOptions.SectionName}:ApiKeyHeaderName is required.");
}

if (string.IsNullOrWhiteSpace(configuredApiOptions.ApiKey))
{
    throw new InvalidOperationException($"{FamilyHubApiOptions.SectionName}:ApiKey is required.");
}

builder.Services.AddFamilyHubApiClients();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();
builder.Services.AddScoped<IExcelTemplateService, ExcelTemplateService>();

// Import handlers – register one per domain type; add new ones here to extend the import system.
builder.Services.AddScoped<IImportHandler, FamilyMemberImportHandler>();
builder.Services.AddScoped<IImportHandler, CalendarEventImportHandler>();
builder.Services.AddScoped<IImportHandler, ItemCategoryImportHandler>();
builder.Services.AddScoped<IImportHandler, ProductImportHandler>();
builder.Services.AddScoped<IImportHandler, RecipeCategoryImportHandler>();
builder.Services.AddScoped<IImportHandler, RecipeImportHandler>();
builder.Services.AddScoped<IImportHandler, RecipeIngredientImportHandler>();
builder.Services.AddScoped<IImportOrchestrator, ImportOrchestrator>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

using Microsoft.Extensions.DependencyInjection;

namespace FamilyHub.Api.Features.Recipes;

public static class RecipeServiceExtensions
{
    public static IServiceCollection AddRecipeServices(this IServiceCollection services)
    {
        services.AddScoped<IRecipeCategoryRequestValidator, RecipeCategoryRequestValidator>();
        services.AddScoped<IRecipeRequestValidator, RecipeRequestValidator>();
        services.AddScoped<IRecipeIngredientRequestValidator, RecipeIngredientRequestValidator>();
        services.AddScoped<IRecipeSyncRequestValidator, RecipeSyncRequestValidator>();
        services.AddScoped<IRecipeCategoryService, RecipeCategoryService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IRecipeSyncService, RecipeSyncService>();
        return services;
    }
}

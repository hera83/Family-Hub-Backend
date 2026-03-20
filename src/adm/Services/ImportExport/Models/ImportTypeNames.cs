namespace FamilyHub.Adm.Services.ImportExport.Models;

/// <summary>
/// Registry of supported import type names and display labels.
/// To add a new import type: add a constant here, add to the All list,
/// add a display name case, implement IImportHandler, and register in Program.cs.
/// </summary>
public static class ImportTypeNames
{
    public const string FamilyMembers = "FamilyMembers";
    public const string CalendarEvents = "CalendarEvents";
    public const string ItemCategories = "ItemCategories";
    public const string Products = "Products";
    public const string RecipeCategories = "RecipeCategories";
    public const string Recipes = "Recipes";
    public const string RecipeIngredients = "RecipeIngredients";

    public static readonly IReadOnlyList<string> All =
    [
        FamilyMembers,
        CalendarEvents,
        ItemCategories,
        Products,
        RecipeCategories,
        Recipes,
        RecipeIngredients,
    ];

    public static string GetDisplayName(string typeName) => typeName switch
    {
        FamilyMembers => "Familiemedlemmer",
        CalendarEvents => "Kalenderbegivenheder",
        ItemCategories => "Madvarekategorier",
        Products => "Madvarer",
        RecipeCategories => "Opskriftskategorier",
        Recipes => "Opskrifter",
        RecipeIngredients => "Opskriftsingredienser",
        _ => typeName
    };
}

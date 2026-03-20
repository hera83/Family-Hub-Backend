using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Services.ImportExport.Models;
using System.Net;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class RecipeIngredientImportHandler(
    IRecipesApiClient recipesApiClient,
    ICatalogApiClient catalogApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.RecipeIngredients;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    // Adjust ingredient import rules here.
    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public Guid? RecipeId { get; init; }
        public string? RecipeTitle { get; init; }
        public Guid? ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? Name { get; init; }
        public decimal? Quantity { get; init; }
        public string? Unit { get; init; }
        public bool IsStaple { get; init; }
        public int SortOrder { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "RecipeIngredients");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum      = xlRow.RowNumber();
            var id          = GetCellString(xlRow, map, "Id");
            var recipeId    = GetCellString(xlRow, map, "RecipeId");
            var recipeTitle = GetCellString(xlRow, map, "RecipeTitle");
            var productId   = GetCellString(xlRow, map, "ProductId");
            var productName = GetCellString(xlRow, map, "ProductName");
            var name        = GetCellString(xlRow, map, "Name");
            var quantity    = GetCellDecimal(xlRow, map, "Quantity");
            var unit        = GetCellString(xlRow, map, "Unit");
            var isStaple    = GetCellBool(xlRow, map, "IsStaple");
            var sortOrder   = GetCellInt(xlRow, map, "SortOrder");

            var errors = new List<string>();
            // Either RecipeId or RecipeTitle must be present to resolve the parent recipe.
            if (string.IsNullOrWhiteSpace(recipeId) && string.IsNullOrWhiteSpace(recipeTitle))
                errors.Add("RecipeId eller RecipeTitle er påkrævet.");
            // Ingredient must have either a product reference or a free-text name.
            if (string.IsNullOrWhiteSpace(productId) && string.IsNullOrWhiteSpace(productName)
                && string.IsNullOrWhiteSpace(name))
                errors.Add("ProductId, ProductName eller Name er påkrævet.");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns =
                [
                    D("Id", id),
                    D("RecipeTitle", recipeTitle),
                    D("ProductName", productName),
                    D("Name", name),
                    D("Quantity", quantity?.ToString("0.##")),
                    D("Unit", unit),
                ],
                Data = errors.Count == 0
                    ? new RowData
                    {
                        Id = ParseGuid(id),
                        RecipeId = ParseGuid(recipeId),
                        RecipeTitle = recipeTitle,
                        ProductId = ParseGuid(productId),
                        ProductName = productName,
                        Name = name,
                        Quantity = quantity,
                        Unit = unit,
                        IsStaple = isStaple,
                        SortOrder = sortOrder,
                    }
                    : null
            });
        }

        return new ImportPreview
        {
            TypeName = ImportType,
            DisplayName = DisplayName,
            TotalRows = rows.Count,
            ValidRows = rows.Count(r => r.IsValid),
            InvalidRows = rows.Count(r => !r.IsValid),
            Rows = rows
        };
    }

    public async Task<ImportResult> ExecuteAsync(ImportPreview preview, CancellationToken cancellationToken)
    {
        // Pre-fetch recipes for title → ID lookup (needed when RecipeTitle is used instead of RecipeId).
        var recipesByTitle = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var existingRecipeIds = new HashSet<Guid>();
        // Pre-fetch products for name → ID lookup.
        var productsByName = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var recipes = await recipesApiClient.GetRecipesAsync(
                new RecipeListQueryRequest { Page = 1, PageSize = 5000 }, cancellationToken);
            foreach (var r in recipes.Items)
            {
                existingRecipeIds.Add(r.Id);
                recipesByTitle.TryAdd(r.Title, r.Id);
            }

            var products = await catalogApiClient.GetProductsAsync(
                new ProductListQueryRequest { Page = 1, PageSize = 5000 }, cancellationToken);
            foreach (var p in products.Items)
                productsByName.TryAdd(p.Name, p.Id);
        }
        catch (ApiClientException) { }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;

            // Resolve recipe: use RecipeId if valid and known, else look up by title.
            Guid? resolvedRecipeId = data.RecipeId.HasValue && existingRecipeIds.Contains(data.RecipeId.Value)
                ? data.RecipeId.Value
                : data.RecipeTitle is not null && recipesByTitle.TryGetValue(data.RecipeTitle, out var foundRecipeId)
                    ? foundRecipeId
                    : null;

            if (resolvedRecipeId is null)
            {
                failed++;
                errors.Add(new ImportRowError
                {
                    RowNumber = row.RowNumber,
                    Message = $"Kunne ikke finde opskriften '{data.RecipeTitle ?? data.RecipeId?.ToString()}'. Import af ingrediens sprunget over."
                });
                continue;
            }

            // Resolve product: use ProductId if valid and known, else look up by name.
            Guid? resolvedProductId = data.ProductId is not null
                ? data.ProductId
                : data.ProductName is not null && productsByName.TryGetValue(data.ProductName, out var foundProductId)
                    ? foundProductId
                    : null;

            var createReq = new CreateRecipeIngredientRequest
            {
                ProductId = resolvedProductId,
                Name = data.Name,
                Quantity = data.Quantity,
                Unit = data.Unit,
                IsStaple = data.IsStaple,
                SortOrder = data.SortOrder,
            };

            try
            {
                if (data.Id.HasValue)
                {
                    // Try UPDATE first; if the ingredient no longer exists, fall back to CREATE.
                    try
                    {
                        await recipesApiClient.UpdateIngredientAsync(resolvedRecipeId.Value, data.Id.Value,
                            new UpdateRecipeIngredientRequest
                            {
                                ProductId = createReq.ProductId,
                                Name = createReq.Name,
                                Quantity = createReq.Quantity,
                                Unit = createReq.Unit,
                                IsStaple = createReq.IsStaple,
                                SortOrder = createReq.SortOrder,
                            }, cancellationToken);
                        updated++;
                    }
                    catch (ApiClientException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        await recipesApiClient.AddIngredientAsync(resolvedRecipeId.Value, createReq, cancellationToken);
                        created++;
                    }
                }
                else
                {
                    await recipesApiClient.AddIngredientAsync(resolvedRecipeId.Value, createReq, cancellationToken);
                    created++;
                }
            }
            catch (ApiClientException ex)
            {
                failed++;
                errors.Add(new ImportRowError { RowNumber = row.RowNumber, Message = ex.UserMessage });
            }
        }

        return new ImportResult
        {
            TypeName = ImportType,
            DisplayName = DisplayName,
            TotalRead = preview.TotalRows,
            Created = created,
            Updated = updated,
            Failed = failed,
            Errors = errors
        };
    }
}

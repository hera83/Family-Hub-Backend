using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class RecipeImportHandler(IRecipesApiClient recipesApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.Recipes;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    // Adjust recipe import rules here.
    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public required string Title { get; init; }
        public Guid? RecipeCategoryId { get; init; }
        public string? RecipeCategoryName { get; init; }
        public string? ImageUrl { get; init; }
        public string? Description { get; init; }
        public int? PrepTimeMinutes { get; init; }
        public int? WaitTimeMinutes { get; init; }
        public string? Instructions { get; init; }
        public bool IsManual { get; init; }
        public bool IsFavorite { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "Recipes");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum         = xlRow.RowNumber();
            var id             = GetCellString(xlRow, map, "Id");
            var title          = GetCellString(xlRow, map, "Title");
            var recipeCatId    = GetCellString(xlRow, map, "RecipeCategoryId");
            var recipeCatName  = GetCellString(xlRow, map, "RecipeCategoryName");
            var imageUrl       = GetCellString(xlRow, map, "ImageUrl");
            var description    = GetCellString(xlRow, map, "Description");
            var prepStr        = GetCellString(xlRow, map, "PrepTimeMinutes");
            var waitStr        = GetCellString(xlRow, map, "WaitTimeMinutes");
            var instructions   = GetCellString(xlRow, map, "Instructions");
            var isManual       = GetCellBool(xlRow, map, "IsManual");
            var isFavorite     = GetCellBool(xlRow, map, "IsFavorite");

            var parsedPrep = prepStr is not null && int.TryParse(prepStr, out var pp) ? (int?)pp : null;
            var parsedWait = waitStr is not null && int.TryParse(waitStr, out var pw) ? (int?)pw : null;

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(title)) errors.Add("Title er påkrævet.");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns =
                [
                    D("Id", id),
                    D("Title", title),
                    D("RecipeCategoryName", recipeCatName),
                    D("PrepTimeMinutes", prepStr),
                    D("IsFavorite", isFavorite.ToString()),
                ],
                Data = errors.Count == 0
                    ? new RowData
                    {
                        Id = ParseGuid(id),
                        Title = title!,
                        RecipeCategoryId = ParseGuid(recipeCatId),
                        RecipeCategoryName = recipeCatName,
                        ImageUrl = imageUrl,
                        Description = description,
                        PrepTimeMinutes = parsedPrep,
                        WaitTimeMinutes = parsedWait,
                        Instructions = instructions,
                        IsManual = isManual,
                        IsFavorite = isFavorite,
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
        // Pre-fetch recipe categories for name → ID resolution.
        var categoriesByName = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var existingRecipeIds = new HashSet<Guid>();
        try
        {
            var cats = await recipesApiClient.GetCategoriesAsync(cancellationToken);
            foreach (var c in cats) categoriesByName[c.Name] = c.Id;

            var recipes = await recipesApiClient.GetRecipesAsync(
                new RecipeListQueryRequest { Page = 1, PageSize = 5000 }, cancellationToken);
            foreach (var r in recipes.Items) existingRecipeIds.Add(r.Id);
        }
        catch (ApiClientException) { }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;

            // Resolve RecipeCategoryId: use provided ID first, fall back to name lookup.
            var resolvedCategoryId = data.RecipeCategoryId;
            if (resolvedCategoryId is null && data.RecipeCategoryName is not null
                && categoriesByName.TryGetValue(data.RecipeCategoryName, out var foundCatId))
            {
                resolvedCategoryId = foundCatId;
            }

            var createReq = new CreateRecipeRequest
            {
                Title = data.Title,
                RecipeCategoryId = resolvedCategoryId,
                ImageUrl = data.ImageUrl,
                Description = data.Description,
                PrepTimeMinutes = data.PrepTimeMinutes,
                WaitTimeMinutes = data.WaitTimeMinutes,
                Instructions = data.Instructions,
                IsManual = data.IsManual,
                IsFavorite = data.IsFavorite,
            };

            try
            {
                if (data.Id.HasValue && existingRecipeIds.Contains(data.Id.Value))
                {
                    await recipesApiClient.UpdateRecipeAsync(data.Id.Value,
                        new UpdateRecipeRequest
                        {
                            Title = createReq.Title,
                            RecipeCategoryId = createReq.RecipeCategoryId,
                            ImageUrl = createReq.ImageUrl,
                            Description = createReq.Description,
                            PrepTimeMinutes = createReq.PrepTimeMinutes,
                            WaitTimeMinutes = createReq.WaitTimeMinutes,
                            Instructions = createReq.Instructions,
                            IsManual = createReq.IsManual,
                            IsFavorite = createReq.IsFavorite,
                        }, cancellationToken);
                    updated++;
                }
                else
                {
                    await recipesApiClient.CreateRecipeAsync(createReq, cancellationToken);
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

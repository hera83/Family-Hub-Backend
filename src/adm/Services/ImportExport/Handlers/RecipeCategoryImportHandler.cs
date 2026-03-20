using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class RecipeCategoryImportHandler(IRecipesApiClient recipesApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.RecipeCategories;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public required string Name { get; init; }
        public int SortOrder { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "RecipeCategories");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum    = xlRow.RowNumber();
            var id        = GetCellString(xlRow, map, "Id");
            var name      = GetCellString(xlRow, map, "Name");
            var sortOrder = GetCellInt(xlRow, map, "SortOrder");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name)) errors.Add("Name er påkrævet.");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns = [D("Id", id), D("Name", name), D("SortOrder", sortOrder.ToString())],
                Data = errors.Count == 0
                    ? new RowData { Id = ParseGuid(id), Name = name!, SortOrder = sortOrder }
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
        var existingIds = new HashSet<Guid>();
        try
        {
            var all = await recipesApiClient.GetCategoriesAsync(cancellationToken);
            foreach (var c in all) existingIds.Add(c.Id);
        }
        catch (ApiClientException) { }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;
            try
            {
                if (data.Id.HasValue && existingIds.Contains(data.Id.Value))
                {
                    await recipesApiClient.UpdateCategoryAsync(data.Id.Value,
                        new UpdateRecipeCategoryRequest { Name = data.Name, SortOrder = data.SortOrder },
                        cancellationToken);
                    updated++;
                }
                else
                {
                    await recipesApiClient.CreateCategoryAsync(
                        new CreateRecipeCategoryRequest { Name = data.Name, SortOrder = data.SortOrder },
                        cancellationToken);
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

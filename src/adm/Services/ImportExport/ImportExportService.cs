using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Sync;
using FamilyHub.Adm.Models.Api.Sync;

namespace FamilyHub.Adm.Services.ImportExport;

public sealed class ImportExportService(ISyncApiClient syncApiClient) : IImportExportService
{
    private readonly ISyncApiClient _syncApiClient = syncApiClient;

    public async Task<ExcelExportFile> ExportFamilyMembersAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddFamilyMembersSheet(workbook, sync));
        return CreateExportFile("family-members", content);
    }

    public async Task<ExcelExportFile> ExportCalendarEventsAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddCalendarEventsSheet(workbook, sync));
        return CreateExportFile("calendar-events", content);
    }

    public async Task<ExcelExportFile> ExportItemCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddItemCategoriesSheet(workbook, sync));
        return CreateExportFile("item-categories", content);
    }

    public async Task<ExcelExportFile> ExportProductsAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddProductsSheet(workbook, sync));
        return CreateExportFile("products", content);
    }

    public async Task<ExcelExportFile> ExportRecipeCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddRecipeCategoriesSheet(workbook, sync));
        return CreateExportFile("recipe-categories", content);
    }

    public async Task<ExcelExportFile> ExportRecipesAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddRecipesSheet(workbook, sync));
        return CreateExportFile("recipes", content);
    }

    public async Task<ExcelExportFile> ExportRecipeIngredientsAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddRecipeIngredientsSheet(workbook, sync));
        return CreateExportFile("recipe-ingredients", content);
    }

    public async Task<ExcelExportFile> ExportOrdersAsync(CancellationToken cancellationToken = default)
    {
        // Orders er historiske snapshots i v1 og understøttes derfor kun til eksport (ikke import).
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddOrdersSheet(workbook, sync));
        return CreateExportFile("orders", content);
    }

    public async Task<ExcelExportFile> ExportOrderLinesAsync(CancellationToken cancellationToken = default)
    {
        // OrderLines eksporteres som snapshots for dokumentation/revision i admin-portalen.
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);
        var content = BuildWorkbook(workbook => AddOrderLinesSheet(workbook, sync));
        return CreateExportFile("order-lines", content);
    }

    public async Task<ExcelExportFile> ExportWorkbookAsync(CancellationToken cancellationToken = default)
    {
        var sync = await _syncApiClient.GetFullSyncAsync(cancellationToken);

        var content = BuildWorkbook(workbook =>
        {
            AddFamilyMembersSheet(workbook, sync);
            AddCalendarEventsSheet(workbook, sync);
            AddItemCategoriesSheet(workbook, sync);
            AddProductsSheet(workbook, sync);
            AddRecipeCategoriesSheet(workbook, sync);
            AddRecipesSheet(workbook, sync);
            AddRecipeIngredientsSheet(workbook, sync);
            AddOrdersSheet(workbook, sync);
            AddOrderLinesSheet(workbook, sync);
        });

        return CreateExportFile("full-export", content);
    }

    private static byte[] BuildWorkbook(Action<XLWorkbook> build)
    {
        using var workbook = new XLWorkbook();
        build(workbook);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static ExcelExportFile CreateExportFile(string datasetName, byte[] content)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var fileName = $"familyhub-{datasetName}-{timestamp}.xlsx";
        return new ExcelExportFile(fileName, content);
    }

    private static void AddFamilyMembersSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("FamilyMembers");

        WriteHeader(sheet,
            "Id",
            "Name",
            "Color",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var member in sync.FamilyMembers.OrderBy(x => x.Name))
        {
            sheet.Cell(row, 1).SetValue(member.Id.ToString());
            sheet.Cell(row, 2).SetValue(member.Name);
            sheet.Cell(row, 3).SetValue(member.Color);
            sheet.Cell(row, 4).SetValue(ToUtcText(member.CreatedAtUtc));
            sheet.Cell(row, 5).SetValue(ToNullableUtcText(member.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddCalendarEventsSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("CalendarEvents");

        WriteHeader(sheet,
            "Id",
            "Title",
            "Description",
            "EventDate",
            "StartTime",
            "EndTime",
            "FamilyMemberId",
            "FamilyMemberName",
            "RecurrenceType",
            "RecurrenceDays",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.CalendarEvents.OrderBy(x => x.EventDate).ThenBy(x => x.StartTime).ThenBy(x => x.Title))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Title);
            sheet.Cell(row, 3).SetValue(item.Description ?? string.Empty);
            sheet.Cell(row, 4).SetValue(item.EventDate.ToString("yyyy-MM-dd"));
            sheet.Cell(row, 5).SetValue(item.StartTime?.ToString("HH:mm:ss") ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.EndTime?.ToString("HH:mm:ss") ?? string.Empty);
            sheet.Cell(row, 7).SetValue(item.FamilyMemberId?.ToString() ?? string.Empty);
            sheet.Cell(row, 8).SetValue(item.FamilyMemberName ?? string.Empty);
            sheet.Cell(row, 9).SetValue(item.RecurrenceType ?? string.Empty);
            sheet.Cell(row, 10).SetValue(item.RecurrenceDays is { Length: > 0 } ? string.Join(",", item.RecurrenceDays) : string.Empty);
            sheet.Cell(row, 11).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 12).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddItemCategoriesSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("ItemCategories");

        WriteHeader(sheet,
            "Id",
            "Name",
            "SortOrder",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.ItemCategories.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Name);
            sheet.Cell(row, 3).SetValue(item.SortOrder);
            sheet.Cell(row, 4).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 5).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddProductsSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("Products");

        WriteHeader(sheet,
            "Id",
            "Name",
            "ItemCategoryId",
            "ItemCategoryName",
            "Description",
            "ImageUrl",
            "Unit",
            "SizeLabel",
            "Price",
            "IsManual",
            "IsFavorite",
            "IsStaple",
            "CaloriesPer100g",
            "FatPer100g",
            "CarbsPer100g",
            "ProteinPer100g",
            "FiberPer100g",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.Products.OrderBy(x => x.Name))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Name);
            sheet.Cell(row, 3).SetValue(item.ItemCategoryId?.ToString() ?? string.Empty);
            sheet.Cell(row, 4).SetValue(item.ItemCategoryName ?? string.Empty);
            sheet.Cell(row, 5).SetValue(item.Description ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.ImageUrl ?? string.Empty);
            sheet.Cell(row, 7).SetValue(item.Unit ?? string.Empty);
            sheet.Cell(row, 8).SetValue(item.SizeLabel ?? string.Empty);
            sheet.Cell(row, 9).SetValue(item.Price);
            sheet.Cell(row, 10).SetValue(item.IsManual);
            sheet.Cell(row, 11).SetValue(item.IsFavorite);
            sheet.Cell(row, 12).SetValue(item.IsStaple);
            sheet.Cell(row, 13).SetValue(item.CaloriesPer100g);
            sheet.Cell(row, 14).SetValue(item.FatPer100g);
            sheet.Cell(row, 15).SetValue(item.CarbsPer100g);
            sheet.Cell(row, 16).SetValue(item.ProteinPer100g);
            sheet.Cell(row, 17).SetValue(item.FiberPer100g);
            sheet.Cell(row, 18).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 19).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddRecipeCategoriesSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("RecipeCategories");

        WriteHeader(sheet,
            "Id",
            "Name",
            "SortOrder",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.RecipeCategories.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Name);
            sheet.Cell(row, 3).SetValue(item.SortOrder);
            sheet.Cell(row, 4).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 5).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddRecipesSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("Recipes");

        WriteHeader(sheet,
            "Id",
            "Title",
            "RecipeCategoryId",
            "RecipeCategoryName",
            "ImageUrl",
            "Description",
            "PrepTimeMinutes",
            "WaitTimeMinutes",
            "Instructions",
            "IsManual",
            "IsFavorite",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.Recipes.OrderBy(x => x.Title))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Title);
            sheet.Cell(row, 3).SetValue(item.RecipeCategoryId?.ToString() ?? string.Empty);
            sheet.Cell(row, 4).SetValue(item.RecipeCategoryName ?? string.Empty);
            sheet.Cell(row, 5).SetValue(item.ImageUrl ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.Description ?? string.Empty);
            sheet.Cell(row, 7).SetValue(item.PrepTimeMinutes);
            sheet.Cell(row, 8).SetValue(item.WaitTimeMinutes);
            sheet.Cell(row, 9).SetValue(item.Instructions ?? string.Empty);
            sheet.Cell(row, 10).SetValue(item.IsManual);
            sheet.Cell(row, 11).SetValue(item.IsFavorite);
            sheet.Cell(row, 12).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 13).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddRecipeIngredientsSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("RecipeIngredients");
        var recipeTitleById = sync.Recipes.ToDictionary(x => x.Id, x => x.Title);

        WriteHeader(sheet,
            "Id",
            "RecipeId",
            "RecipeTitle",
            "ProductId",
            "ProductName",
            "Name",
            "Quantity",
            "Unit",
            "IsStaple",
            "SortOrder",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.RecipeIngredients.OrderBy(x => x.RecipeId).ThenBy(x => x.SortOrder).ThenBy(x => x.Name))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.RecipeId.ToString());
            sheet.Cell(row, 3).SetValue(recipeTitleById.TryGetValue(item.RecipeId, out var title) ? title : string.Empty);
            sheet.Cell(row, 4).SetValue(item.ProductId?.ToString() ?? string.Empty);
            sheet.Cell(row, 5).SetValue(item.ProductName ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.Name ?? string.Empty);
            sheet.Cell(row, 7).SetValue(item.Quantity);
            sheet.Cell(row, 8).SetValue(item.Unit ?? string.Empty);
            sheet.Cell(row, 9).SetValue(item.IsStaple);
            sheet.Cell(row, 10).SetValue(item.SortOrder);
            sheet.Cell(row, 11).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 12).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddOrdersSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("Orders");

        WriteHeader(sheet,
            "Id",
            "Status",
            "TotalItems",
            "TotalPrice",
            "Notes",
            "HasPdf",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.Orders.OrderByDescending(x => x.CreatedAtUtc))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.Status);
            sheet.Cell(row, 3).SetValue(item.TotalItems);
            sheet.Cell(row, 4).SetValue(item.TotalPrice);
            sheet.Cell(row, 5).SetValue(item.Notes ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.HasPdf);
            sheet.Cell(row, 7).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 8).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void AddOrderLinesSheet(XLWorkbook workbook, FullSyncDto sync)
    {
        var sheet = workbook.Worksheets.Add("OrderLines");

        WriteHeader(sheet,
            "Id",
            "OrderId",
            "ProductName",
            "Quantity",
            "Unit",
            "CategoryName",
            "Price",
            "SizeLabel",
            "CreatedAtUtc",
            "UpdatedAtUtc");

        var row = 2;
        foreach (var item in sync.OrderLines.OrderByDescending(x => x.CreatedAtUtc).ThenBy(x => x.OrderId))
        {
            sheet.Cell(row, 1).SetValue(item.Id.ToString());
            sheet.Cell(row, 2).SetValue(item.OrderId.ToString());
            sheet.Cell(row, 3).SetValue(item.ProductName);
            sheet.Cell(row, 4).SetValue(item.Quantity);
            sheet.Cell(row, 5).SetValue(item.Unit ?? string.Empty);
            sheet.Cell(row, 6).SetValue(item.CategoryName ?? string.Empty);
            sheet.Cell(row, 7).SetValue(item.Price);
            sheet.Cell(row, 8).SetValue(item.SizeLabel ?? string.Empty);
            sheet.Cell(row, 9).SetValue(ToUtcText(item.CreatedAtUtc));
            sheet.Cell(row, 10).SetValue(ToNullableUtcText(item.UpdatedAtUtc));
            row++;
        }

        FinalizeSheet(sheet);
    }

    private static void WriteHeader(IXLWorksheet sheet, params string[] headers)
    {
        for (var index = 0; index < headers.Length; index++)
        {
            sheet.Cell(1, index + 1).SetValue(headers[index]);
        }

        var headerRange = sheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
    }

    private static void FinalizeSheet(IXLWorksheet sheet)
    {
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();
    }

    private static string ToUtcText(DateTime value)
        => value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

    private static string ToNullableUtcText(DateTime? value)
        => value.HasValue ? ToUtcText(value.Value) : string.Empty;
}

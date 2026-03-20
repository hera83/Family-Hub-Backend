using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class ProductImportHandler(ICatalogApiClient catalogApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.Products;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    // Adjust product import rules here (e.g. required fields, category resolution).
    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public required string Name { get; init; }
        public Guid? ItemCategoryId { get; init; }
        public string? ItemCategoryName { get; init; }
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public string? Unit { get; init; }
        public string? SizeLabel { get; init; }
        public decimal? Price { get; init; }
        public bool IsManual { get; init; }
        public bool IsFavorite { get; init; }
        public bool IsStaple { get; init; }
        public decimal? CaloriesPer100g { get; init; }
        public decimal? FatPer100g { get; init; }
        public decimal? CarbsPer100g { get; init; }
        public decimal? ProteinPer100g { get; init; }
        public decimal? FiberPer100g { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "Products");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum          = xlRow.RowNumber();
            var id              = GetCellString(xlRow, map, "Id");
            var name            = GetCellString(xlRow, map, "Name");
            var itemCategoryId  = GetCellString(xlRow, map, "ItemCategoryId");
            var itemCategoryName = GetCellString(xlRow, map, "ItemCategoryName");
            var description     = GetCellString(xlRow, map, "Description");
            var imageUrl        = GetCellString(xlRow, map, "ImageUrl");
            var unit            = GetCellString(xlRow, map, "Unit");
            var sizeLabel       = GetCellString(xlRow, map, "SizeLabel");
            var price           = GetCellDecimal(xlRow, map, "Price");
            var isManual        = GetCellBool(xlRow, map, "IsManual");
            var isFavorite      = GetCellBool(xlRow, map, "IsFavorite");
            var isStaple        = GetCellBool(xlRow, map, "IsStaple");
            var calories        = GetCellDecimal(xlRow, map, "CaloriesPer100g");
            var fat             = GetCellDecimal(xlRow, map, "FatPer100g");
            var carbs           = GetCellDecimal(xlRow, map, "CarbsPer100g");
            var protein         = GetCellDecimal(xlRow, map, "ProteinPer100g");
            var fiber           = GetCellDecimal(xlRow, map, "FiberPer100g");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name)) errors.Add("Name er påkrævet.");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns =
                [
                    D("Id", id),
                    D("Name", name),
                    D("ItemCategoryName", itemCategoryName),
                    D("Unit", unit),
                    D("Price", price?.ToString("0.##")),
                    D("IsManual", isManual.ToString()),
                    D("IsStaple", isStaple.ToString()),
                ],
                Data = errors.Count == 0
                    ? new RowData
                    {
                        Id = ParseGuid(id),
                        Name = name!,
                        ItemCategoryId = ParseGuid(itemCategoryId),
                        ItemCategoryName = itemCategoryName,
                        Description = description,
                        ImageUrl = imageUrl,
                        Unit = unit,
                        SizeLabel = sizeLabel,
                        Price = price,
                        IsManual = isManual,
                        IsFavorite = isFavorite,
                        IsStaple = isStaple,
                        CaloriesPer100g = calories,
                        FatPer100g = fat,
                        CarbsPer100g = carbs,
                        ProteinPer100g = protein,
                        FiberPer100g = fiber,
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
        // Pre-fetch item categories for name → ID resolution.
        var categoriesByName = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var existingProductIds = new HashSet<Guid>();
        try
        {
            var cats = await catalogApiClient.GetCategoriesAsync(cancellationToken);
            foreach (var c in cats) categoriesByName[c.Name] = c.Id;

            // Fetch all existing products (large page size for typical family hub data volumes).
            var products = await catalogApiClient.GetProductsAsync(
                new ProductListQueryRequest { Page = 1, PageSize = 5000 }, cancellationToken);
            foreach (var p in products.Items) existingProductIds.Add(p.Id);
        }
        catch (ApiClientException) { }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;

            // Resolve ItemCategoryId: use provided ID first, fall back to name lookup.
            var resolvedCategoryId = data.ItemCategoryId;
            if (resolvedCategoryId is null && data.ItemCategoryName is not null
                && categoriesByName.TryGetValue(data.ItemCategoryName, out var foundCatId))
            {
                resolvedCategoryId = foundCatId;
            }

            var createReq = new CreateProductRequest
            {
                Name = data.Name,
                ItemCategoryId = resolvedCategoryId,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                Unit = data.Unit,
                SizeLabel = data.SizeLabel,
                Price = data.Price,
                IsManual = data.IsManual,
                IsFavorite = data.IsFavorite,
                IsStaple = data.IsStaple,
                CaloriesPer100g = data.CaloriesPer100g,
                FatPer100g = data.FatPer100g,
                CarbsPer100g = data.CarbsPer100g,
                ProteinPer100g = data.ProteinPer100g,
                FiberPer100g = data.FiberPer100g,
            };

            try
            {
                if (data.Id.HasValue && existingProductIds.Contains(data.Id.Value))
                {
                    await catalogApiClient.UpdateProductAsync(data.Id.Value,
                        new UpdateProductRequest
                        {
                            Name = createReq.Name,
                            ItemCategoryId = createReq.ItemCategoryId,
                            Description = createReq.Description,
                            ImageUrl = createReq.ImageUrl,
                            Unit = createReq.Unit,
                            SizeLabel = createReq.SizeLabel,
                            Price = createReq.Price,
                            IsManual = createReq.IsManual,
                            IsFavorite = createReq.IsFavorite,
                            IsStaple = createReq.IsStaple,
                            CaloriesPer100g = createReq.CaloriesPer100g,
                            FatPer100g = createReq.FatPer100g,
                            CarbsPer100g = createReq.CarbsPer100g,
                            ProteinPer100g = createReq.ProteinPer100g,
                            FiberPer100g = createReq.FiberPer100g,
                        }, cancellationToken);
                    updated++;
                }
                else
                {
                    await catalogApiClient.CreateProductAsync(createReq, cancellationToken);
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

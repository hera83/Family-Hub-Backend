using FamilyHub.Adm.Models.Api.Orders;

namespace FamilyHub.Adm.Models.Orders;

internal static class OrderViewModelMappings
{
    internal static OrderListItemViewModel ToViewModel(this OrderListItemDto dto) => new()
    {
        Id = dto.Id,
        Status = dto.Status,
        TotalItems = dto.TotalItems,
        TotalPrice = dto.TotalPrice,
        HasPdf = dto.HasPdf,
        CreatedAtUtc = dto.CreatedAtUtc,
        UpdatedAtUtc = dto.UpdatedAtUtc
    };

    internal static OrderLineViewModel ToViewModel(this OrderLineDto dto) => new()
    {
        Id = dto.Id,
        OrderId = dto.OrderId,
        ProductName = dto.ProductName,
        CategoryName = dto.CategoryName,
        Quantity = dto.Quantity,
        Unit = dto.Unit,
        Price = dto.Price,
        SizeLabel = dto.SizeLabel,
        CreatedAtUtc = dto.CreatedAtUtc,
        UpdatedAtUtc = dto.UpdatedAtUtc
    };

    internal static OrderDetailsViewModel ToViewModel(this OrderDetailsDto dto) => new()
    {
        Id = dto.Id,
        Status = dto.Status,
        TotalItems = dto.TotalItems,
        TotalPrice = dto.TotalPrice,
        Notes = dto.Notes,
        HasPdf = dto.HasPdf,
        CreatedAtUtc = dto.CreatedAtUtc,
        UpdatedAtUtc = dto.UpdatedAtUtc,
        Lines = dto.Lines.Select(x => x.ToViewModel()).ToArray()
    };

    internal static OrderPdfViewModel ToViewModel(this OrderPdfDto dto) => new()
    {
        OrderId = dto.OrderId,
        HasPdf = dto.HasPdf,
        PdfData = dto.PdfData,
        FileName = dto.FileName,
        ContentType = dto.ContentType
    };
}
